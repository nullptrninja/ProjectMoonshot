using System.Linq;
using Core.Utility;
using UnityEngine;

namespace Core.Behaviors.Functional.Level {
    public class Ladder : MonoBehaviour {
        public enum TraversalDirection
        {
            Vertical,
            Horizontal
        }

        public Collider TriggerVolume;
        public Transform Start;
        public Transform End;
        public float TopLandingOffset;
        public float BottomLandingOffset;

        public Vector3 TopLanding { get; private set; }
        public Vector3 BottomLanding { get; private set; }
        public Vector3 Slope { get; private set; }

        private Vector3[] mStartToEndWayPoints;
        private Vector3[] mEndToStartWayPoints;

        public void Awake()
        {
            this.Slope = VectorUtility.GetPointingVector3(this.Start, this.End);

            var startPos = this.Start.transform.position;
            var endPos = this.End.transform.position;
            this.BottomLanding = startPos + (this.transform.forward * this.BottomLandingOffset);
            this.TopLanding = endPos + (this.transform.forward * -this.TopLandingOffset);

            mStartToEndWayPoints = new Vector3[] {
                this.BottomLanding,
                this.Start.position,
                this.End.position,
                this.TopLanding
            };

            mEndToStartWayPoints = mStartToEndWayPoints.Reverse().ToArray();
        }

        public Vector3[] GetStartToEndWaypoints() {
            return mStartToEndWayPoints;
        }

        public Vector3[] GetEndToStartWaypoints() {
            return mStartToEndWayPoints;
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.green;

            var startPos = this.Start.transform.position;
            var endPos = this.End.transform.position;
            var bottomLanding = startPos + (this.transform.forward * this.BottomLandingOffset);
            var topLanding = endPos + (this.transform.forward * -this.TopLandingOffset);

            Gizmos.DrawWireSphere(bottomLanding, 0.15f);
            Gizmos.DrawLine(this.Start.position, bottomLanding);

            Gizmos.DrawLine(this.Start.position, this.End.position);

            Gizmos.DrawWireSphere(topLanding, 0.15f);
            Gizmos.DrawLine(this.End.position, topLanding);
        }


        //private Vector3 ResolveVerticalAttachmentPoint(Vector3 rawContactPt)
        //{
        //    // We align the CP relative to the base of the start position node on all axis except the vertical position
        //    // as that can vary at the point of attachment. This way we can use the CP in a dot product to find the projection pt.
        //    var alignedCp = new Vector3(this.Start.position.x, rawContactPt.y, this.Start.position.z);

        //    // Get pointing + normalized vector from start to alignedCp
        //    var normalizedCp = VectorUtility.GetPointingVector3(this.Start.position, alignedCp);
        //    var ladderEdgeN = VectorUtility.GetPointingVector3(this.Start, this.End);

        //    // Project alignedCpN onto ladderEdgeN to get the final position scalar.
        //    var projectOntoLadderScalar = Vector3.Dot(normalizedCp, ladderEdgeN);
        //    var position = ladderEdgeN * projectOntoLadderScalar;

        //    return position;
        //}
    }
}
