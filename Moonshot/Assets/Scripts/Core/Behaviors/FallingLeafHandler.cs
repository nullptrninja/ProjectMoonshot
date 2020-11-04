using Core.Utility;
using UnityEngine;

namespace Core.Behaviors {
    public class FallingLeafHandler : MonoBehaviour {
        private const int VariancePacing = 10;

        public Rigidbody Body;
        public float WindForce = 1f;
        public float WindVariancePercentage = 0.5f;
        public float CycleTime = 3f;
        public float MovementDuration = 10f;
        public bool RandomStartCyclePosition;

        private TrigWaveGenerator mWaveFunction;
        private int mVariancePace;
        private float mElapsedTime;

        public void Awake() {            
            this.mWaveFunction = new TrigWaveGenerator(Common.TimeScale.Scaled, true);

            var startTimePosition = this.RandomStartCyclePosition ? Random.Range(0f, 0.99f) : 0f;
            this.mWaveFunction.Start(TrigWaveGenerator.WaveType.Sine, this.CycleTime, 0f, 360f, startTimePosition);
        }

        public void Update() {
            this.mElapsedTime += Time.deltaTime;
            
            mWaveFunction.Update();
            var windForceRange = this.WindForce;

            if (mVariancePace++ >= VariancePacing) {
                mVariancePace = 0;
                var variance = this.WindForce * this.WindVariancePercentage;
                windForceRange += Random.Range(-variance, variance);
            }

            var appliedForce = mWaveFunction.CurrentValue * Mathf.Lerp(windForceRange, 0f, Mathf.Min((mElapsedTime / this.MovementDuration), 1f));
            
            this.Body.AddForce(appliedForce, 0f, 0f, ForceMode.Force);
        }
    }
}
