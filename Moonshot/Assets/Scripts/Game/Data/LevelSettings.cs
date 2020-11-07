using UnityEngine;

namespace Assets.Scripts.Game.Data {
    [CreateAssetMenu(fileName = "level", menuName = "ScriptableObjects/Data/LevelSetting")]
    public class LevelSettings : ScriptableObject {
        public EnvironmentSetting Environment;
    }
}
