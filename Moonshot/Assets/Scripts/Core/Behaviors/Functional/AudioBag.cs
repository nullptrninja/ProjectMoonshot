using UnityEngine;

namespace Core.Behaviors.Functional {
    /// <summary>
    /// Allows a GameObject to store and play a random sound out of a list of available choices
    /// </summary>
    public class AudioBag : MonoBehaviour {
        public bool PlayAtStart;
        public AudioClip[] AudioClips;

        public void Start() {
            if (this.PlayAtStart) {
                Play();
            }
        }

        public void Play() {
            AudioClip audio = this.AudioClips[Random.Range(0, this.AudioClips.Length)];
            if (audio != null) {
                AudioSource.PlayClipAtPoint(audio, this.transform.position);
            }
        }
    }
}