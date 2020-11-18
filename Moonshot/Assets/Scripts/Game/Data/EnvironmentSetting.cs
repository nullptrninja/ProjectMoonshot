using UnityEngine;

namespace Assets.Scripts.Game.Data {
    [CreateAssetMenu(fileName = "envSetting", menuName = "ScriptableObjects/Data/EnvironmentSetting")]
    public class EnvironmentSetting : ScriptableObject {
        public Vector2 AreaSize;
        public AtmosphereDescriptor[] AtmosphericLayers;
        public float Gravity;
        public WindSettings Wind;

    }
}
