using Core.Behaviors.Functional.Sensors;
using Core.Utility;
using System;
using UnityEngine;

namespace Core.Behaviors.Functional.Character {
    /// <summary>
    /// Provides top-down/isometric handling on a 2D plane with gravity support on the Y axis. Note that the physics units differ from
    /// RigidBody units and are not interchangeable. Here's how to tune handling to suit your project:
    /// 
    /// - Mass: Mass of the object, used in pushing-object calculations as a scalar unit
    /// - Acceleration: How many units/sec to increase velocity by per frame
    /// - AirControlScalar: A percentage of how much input control is applied when attempting to maneuver mid-air
    /// - MaxVelocity: Max velocity along X and Z directions
    /// - MaxSlope: The maximum angle of a slope that can be moved over (Currently not implemented)
    /// - MinStep: The minimum amount of acceleration that can be applied
    /// - Drag: How many units/sec to drag the X and Z velocity by    
    /// - Grip: time-scalar value that controls lateral traction when making sudden directional changes. The lower the value, the more "drifty"
    ///                         the character will move when changing directions. "5" is considered "a little slide-y", with "10" or higher
    ///                         exhibiting no sliding at all.
    /// - StopOnADime: If true the object moves 1-to-1 with the controller input and literally stops moving when no movement is applied
    /// - CollisionAppliedMassRatio: A multiplier against Mass used when this object is pushing into a RigidBody
    /// - PenetrationPercentage: The percentage of penetration this object's collider will stick into a colliding RigidBody. The higher the more pushing
    ///                         will be applied to the other RigidBody.
    /// - CollisionLayerMask: Which collision layer this object can collide with
    /// - UseGravity: If true, gravity is applied on the Y axis
    /// - GravityVect: The gravity vector to apply if gravity is enabled. Note this differs from the real gravity constant.
    /// 
    /// Other information:
    /// - This controller implements IIsometricCharacterController and will automatically register it with an ObjectResourceProvider if available
    /// </summary>
    public class NPCharacterControllerIsometric : MonoBehaviour, IIsometricCharacterController {
        private const float VelocityReductionRatio = 4f;        // Reduces the specified max velocity by this ratio
        private const float MaxGravityVelocity = -0.981f;

        public ObjectResourceProvider ResourceProvider;     // Optional dependency injector
        public CapsuleCollider Collider;
        public SensorMap Sensors;

        // Movement
        public float Mass;        
        public float Acceleration = 0.35f;         // Units per second
        public float AirControlScalar = 0.05f;      // Air movement control speed
        public float MaxVelocity = 0.75f;          // Note that at the moment this is mutable but it shouldn't be!
        public float MaxSlope = 45f;
        public float MinStep = 0.01f;
        public float Drag = 2.0f;                 // Vel decay per second when not applying movement
        public float Grip = 0.5f;                 // How quickly we resolve lateral-friction during movement        

        // Collision
        public float CollisionAppliedMassRatio = 0.45f;
        public float PenetrationPercentage = 0.2f;
        public int CollisionLayerMask = -5;

        // Characteristics
        public bool StopOnADime = false;          // If true, drag is applied but the player stops instantly instead when no input is registered
        public bool EnableGravity = true;
        public bool EnableCollisions = true;

        // Misc
        public Vector3 Gravity = new Vector3(0f, -0.0125f, 0f);

        /// <summary>
        /// Gets the current velocity
        /// </summary>
        public float Velocity => mCurrentVelocity.magnitude;

        /// <summary>
        /// Gets the squared moving velocity
        /// </summary>
        public float SqrVelocity => mCurrentVelocity.sqrMagnitude;

        /// <summary>
        /// Gets or sets the event handler that is signaled when a collision occurs
        /// </summary>
        public Action<Collider> OnCollisionHandler { get; set; }

        /// <summary>
        /// Gets the state of the character if they are falling or have collided with the ground
        /// </summary>
        public bool IsFalling { get; private set; }


        private float mCachedMaxVelocity;
        private Vector3 mCurrentVelocity;
        private Vector3 mLastInputVect;
        private float mPenetrationPct;
        private bool mMoveIsBeingApplied;

        public void Start() {
            if (this.ResourceProvider != null) {
                this.ResourceProvider.RegisterResource<IIsometricCharacterController>(this);
            }

            mCachedMaxVelocity = this.MaxVelocity / VelocityReductionRatio;         // This needs to be reduced by a factor because our moveVects aren't unit vects
            mPenetrationPct = this.PenetrationPercentage + 1f;
        }

        public void Move(Vector3 normalizedMoveVect) {
            var baseAccel = this.Acceleration;
            var applyAirControlRules = this.IsFalling && this.EnableGravity;

            var accel = Time.deltaTime * baseAccel;            
            if (accel > 0f && accel < this.MinStep && !applyAirControlRules) {
                accel = this.MinStep;
            }

            var accelVect = normalizedMoveVect * accel;
            mCurrentVelocity += accelVect;
                        
            mMoveIsBeingApplied = true;
            mLastInputVect = applyAirControlRules ? 
                                    normalizedMoveVect * this.AirControlScalar :
                                    normalizedMoveVect;

            CheckAndHandleStairs();
        }

        public void ReleaseMovement() {
            mMoveIsBeingApplied = false;

            if (this.StopOnADime) {
                mCurrentVelocity.x = 0f;
                mCurrentVelocity.z = 0f;
            }
        }

        public void StopMovement() {
            mMoveIsBeingApplied = false;
            mCurrentVelocity.x = 0f;
            mCurrentVelocity.z = 0f;

            if (!this.EnableGravity) {
                mCurrentVelocity.y = 0f;
            }
        }

        public void FaceObject(Transform obj) {
            this.transform.LookAt(obj);
        }

        public void MoveImpulse(Vector3 normalizedMoveVect, float maxVelocity) {
            var vel = normalizedMoveVect * maxVelocity;
            mCurrentVelocity = mCurrentVelocity + vel;

            mMoveIsBeingApplied = true;
            mLastInputVect = normalizedMoveVect;
            CheckAndHandleStairs();
        }

        public bool InterpolateToPoint(Vector3 targetPoint, float moveScalar, float tolerance)
        {
            var moveVect = VectorUtility.GetPointingVector3(this.transform.position, targetPoint);
            var dist = Vector3.Distance(this.transform.position, targetPoint);

            if (dist <= tolerance)
            {
                return true;
            }

            Move(moveVect * moveScalar);
            return false;
        }

        public void FixedUpdate() {
            if (this.EnableGravity) {
                ApplyGravity();
            }

            if (mCurrentVelocity.sqrMagnitude != 0f) {
                MoveStepInternal();
            }
            
            DecayVelocity();

            if (this.EnableCollisions) {
                DetectCollisions();
            }
        }

        private void MoveStepInternal() {
            // Clamp velocity here (Note: after gravity has already been clamped)
            mCurrentVelocity = mCurrentVelocity.ClampByAxis(mCachedMaxVelocity, 0f, mCachedMaxVelocity);            

            this.gameObject.transform.Translate(mCurrentVelocity, Space.World);
        }

        private void CheckAndHandleStairs() {
            if (this.Sensors.StairDetector.ShouldClimbStair(out var yOffset)) {
                var p = this.gameObject.transform.position;
                this.gameObject.transform.position = new Vector3(p.x, p.y + yOffset, p.z);
            }
        }

        private void DecayVelocity(bool forceDecay = false) {
            // Only apply drag if we've stopped explicitly moving            
            if (!mMoveIsBeingApplied || forceDecay) {
                var decay = Time.deltaTime * this.Drag;
                var invVel = -(mCurrentVelocity.normalized * decay);      // "Braking" vector
                mCurrentVelocity = mCurrentVelocity.DecayToZero(invVel.x, 0f, invVel.z);
            }
            else {
                // When moving we still need to decay velocity but differently than when we've stopped
                // movement commands; here we lerp between the intended input vector and the current velocity.
                // The lerp-time isn't used like traditional lerp, instead we have a lerp-rate (grip) that is
                // scaled by the elapsed time, the end effect is that we slowly lerp over time which also gives
                // us a lot of resolution to fine tune grip rates when changing direction.
                var preLerpGravity = mCurrentVelocity.y;
                var gripInterp = Vector3.Lerp(mCurrentVelocity, mLastInputVect, this.Grip * Time.deltaTime);
                mCurrentVelocity = gripInterp;

                // While we're moving, the lerp above will inadvertently also lerp the gravity component. So here,
                // if applicable, we re-establish gravity to ensure we can fall correctly if moving mid-air
                if (this.EnableGravity) {
                    mCurrentVelocity.y = preLerpGravity;
                }
            }
        }

        private void ApplyGravity() {
            // NOTE: We currently only support gravity on the Y-axis. If you need to utilize the other axes then
            // the change is trivial; just be sure to clamp correctly.
            mCurrentVelocity.y += this.Gravity.y * Time.deltaTime;
                
            if (mCurrentVelocity.y < MaxGravityVelocity) {
                mCurrentVelocity.y = MaxGravityVelocity;
            }
        }

        private void DetectCollisions() {
            var colliders = PhysicsExtensions.OverlapCapsule(this.Collider, this.CollisionLayerMask, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0) {
                var velMag = mCurrentVelocity.sqrMagnitude * this.Mass * this.CollisionAppliedMassRatio;

                for (var i = 0; i < colliders.Length; ++i) {
                    var c = colliders[i];
                    if (this.Collider != c) {
                        HandleCollision(c, velMag);
                    }
                }

                this.IsFalling = !this.Sensors.GroundSensors.HasCollidedWithGround();
                if (!this.IsFalling) {
                    mCurrentVelocity.y = 0f;
                }
            }
            else {
                this.IsFalling = true;
            }
        }

        private void HandleCollision(Collider other, float thisVelocityMagnitude) {
            // Resolution guide from:
            // https://forum.unity.com/threads/we-need-a-way-to-detect-and-resolve-collision-hits-manually.379699/

            var otherGameObj = other.gameObject;
            var r = Physics.ComputePenetration(this.Collider, this.transform.position, this.transform.rotation,
                                               other, otherGameObj.transform.position, otherGameObj.transform.rotation,
                                               out var resolveDirVect,
                                               out var resolveDist);
            if (r) {
                Vector3 mtd;
                if (other.attachedRigidbody != null) {
                    mtd = (resolveDirVect * resolveDist) * mPenetrationPct;

                    // We apply an opposite force to the colliding rigidbody if applicable using skin penetration
                    // to force an intersection into the other collider
                    other.attachedRigidbody.AddForce(-resolveDirVect * thisVelocityMagnitude, ForceMode.Force);
                }
                else {
                    mtd = resolveDirVect * resolveDist;                    
                }
                this.gameObject.transform.Translate(mtd, Space.World);                
            }

            this.OnCollisionHandler?.Invoke(other);
        }
    }
}
