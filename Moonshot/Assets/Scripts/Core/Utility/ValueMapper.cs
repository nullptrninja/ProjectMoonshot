using System;
namespace Core.Utility {
    /// <summary>
    /// Maps a range of input values to another range of output (mapped) values
    /// </summary>
    public class ValueMapper {

        /// <summary>
        /// The minimum (inclusive) unmapped input value
        /// </summary>
        public float MinValue {
            get;
            set;
        }

        /// <summary>
        /// The maximum (inclusive) unmapped input value
        /// </summary>
        public float MaxValue {
            get;
            set;
        }

        /// <summary>
        /// The minimum (inclusive) mapped output value
        /// </summary>
        public float MinMappedValue {
            get;
            set;
        }

        /// <summary>
        /// The maximum (inclusive) mapped output value
        /// </summary>
        public float MaxMappedValue {
            get;
            set;
        }

        public ValueMapper(float minOriginal, float maxOriginal, float minMapped, float maxMapped) {
            this.MinValue = minOriginal;
            this.MaxValue = maxOriginal;
            this.MinMappedValue = minMapped;
            this.MaxMappedValue = maxMapped;

            if (this.MaxValue == this.MinValue) {
                throw new ArgumentOutOfRangeException("Max value and Min value cannot be the same");
            }
        }

        /// <summary>
        /// Gets the mapped output value for the given input value. If the output range is exceeded, the
        /// min or max values will be returned instead.
        /// </summary>
        /// <param name="inValue">The input value</param>
        /// <returns>Mapped output value</returns>
        public float GetMappedValue(float inValue) {
            float position = GetUnmappedPosition(inValue);
            if (position > 1f) {
                return this.MaxMappedValue;
            }
            else if (position < 0f) {
                return this.MinMappedValue;
            }

            return ((this.MaxMappedValue - this.MinMappedValue) * position) + this.MinMappedValue;
        }

        private float GetUnmappedPosition(float inValue) {
            if (this.MaxValue - this.MinValue == 0) {
                return inValue;
            }
            return (inValue - this.MinValue) / (this.MaxValue - this.MinValue);
        }
    }
}
