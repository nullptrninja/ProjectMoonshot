using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.Utility {
    /// <summary>
    /// A quick and dirty cooldown timer
    /// </summary>
    public class CooldownTimer {
        public float CooldownTime { get; set; }
        public bool CooledDown {
            get { return mElapsedTime >= this.CooldownTime; }
        }

        private float mElapsedTime;
        private bool mEnabled;

        public CooldownTimer(float cooldownTime) {
            this.CooldownTime = cooldownTime;
            mElapsedTime = cooldownTime;
            mEnabled = false;
        }

        public void BeginCooldown() {
            mElapsedTime = 0f;
            mEnabled = true;
        }

        public void Update() {
            if (mEnabled) {
                mElapsedTime += Time.deltaTime;

                if (this.CooledDown) {
                    mEnabled = false;
                }
            }
        }
    }
}
