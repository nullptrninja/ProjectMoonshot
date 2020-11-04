using Core.Behaviors.Functional.Sensors;
using UnityEngine;

namespace Core.Behaviors.Functional.Character {
    /// <summary>
    /// Handles 2D platformer jumping logic to handle the most commonly needed scenarios for a 2D platform game
    /// including variable jump heights and max jump forces.
    /// Note that the GroundSensor needs to be placed at the character's feet.
    /// </summary>
    public class JumpHandler : MonoBehaviour {
        public Vector2 JumpDirection = Vector2.up;
                
        public float MicroImpulseForce = 0.5f;          // How much additional force to apply past the initial force for additional jump control
        public float InitialImpulseForce = 10f;         // The minimum jump force
        public float MaxForce = 20f;                    // The maximum jump force
        public float ImpulseWindowSeconds = 0.5f;       // How much time the player has to apply micro impulses        
        public float GroundCheckDelaySeconds = 0.5f;    // In order to avoid prematurely determining that we've landed, we allow some number of seconds to 
                                                        // elapse before we check for grounded state.
        public GroundSensor GroundCheckSensor;
        public Rigidbody PlayerRigidBody;

        // State
        private bool mIsJumping;
        private bool mJumpKeyHeld;                      // Passed in from the host
        private bool mDisallowImpulseJumping;           // Only set when you release the jump key midflight
        private float mCurrentImpulseBuffer;
        private float mElapsedJumpTime;                 // Jump timer used for a variety of purposes

        private Vector2 mInitialJumpForce;              // Computed and cached
        private Vector2 mImpulseJumpForce;              // Computed and cached

        public void Awake() {
            this.mInitialJumpForce = this.JumpDirection * this.InitialImpulseForce;
            this.mImpulseJumpForce = this.JumpDirection * this.MicroImpulseForce;
        }

        public bool CanJump() {
            // TODO: If you want something like, double-jumping you'll have to modify this check
            return !mIsJumping && !mJumpKeyHeld  && this.GroundCheckSensor.HasCollidedWithGround();
        }

        /// <summary>
        /// Starts the initial jump. Note that this will not forbid you from repeated jumping, you
        /// must use CanJump() at your own discretion.
        /// </summary>
        public void BeginJump() {
            this.mIsJumping = true;
            this.mCurrentImpulseBuffer = this.InitialImpulseForce;
            this.mElapsedJumpTime = 0f;
            this.mJumpKeyHeld = true;
            this.mDisallowImpulseJumping = false;
            this.HasLanded = false; 

            this.PlayerRigidBody.AddForce(mInitialJumpForce, ForceMode.Impulse);
        }


        public void BeginFixedJump(Vector2 jumpVector) {
            this.mIsJumping = true;
            this.mCurrentImpulseBuffer = MaxForce;
            this.mJumpKeyHeld = true;
            this.mDisallowImpulseJumping = false;
            this.HasLanded = false;
            this.mElapsedJumpTime = this.ImpulseWindowSeconds;      // Disallow jump buffering

            this.PlayerRigidBody.AddForce(mInitialJumpForce + (-jumpVector * this.MaxForce), ForceMode.Impulse);
        }

        public void EndJump() {
            this.HasLanded = true;
            mIsJumping = false;
            mDisallowImpulseJumping = false;
        }
        
        /// <summary>
        /// Each frame your host needs to call this method to know when we've released the jump key
        /// or still holding it. The JumpHandler is input agnostic and doesn't give a shit which key
        /// is the actual jump key.
        /// </summary>
        /// <param name="isHeld">True if the key is still held down</param>
        public void UpdateJumpKeyHeld(bool isHeld) {
            if (mIsJumping && mJumpKeyHeld && !isHeld) {
                mDisallowImpulseJumping = true;
            }

            this.mJumpKeyHeld = isHeld;
        }

        public void Update() {
            if (this.mIsJumping) {
                this.mElapsedJumpTime += Time.deltaTime;

                // If the jump key is held we keep accumulating the impulse buffer if we're still within the impulse window
                if (!mDisallowImpulseJumping && mJumpKeyHeld && mElapsedJumpTime <= this.ImpulseWindowSeconds) {
                    mCurrentImpulseBuffer += this.MicroImpulseForce;

                    // If your impulse is larger than MaxForce we don't make up the difference. Make your adjustments wisely.
                    if (mCurrentImpulseBuffer <= this.MaxForce) {
                        this.PlayerRigidBody.AddForce(mImpulseJumpForce, ForceMode.Impulse);
                    }
                }

                if (mElapsedJumpTime >= this.GroundCheckDelaySeconds && this.GroundCheckSensor.HasCollidedWithGround()) {
                    EndJump();
                }
            }
        }

        // Properties

        public bool IsTouchingGround {
            get {
                return this.GroundCheckSensor.HasCollidedWithGround();
            }
        }

        /// <summary>
        /// Returns true if the player is mid-jump and is descending.
        /// </summary>
        public bool IsDescending {
            get {
                return mIsJumping && this.PlayerRigidBody.velocity.y < -0.5f;
            }
        }

        /// <summary>
        /// Returns true if the host is descending but has not started a jump. For example, if the underlying platform
        /// beneath the host has disappeared and the host begins falling as a result. There is a small velocity threshold that must be
        /// surpassed before this property returns true.
        /// </summary>
        public bool IsFreeFalling {
            get {
                return !mIsJumping && this.PlayerRigidBody.velocity.y < -0.5f && !this.GroundCheckSensor.HasCollidedWithGround();
            }
        }

        public bool IsJumping {
            get { return mIsJumping; }
        }

        public bool HasLanded {
            get; private set;
        }
    }
}
