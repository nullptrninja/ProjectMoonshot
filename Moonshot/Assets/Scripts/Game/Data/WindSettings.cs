using System;
using Assets.Scripts.Game.Common;

namespace Assets.Scripts.Game.Data {
    [Serializable]
    public class WindSettings {
        public CompassDirection Direction;
        public float WindSpeed;
        public float ImpulseDurationSeconds;
    }
}
