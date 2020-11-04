using Core.Behaviors.Functional.Level;
using UnityEngine;

namespace Core.Behaviors.Functional.Sensors
{
    /// <summary>
    /// Provides abstracted methods to assist in ladder detection and traversal.
    /// 
    /// Implementation:
    /// You need two OverlapSensors:
    /// - One at foot level in front of the object
    /// - One at head level in front of the object
    /// - Both sensors should be vertically aligned
    /// </summary>
    public class LadderSensor : MonoBehaviour
    {
        public OverlapSensor TopSensor;
        public OverlapSensor BottomSensor;

        public Ladder OverlappingLadder => GetBottomOverlappingLadder() ?? GetTopOverlappingLadder();

        public void Start() {
            this.TopSensor.SetTypeFilter(typeof(Ladder));
            this.BottomSensor.SetTypeFilter(typeof(Ladder));
        }

        public bool IsFootOverlappingLadder()
        {
            return GetBottomOverlappingLadder() != null;
        }

        public bool IsHeadOverlappingLadder()
        {
            return GetTopOverlappingLadder() != null;
        }

        public bool IsHeadAndFootOverlappingLadder()
        {
            var result = IsFootOverlappingLadder() && IsHeadOverlappingLadder();
            return result;
        }

        private Ladder GetTopOverlappingLadder()
        {
            this.TopSensor.ForceRefresh();
            var obj = this.TopSensor.OverlappingObject?.GetComponent<Ladder>();
            return obj;
        }

        private Ladder GetBottomOverlappingLadder()
        {
            this.BottomSensor.ForceRefresh();
            var obj = this.BottomSensor.OverlappingObject?.GetComponent<Ladder>();
            return obj;
        }
    }
}
