using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Spacecraft {
    public class FixedThrustRocketMotor : BaseRocketMotor {
        public void Update() {
            this.CurrentThrottle = 1f;
            this.DoRocketFixedUpdate();
        }
    }
}
