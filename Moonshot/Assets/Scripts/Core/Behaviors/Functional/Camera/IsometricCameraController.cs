using Core.Utility;
using UnityEngine;

namespace Core.Behaviors.Functional.Camera {
    public class IsometricCameraController : MonoBehaviour {
        private const float WaitForTargetCheckIntervalSeconds = 1f;
        private const float AtTargetDistanceEvalIntervalSeconds = 1f;

        private enum State {
            AtTarget,
            DecelToTarget,
            SeekingTarget,
            WaitForTarget,
            FreeLook
        }

        public Transform FollowTarget;
        public float SeekDurationSeconds;
        public float SnapToTargetTimeSeconds;
        public float DecelerationDistanceThreshold;
        public float SnapToTargetThreshold;
        public Vector3 MinDistanceOffsets;

        private State mState;
        private float mElapsedTime;
        private Vector3 mFixedLookAtOffset;

        public void Start() {
            mFixedLookAtOffset = GetLookAtPositionOffset();
            GoToWaitForTarget();
        }

        public void Update() {
            switch (mState) {
                case State.AtTarget:
                    DoAtTarget();
                    break;

                case State.DecelToTarget:
                    DoDecelToTarget();
                    break;

                case State.SeekingTarget:
                    DoSeekingTarget();
                    break;

                case State.WaitForTarget:
                    DoWaitForTarget();
                    break;

                case State.FreeLook:
                    DoFreeLook();
                    break;
            }
        }

        // -----------------

        private void GoToSeekingTarget() {
            mState = State.SeekingTarget;
            mElapsedTime = 0f;
        }

        private void DoSeekingTarget() {
            if (this.FollowTarget != null) {
                var positionToSeekTo = OffsetVector(this.FollowTarget.position, mFixedLookAtOffset);

                mElapsedTime += Time.deltaTime;
                var pctComplete = mElapsedTime / this.SeekDurationSeconds;

                //var seekPosition = this.transform.position.SmoothMove(positionToSeekTo, pctComplete);
                var seekPosition = Vector3.Slerp(this.transform.position, positionToSeekTo, pctComplete);
                //var seekPosition = Vector3.Lerp(this.transform.position, positionToSeekTo, pctComplete);

                this.transform.position = seekPosition;
                var dist = GetSqrDistanceTo(this.transform.position, positionToSeekTo);
                if (dist <= this.DecelerationDistanceThreshold) {
                    GoToDecelToTarget();
                }
            }
            else {
                GoToWaitForTarget();
            }
        }

        private void GoToFreeLook() {
            mState = State.FreeLook;
        }

        private void DoFreeLook() {
        }

        private void GoToDecelToTarget() {
            mState = State.DecelToTarget;
            mElapsedTime = 0f;
        }

        private void DoDecelToTarget() {
            if (this.FollowTarget != null) {
                var positionToSeekTo = OffsetVector(this.FollowTarget.position, mFixedLookAtOffset);

                mElapsedTime += Time.deltaTime;
                var pctComplete = mElapsedTime / this.SnapToTargetTimeSeconds;

                var seekPosition = this.transform.position.SmoothMove(positionToSeekTo, pctComplete);
                //var seekPosition = Vector3.Slerp(this.transform.position, positionToSeekTo, pctComplete);

                this.transform.position = seekPosition;
                var dist = GetSqrDistanceTo(this.transform.position, positionToSeekTo);
                if (dist <= this.SnapToTargetThreshold) {
                    GoToAtTarget();
                }
            }
            else {
                GoToWaitForTarget();
            }
        }

        private void GoToWaitForTarget() {
            mState = State.WaitForTarget;
            mElapsedTime = 0f;
        }

        private void DoWaitForTarget() {
            mElapsedTime += Time.deltaTime;

            if (mElapsedTime >= WaitForTargetCheckIntervalSeconds) {
                mElapsedTime = 0f;

                if (this.FollowTarget != null) {
                    GoToSeekingTarget();
                }
            }
        }

        private void GoToAtTarget() {
            mState = State.AtTarget;
            mElapsedTime = 0f;
        }

        private void DoAtTarget() {
            if (this.FollowTarget != null) {
                mElapsedTime += Time.deltaTime;
                var targetPosition = OffsetVector(this.FollowTarget.position, mFixedLookAtOffset);

                if (mElapsedTime >= AtTargetDistanceEvalIntervalSeconds) {
                    // Check for distance to target. This is usually needed
                    // when the follow target changes suddenly or if the current
                    // target teleports outside of the decel distance
                    var dist = GetSqrDistanceTo(this.transform.position, targetPosition);
                    if (dist > this.DecelerationDistanceThreshold) {
                        GoToSeekingTarget();
                        return;
                    }

                    mElapsedTime = 0f;
                }

                // Snap to target
                this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, 1f);
            }
        }

        // ----------------

        private static float GetSqrDistanceTo(Vector3 from, Vector3 target) {
            //var fromIgnoreY = new Vector3(from.x, 0f, from.z);
            //var targetIgnoreZ = new Vector3(target.x, 0f, target.z);
            return (from - target).sqrMagnitude;
        }

        private Vector3 OffsetVector(Vector3 seekPositionOriginal, Vector3 offset) {
            return seekPositionOriginal + offset;
        }

        private Vector3 GetLookAtPositionOffset() {            
            return new Vector3(-this.MinDistanceOffsets.x,
                               this.MinDistanceOffsets.y,
                               -this.MinDistanceOffsets.z);
        }
    }
}
