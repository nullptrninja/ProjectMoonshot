using UnityEngine;

namespace Core.Behaviors.Functional.Sensors {
    /// <summary>
    /// Nothing fancy, just aggregates all sensors into one component for easy referencing
    /// </summary>
    public class SensorMap : MonoBehaviour {
        public GroundSensor Ground;
        public GroundSensorNetwork GroundSensors;
        public StairDetectionSensor StairDetector;
    }
}
