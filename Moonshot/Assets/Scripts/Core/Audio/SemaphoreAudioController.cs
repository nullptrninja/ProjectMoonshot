using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio {
    /// <summary>
    /// SemaphoreAudioController allows for centralized control of non-spatial audio playback with the
    /// benefit of pacing and simultaneous playback management for each individual asset. This allows us
    /// to avoid tidal waves of garbage sounds by limiting the number of times an asset can be played
    /// back using timed spacing and max-simultaneous-play counters.
    /// This controller also supports Core library's AudioBag to allow for an asset to be randomized while
    /// still sharing the same asset ID.
    /// 
    /// Usage:
    /// - Sound asset identification is done via an ID which is specific to your game implementation
    /// - As a suggestion, you may either store a library of sound IDs or write a repository with those
    ///   IDs baked in, which will call into this controller.
    /// </summary>
    public class SemaphoreAudioController : MonoBehaviour {
        public Transform AudioOriginPoint;
        public SoundSemaphore[] SoundsList;

        private Dictionary<int, SoundSemaphore> mIdToSoundLookUp;
        private List<SoundSemaphore> mActiveSounds;

        public void Start() {
            Initialize();
        }

        public void PlaySound(int id) {
            if (mIdToSoundLookUp.TryGetValue(id, out var soundEntry)) {
                PlaySound(soundEntry);
            }
        }

        public void Update() {
            if (mActiveSounds.Count > 0) {
                for (var i = 0; i < mActiveSounds.Count; i++) {
                    var entry = mActiveSounds[i];
                    if (entry.IsActive) {
                        UpdateSoundEntry(entry);
                    }
                    
                    // This double-check-remove allows us to immediately remove an entry as soon as it becomes
                    // inactive. This will also prevent double-entries of the same sound in the active list.
                    if (!entry.IsActive) { 
                        // Remove and decrement to compensate
                        mActiveSounds.RemoveAt(i--);
                    }
                }
            }
        }

        private void Initialize() {
            mIdToSoundLookUp = new Dictionary<int, SoundSemaphore>();
            for (var i = 0; i < this.SoundsList.Length; i++) {
                var ss = this.SoundsList[i];
                ss.PlaysRemaining = ss.MaxSimultaneousPlays;
                ss.ElapsedTime = 0f;
                ss.IsActive = false;

                mIdToSoundLookUp.Add(ss.Id, ss);
            }

            // No need to prime the list with full capacity, we take a heuristic approach here
            mActiveSounds = new List<SoundSemaphore>(Math.Max(1, this.SoundsList.Length / 2));
        }

        private void PlaySound(SoundSemaphore soundEntry) {
            // Check if we have any plays available
            if (soundEntry.PlaysRemaining > 0) {
                soundEntry.PlaysRemaining--;

                if (soundEntry.SingleSound != null) {
                    AudioSource.PlayClipAtPoint(soundEntry.SingleSound, this.AudioOriginPoint.position);
                }
                else {
                    soundEntry.Sounds.Play();
                }

                if (!soundEntry.IsActive) {
                    soundEntry.IsActive = true;
                    mActiveSounds.Add(soundEntry);
                }
            }
        }

        private void UpdateSoundEntry(SoundSemaphore soundEntry) {
            if (soundEntry.PlaysRemaining < soundEntry.MaxSimultaneousPlays) {
                soundEntry.ElapsedTime += Time.unscaledDeltaTime;
                if (soundEntry.ElapsedTime >= soundEntry.CooldownSeconds) {
                    // Note: there can be a slight drift in cooldown since we're not carrying over the remainder.
                    soundEntry.ElapsedTime = 0f;
                    soundEntry.PlaysRemaining = Math.Min(soundEntry.PlaysRemaining + 1, soundEntry.MaxSimultaneousPlays);
                }
            }
            else {
                // This will flag the update loop to remove it from the active list
                soundEntry.IsActive = false;
            }
        }
    }
}
