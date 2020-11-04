using Core.Behaviors.Functional;
using System;
using UnityEngine;

namespace Core.Audio {
    [Serializable]
    public class SoundSemaphore {
        public int Id;
        public AudioClip SingleSound;
        public AudioBag Sounds;
        public int MaxSimultaneousPlays;
        public float CooldownSeconds;

        // State tracking during runtime
        public int PlaysRemaining { get; set; }
        public float ElapsedTime { get; set; }
        public bool IsActive { get; set; }
    }
}
