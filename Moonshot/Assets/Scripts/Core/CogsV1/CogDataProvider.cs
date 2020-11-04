using Core.CogsV1;
using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// When attached to a GameObject that can trigger cogs, holds data that would be
    /// passed into the triggered cog. This assumes that the object activating the cog
    /// sends itself into the Activate/Deactivate function call as the "sender" parameter.
    /// </summary>
    public class CogDataProvider : MonoBehaviour {
        public CogData Data;
    }
}
