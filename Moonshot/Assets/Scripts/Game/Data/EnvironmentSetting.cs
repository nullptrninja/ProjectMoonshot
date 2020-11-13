using Assets.Scripts.Game.Common;
using UnityEngine;

namespace Assets.Scripts.Game.Data {
    [CreateAssetMenu(fileName = "envSetting", menuName = "ScriptableObjects/Data/EnvironmentSetting")]
    public class EnvironmentSetting : ScriptableObject {
        public AtmosphereDescriptor[] AtmosphericLayers;
        public float Gravity;
        public WindSettings Wind;

    }
}
