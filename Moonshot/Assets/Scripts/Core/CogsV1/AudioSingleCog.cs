using UnityEngine;

namespace Core.CogsV1 {
    public class AudioSingleCog : CogAdapter {
        public AudioClip Sound;

        public override void Activate(object source, CogData data) {
            AudioSource.PlayClipAtPoint(this.Sound, this.transform.position);
        }
    }
}
