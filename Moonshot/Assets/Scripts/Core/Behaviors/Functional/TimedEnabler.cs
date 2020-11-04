using UnityEngine;

namespace Core.Behaviors.Functional
{
    /// <summary>
    /// Enables a linked GameObject after a specified period of time
    /// </summary>
    public class TimedEnabler : MonoBehaviour
    {
        public GameObject TargetObject;
        public float DelaySeconds;
        public bool DestroySelfAfterwards;

        private float mElapsedTime;

        public void Update()
        {
            mElapsedTime += Time.deltaTime;
            if (mElapsedTime >= this.DelaySeconds)
            {
                this.TargetObject.SetActive(true);

                if (this.DestroySelfAfterwards)
                {
                    GameObject.Destroy(this.gameObject);
                }
            }
        }
    }
}
