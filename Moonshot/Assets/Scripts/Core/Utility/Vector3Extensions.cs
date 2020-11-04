using UnityEngine;

namespace Core.Utility
{
    public static class Vector3Extensions
    {        
        public static bool HasNan(this Vector3 t1)
        {
            return float.IsNaN(t1.x) || float.IsNaN(t1.y) || float.IsNaN(t1.z);
        }  
        
        public static Vector3 SmoothMove(this Vector3 current, Vector3 target, float percentComplete)
        {
            float x = Mathf.SmoothStep(current.x, target.x, percentComplete);
            float y = Mathf.SmoothStep(current.y, target.y, percentComplete);
            float z = Mathf.SmoothStep(current.z, target.z, percentComplete);

            return new Vector3(x, y, z);
        }

        public static bool ApproxEquals(this Vector3 current, Vector3 target)
        {
            return Mathf.Approximately(current.x, target.x) &&
                   Mathf.Approximately(current.y, target.y) &&
                   Mathf.Approximately(current.z, target.z);
        }
    }
}
