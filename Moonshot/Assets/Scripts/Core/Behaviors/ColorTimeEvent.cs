using Core.Utility;
using System;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// This is used for LightAttenuator since Unity won't accept a generic type as part of
    /// a behaviour's property type.
    /// </summary>
    [Serializable]
    public class ColorTimeEvent : TimeEvent<Color> {
    }
}
