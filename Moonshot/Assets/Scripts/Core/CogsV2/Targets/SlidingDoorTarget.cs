using Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.CogsV2.Targets
{
    public class SlidingDoorTarget : CogTriggerTarget
    {
        [Serializable]
        public enum SlideAxis
        {
            XAxis,
            YAxis,
            ZAxis
        }

        private enum State
        {
            Closed,
            Opening,
            WaitingToClose,
            Closing,
            Open
        }

        public SlideAxis SlideOnAxis;
        public Collider SlideCollider;
        public float MoveTime;
        public float SlidePercentage;       // Normalized range (0-1) of pct of door movement when sliding
        public bool FlipSlideDirection;
        public bool ThisPositionIsOpen;
        public bool AutoClose;
        public float TimeBeforeClose;
        public bool AllowCloseByTrigger;

        private State mState;
        private Vector3 mTargetPosition;
        private Vector3 mOpenPosition;
        private Vector3 mClosedPosition;
        private float mElapsedTime;
        
        public override bool Activate(CogTriggerSource source, CommonTargetAction action, TriggerData triggerData)
        {
            if (action == CommonTargetAction.Open || action == CommonTargetAction.Activate)
            {
                if (mState == State.Closed || mState == State.Closing)
                {
                    GoToOpening();
                    return true;
                }
            }
            else if (action == CommonTargetAction.Close && this.AllowCloseByTrigger)
            {
                if (mState != State.Closed || mState != State.Closing)
                {
                    GoToClosing();
                    return true;
                }
            }

            return false;
        }

        public void Start()
        {
            if (this.MoveTime <= 0f)
            {
                this.MoveTime = 1f;
                Debug.Log($"SlidingDoorTarget: {this.name} has MoveTime of 0. Defaulting to 1");
            }

            ComputePositions();
            if (this.ThisPositionIsOpen)
            {
                GoToOpen();
            }
            else
            {
                GoToClosed();
            }
        }

        public void Update()
        {
            switch (mState)
            {
                case State.Opening:
                    DoOpening();
                    break;

                case State.Closing:
                    DoClosing();
                    break;

                case State.WaitingToClose:
                    DoWaitingToClose();
                    break;
            }
        }

        private void GoToOpen()
        {
            mState = State.Open;
        }

        private void GoToClosed()
        {
            mState = State.Closed;
        }

        private void GoToOpening()
        {
            mState = State.Opening;
            mTargetPosition = mOpenPosition;
            mElapsedTime = 0f;
        }

        private void DoOpening()
        {
            mElapsedTime += Time.deltaTime;
            if (DoMoveToTarget())
            {
                if (this.AutoClose)
                {
                    GoToWaitingToClose();
                }
                else
                {
                    GoToOpen();
                }
            }
        }

        private void GoToClosing()
        {
            mState = State.Closing;
            mTargetPosition = mClosedPosition;
            mElapsedTime = 0f;
        }

        private void DoClosing()
        {
            mElapsedTime += Time.deltaTime;
            if (DoMoveToTarget())
            {
                GoToClosed();
            }
        }

        private void GoToWaitingToClose()
        {
            mState = State.WaitingToClose;
            mElapsedTime = 0f;
        }

        private void DoWaitingToClose()
        {
            mElapsedTime += Time.deltaTime;
            if (mElapsedTime >= this.TimeBeforeClose)
            {
                GoToClosing();
            }
        }

        private bool DoMoveToTarget()
        {
            var pct = mElapsedTime / this.MoveTime;
            if (pct < 1f)
            {
                var moveVect = (mTargetPosition - this.transform.position).normalized;                
                var pos = this.transform.position.SmoothMove(mTargetPosition, pct);
                this.transform.position = pos;

                return false;
            }
            return true;
        }

        private void ComputePositions()
        {
            var slideDist = ComputeSlideDistance();
            var flipDirScalar = this.FlipSlideDirection ? -1 : 1;
            
            if (this.ThisPositionIsOpen)                
            {
                mOpenPosition = this.transform.position;
                mClosedPosition = ComputeTerminalPosition(mOpenPosition, flipDirScalar * slideDist);
            }
            else
            {
                mClosedPosition = this.transform.position;
                mOpenPosition = ComputeTerminalPosition(mClosedPosition, flipDirScalar * slideDist);
            }
        }

        private Vector3 ComputeTerminalPosition(Vector3 startingPosition, float traverseDistance)
        {            
            if (this.SlideOnAxis == SlideAxis.XAxis)
            {
                var pos = new Vector3(startingPosition.x + traverseDistance,
                                      startingPosition.y,
                                      startingPosition.z);
                return pos;
            }
            else if (this.SlideOnAxis == SlideAxis.YAxis)
            {
                var pos = new Vector3(startingPosition.x,
                                      startingPosition.y + traverseDistance,
                                      startingPosition.z);
                return pos;
            }
            else
            {
                var pos = new Vector3(startingPosition.x,
                                      startingPosition.y,
                                      startingPosition.z + traverseDistance);
                return pos;
            }
        }

        private float ComputeSlideDistance()
        {
            var bounds = this.SlideCollider.bounds;
            var min = bounds.min;
            var max = bounds.max;

            float dist;
            if (this.SlideOnAxis == SlideAxis.XAxis)
            {
                dist = max.x - min.x;
            }
            else if (this.SlideOnAxis == SlideAxis.YAxis)
            {
                dist = max.y - min.y;
            }
            else
            {
                dist = max.z - min.z;
            }

            return dist * this.SlidePercentage;
        }
    }
}
