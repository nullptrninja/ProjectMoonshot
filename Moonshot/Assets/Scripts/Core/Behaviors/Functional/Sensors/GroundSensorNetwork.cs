namespace Core.Behaviors.Functional.Sensors {
    /// <summary>
    /// Often times a single GroundSensor does not cover a wide enough area; to work around that for certain games, a network of
    /// GroundSensors is needed. This component acts as a single sensor but aggregates the results from an array of "child" sensors
    /// to gain a consensus result. We made this component itself a GroundSensor so that a single sensor is interchangable with
    /// a sensor network.
    /// 
    /// NOTE: The properties attached to this sensor are ignored! The individual child sensors are used instead.
    /// </summary>
    public class GroundSensorNetwork : GroundSensor {
        public bool UseSelfAsSensor;
        public GroundSensor[] ChildSensors;

        public override bool HasCollidedWithGround() {
            var consensusResult = false;
            for (int i = 0; i < this.ChildSensors.Length; ++i) {
                consensusResult |= this.ChildSensors[i].HasCollidedWithGround();
            }

            if (UseSelfAsSensor) {
                consensusResult |= base.HasCollidedWithGround();
            }

            return consensusResult;
        }

        public override bool IsCloseToGround() {
            var consensusResult = false;
            for (int i = 0; i < this.ChildSensors.Length; ++i) {
                consensusResult |= this.ChildSensors[i].IsCloseToGround();
            }

            if (UseSelfAsSensor) {
                consensusResult |= base.IsCloseToGround();
            }

            return consensusResult;
        }

        public override bool IsCloseToGroundButNotTouching() {
            var consensusResult = false;
            for (int i = 0; i < this.ChildSensors.Length; ++i) {
                consensusResult |= this.ChildSensors[i].IsCloseToGroundButNotTouching();
            }

            if (UseSelfAsSensor) {
                consensusResult |= base.IsCloseToGroundButNotTouching();
            }

            return consensusResult;
        }
    }
}
