using System;
using UnityEngine;

namespace Core.Utility
{
    public static class Vector2Extensions
    {
        public static bool HasNan(this Vector2 t1)
        {
            return float.IsNaN(t1.x) || float.IsNaN(t1.y);
        }

        public static Vector2 SmoothMove(this Vector2 current, Vector2 target, float percentComplete)
        {
            float x = Mathf.SmoothStep(current.x, target.x, percentComplete);
            float y = Mathf.SmoothStep(current.y, target.y, percentComplete);            

            return new Vector2(x, y);
        }

        public static bool Approximately(this Vector2 current, Vector2 target)
        {
            if (Mathf.Approximately(current.x, target.x) &&
                Mathf.Approximately(current.y, target.y))
            {
                return true;
            }
            return false;
        }

        public static Vector2 Perpendicular(this Vector2 v) {
            return new Vector2(-v.y, v.x);
        }
    }
}
