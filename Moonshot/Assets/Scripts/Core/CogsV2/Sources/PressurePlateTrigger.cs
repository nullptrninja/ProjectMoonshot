using System.Collections;
using UnityEngine;

namespace Core.CogsV2.Sources
{
    public class PressurePlateTrigger : CogTriggerSource
    {
        private enum State
        {
            Depressed,
            PreActivation,
            Pressed,
            CoolingDown
        }

        public bool FireActionOnDepress;
        public CommonTargetAction DepressedTargetAction;
        public Collider ActivationZone;
        public bool OnlyActivateOnRigidBody;        
        public float MinimumActivationMass;
        public float TimeBeforeActivate;
        public float CooldownTime;

        private float mElapsedTime;
        private bool mReady;
        private State mState;

        public void Awake()
        {
            mState = State.Depressed;
            mReady = true;
            mElapsedTime = 0f;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (mReady && !collider.isTrigger)
            {
                if (this.OnlyActivateOnRigidBody)
                {
                    // Do two searches just in case the collider doesn't have the rigidbody.
                    // Note that we use the inline conditional instead of null coalesce because some Unity types don't compare
                    // to null correctly when run in Editor mode (but will compare fine when expliclty using == operator to null)
                    var bodyAtColliderLevel = collider.gameObject.GetComponent<Rigidbody>();
                    var body = bodyAtColliderLevel == null ? collider.transform.root.GetComponent<Rigidbody>() : bodyAtColliderLevel;
                    if (body != null && body.mass >= this.MinimumActivationMass && mState == State.Depressed)
                    {
                        mState = State.PreActivation;
                        mElapsedTime = 0f;                        
                    }
                }
                else if (mState == State.Depressed)
                {
                    mState = State.PreActivation;
                    mElapsedTime = 0f;
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            // TODO: This will deactivate if any object leaves the activation zone, make this track who's
            // sitting on top so we don't deactivate by accident if something randomly leaves while something
            // is still sitting in the zone.
            if (!collider.isTrigger)
            {
                if (mState == State.PreActivation || mState == State.Depressed)
                {
                    mState = State.Depressed;
                    mElapsedTime = 0f;
                    mReady = true;
                }
                else if (mState == State.Pressed)
                {
                    if (this.FireActionOnDepress)
                    {
                        this.TriggerTarget?.Activate(this, this.DepressedTargetAction, this.DataOnTrigger);
                        mState = State.Depressed;
                        mElapsedTime = 0f;
                        mReady = true;
                    }
                }
            }
        }

        public void Update()
        {
            if (mState == State.PreActivation)
            {
                mElapsedTime += Time.deltaTime;
                if (mElapsedTime >= this.TimeBeforeActivate)
                {
                    Debug.Log("PressurePlateTrigger activated");
                    this.TriggerTarget?.Activate(this, this.TargetAction, this.DataOnTrigger);
                    mState = State.Pressed;
                    mElapsedTime = 0f;
                    mReady = false;

                    StartCoroutine(BeginCooldownRoutine());
                }
            }
        }

        private IEnumerator BeginCooldownRoutine()
        {
            yield return new WaitForSeconds(this.CooldownTime);
            mReady = true;
        }
    }
}
