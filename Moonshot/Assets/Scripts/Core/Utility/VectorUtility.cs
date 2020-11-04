using System;
using UnityEngine;

namespace Core.Utility {
    public static class VectorUtility {
        private const float DegreesToRadians = (float)Math.PI / 180f;
        private const float RadiansToDegrees = (float)(180f / Math.PI);
        private const float NinetyDegAsRads = DegreesToRadians * 90f;

        /// <summary>
        /// Generates a randomized vector at the specified positive and negative limits
        /// </summary>
        /// <param name="xLim">Maximum X value (inclusive)</param>
        /// <param name="yLim">Maximum Y value (inclusive)</param>
        /// <returns>Random vector</returns>
        public static Vector2 RandomVector2(float xLim, float yLim) {
            float x = UnityEngine.Random.Range(-xLim, xLim);
            float y = UnityEngine.Random.Range(-yLim, yLim);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Generates a randomized vector at the specified positive range
        /// </summary>
        /// <param name="xLim">Maximum X value (inclusive)</param>
        /// <param name="yLim">Maximum Y value (inclusive)</param>
        /// <returns>Random vector</returns>
        public static Vector2 RandomPositiveVector2(float xLim, float yLim) {
            float x = UnityEngine.Random.Range(0f, xLim);
            float y = UnityEngine.Random.Range(0f, yLim);

            return new Vector2(x, y);
        }

        public static Vector2 GetRotatedVector2(float angleDegrees) {
            return new Vector2((float)Math.Cos(angleDegrees), (float)Math.Sin(angleDegrees));
        }
                
        /// <summary>
        /// Generates a randomized vector at the specified positive and negative limits
        /// </summary>
        /// <param name="xLim">Maximum X value (inclusive)</param>
        /// <param name="yLim">Maximum Y value (inclusive)</param>
        /// <param name="zLim">Maximum Z value (inclusive)</param>
        /// <returns>Random vector</returns>
        public static Vector3 RandomVector3(float xLim, float yLim, float zLim) {
            float x = UnityEngine.Random.Range(-xLim, xLim);
            float y = UnityEngine.Random.Range(-yLim, yLim);
            float z = UnityEngine.Random.Range(-zLim, zLim);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Generates a randomized vector at the specified positive range
        /// </summary>
        /// <param name="xLim">Maximum X value (inclusive)</param>
        /// <param name="yLim">Maximum Y value (inclusive)</param>
        /// <param name="zLim">Maximum Z value (inclusive)</param>
        /// <returns>Random vector</returns>
        public static Vector3 RandomPositiveVector3(float xLim, float yLim, float zLim) {
            float x = UnityEngine.Random.Range(0f, xLim);
            float y = UnityEngine.Random.Range(0f, yLim);
            float z = UnityEngine.Random.Range(0f, zLim);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Returns a normalized vector originating at the 'from' object towards
        /// the 'to' object
        /// </summary>
        /// <returns>Normalized vector pointing towards the 'to' object</returns>
        public static Vector2 GetPointingVector2(Transform from, Transform to) {
            float dY = to.position.y - from.position.y;
            float dX = to.position.x - from.position.x;

            Vector2 v = new Vector2(dX, dY);
            if (v != Vector2.zero) {
                v.Normalize();
            }

            return v;
        }

        /// <summary>
        /// Returns a normalized vector originating at the 'from' object towards
        /// the 'to' object
        /// </summary>
        /// <returns>Normalized vector pointing towards the 'to' object</returns>
        public static Vector2 GetPointingVector2(Vector2 from, Vector2 to) {
            float dY = to.y - from.y;
            float dX = to.x - from.x;

            Vector2 v = new Vector2(dX, dY);
            if (v != Vector2.zero) {
                v.Normalize();
            }

            return v;
        }

        /// <summary>
        /// Returns a normalized vector originating at the 'from' object towards
        /// the 'to' object
        /// </summary>
        /// <returns>Normalized vector pointing towards the 'to' object</returns>
        public static Vector3 GetPointingVector3(Transform from, Transform to) {
            float dY = to.position.y - from.position.y;
            float dX = to.position.x - from.position.x;
            float dZ = to.position.z - from.position.z;

            Vector3 v = new Vector3(dX, dY, dZ);
            if (v != Vector3.zero) {
                v.Normalize();
            }

            return v;
        }

        /// <summary>
        /// Returns a normalized vector originating at the 'from' object towards
        /// the 'to' object
        /// </summary>
        /// <returns>Normalized vector pointing towards the 'to' object</returns>
        public static Vector3 GetPointingVector3(Vector3 from, Vector3 to) {
            var dY = to.y - from.y;
            var dX = to.x - from.x;
            var dZ = to.z - from.z;

            var v = new Vector3(dX, dY, dZ);
            if (v != Vector3.zero) {
                v.Normalize();
            }

            return v;
        }

        /// <summary>
        /// Finds the angle between two vectors. Note: If using the mouse position
        /// as one of the parameters, be sure to convert it to World coordinates from Screen.
        /// </summary>
        /// <param name="v1">First Vector start point</param>
        /// <param name="v2">Second Vector end point</param>
        /// <returns>Angle (in degrees) between the two vectors</returns>
        public static float FindAngleBetween(Vector3 v1, Vector3 v2) {
            return (float)Math.Atan2(v2.y - v1.y, v2.x - v1.x) * RadiansToDegrees;
        }

        /// <summary>
        /// Returns a random unit vector between two angles. This function was adapted
        /// from the Manticore version.
        /// </summary>
        /// <param name="minAngle">Min angle (inclusive)</param>
        /// <param name="maxAngle">Max angle (inclusive)</param>
        /// <returns>Vector2 pointing at a random angle</returns>
        public static Vector2 GetRandomAngledVector(float minAngle, float maxAngle) {
            float angle = UnityEngine.Random.Range(minAngle, maxAngle);

            Vector2 v = Vector2.zero;
            float rad = angle * DegreesToRadians;
            v.x = (float)Math.Sin(rad);
            v.y = (float)Math.Cos(rad);

            if (Math.Abs(v.x) < 0.001f) {
                v.x = 0f;
            }
            if (Math.Abs(v.y) < 0.001f) {
                v.y = 0f;
            }

            return v;
        }

        /// <summary>
        /// Wiggles the vector by a maximum amount while maintaining the direction
        /// </summary>
        /// <param name="srcVec">Original vector to wiggle</param>
        /// <param name="maxWiggle">Between 1 and X, how much to maximally wiggle the vector.  The higher the value the less wiggle there is.</param>
        /// <returns>Wiggled vector</returns>
        public static Vector2 WiggleVector(Vector2 srcVec, int maxWiggle) {
            if (srcVec == Vector2.zero || maxWiggle == 0f) {
                return srcVec;
            }

            Vector2 normalized = srcVec;
            normalized.Normalize();

            float scalar = 1f / UnityEngine.Random.Range(1, maxWiggle);
            normalized = normalized * scalar;

            // Randomly negate the vector
            if (UnityEngine.Random.Range(0, 10) < 5) {
                normalized = -normalized;
            }

            // Swap the components
            float tX = normalized.x;
            normalized.x = normalized.y;
            normalized.y = tX;


            return srcVec + normalized;
        }

        public static Vector2 Rotate(this Vector2 v, float angle) {
            var rads = angle * DegreesToRadians;
            var tx = v.x;
            var cosRads = (float)Math.Cos(rads);
            var sinRads = (float)Math.Sin(rads);

            v.x = v.x * cosRads - v.y * sinRads;
            v.y = tx * sinRads + v.y * cosRads;
            return v;
        }

        public static Vector3 RotateXZPlanar(this Vector3 v, float angle) {
            var rads = angle * DegreesToRadians;
            var tx = v.x;
            var cosRads = (float)Math.Cos(rads);
            var sinRads = (float)Math.Sin(rads);

            v.x = (v.x * cosRads) - (v.z * sinRads);
            v.z = (tx * sinRads) + (v.z * cosRads);
            return v;
        }

        public static Vector3 ToOrthogonalXZPlanar(this Vector3 v) {
            // We _could_ implement in terms of RotateXYPlanar but this saves us the multiply
            var rads = NinetyDegAsRads;
            var tx = v.x;
            var cosRads = (float)Math.Cos(rads);
            var sinRads = (float)Math.Sin(rads);

            v.x = v.x * cosRads - v.z * sinRads;
            v.z = tx * sinRads + v.z * cosRads;
            return v;
        }

        public static Vector2 ToCardinal(this Vector2 v) {
            var absX = Math.Abs(v.x);
            var absY = Math.Abs(v.y);

            // X gets the bias treatment
            if (absX >= absY) {
                return new Vector2(v.x, 0f);
            }
            else {
                return new Vector2(0f, v.y);
            }
        }

        public static Vector3 DecayToZero(this Vector3 v, float dx, float dy, float dz) {
            var x = v.x;
            var y = v.y;
            var z = v.z;

            if (dx != 0f) {
                if (x > 0f) {
                    x += dx;
                    if (x < 0f) {
                        x = 0f;
                    }
                }
                else if (x < 0f) {
                    x += dx;
                    if (x > 0f) {
                        x = 0f;
                    }
                }
            }

            if (dy != 0f) {
                if (y > 0f) {
                    y += dy;
                    if (y < 0f) {
                        y = 0f;
                    }
                }
                else if (y < 0f) {
                    y += dy;
                    if (y > 0f) {
                        y = 0f;
                    }
                }
            }

            if (dz != 0f) {
                if (z > 0f) {
                    z += dz;
                    if (z < 0f) {
                        z = 0f;
                    }
                }
                else if (z < 0f) {
                    z += dz;
                    if (z > 0f) {
                        z = 0f;
                    }
                }
            }

            return new Vector3(x, y, z);
        }

        public static Vector3 DecayToZero(this Vector3 v, Vector3 decayVect) {
            var x = v.x;
            var y = v.y;
            var z = v.z;

            if (x > 0f) {
                x += decayVect.x;
                if (x < 0f) {
                    x = 0f;
                }
            }
            else if (x < 0f) {
                x += decayVect.x;
                if (x > 0f) {
                    x = 0f;
                }
            }

            if (y > 0f) {
                y += decayVect.y;
                if (y < 0f) {
                    y = 0f;
                }
            }
            else if (y < 0f) {
                y += decayVect.y;
                if (y > 0f) {
                    y = 0f;
                }
            }

            if (z > 0f) {
                z += decayVect.z;
                if (z < 0f) {
                    z = 0f;
                }
            }
            else if (z < 0f) {
                z += decayVect.z;
                if (z > 0f) {
                    z = 0f;
                }
            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Clamps a vector to +/- VALUE if the specified max value for a given axis is non-zero.
        /// </summary>
        public static Vector3 ClampByAxis(this Vector3 v, float maxX, float maxY, float maxZ) {
            if (!maxX.IsAlmostEqualTo(0f)) {
                if (v.x < -maxX) {
                    v.x = -maxX;
                }
                else if (v.x > maxX) {
                    v.x = maxX;
                }
            }

            if (!maxY.IsAlmostEqualTo(0f)) {
                if (v.y < -maxY) {
                    v.y = -maxY;
                }
                else if (v.x > maxY) {
                    v.y = maxY;
                }
            }

            if (!maxZ.IsAlmostEqualTo(0f)) {
                if (v.z < -maxZ) {
                    v.z = -maxZ;
                }
                else if (v.z > maxZ) {
                    v.z = maxZ;
                }
            }

            return v;
        }
    }
}
