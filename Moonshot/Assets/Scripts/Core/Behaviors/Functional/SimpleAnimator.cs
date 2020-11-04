using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Core.Behaviors.Functional {
    /// <summary>
    /// Plays animations without the use of Mechanim.
    /// Adapted from http://pastebin.com/rW6RV732
    /// </summary>
    public class SimpleAnimator : MonoBehaviour {
        [System.Serializable]
        public class AnimationTrigger {
            public int Frame;
            public string Name;
        }

        [System.Serializable]
        public class Animation {
            public string Name;
            public int ID;
            public int FPS;
            public Sprite[] Frames;
            public int LoopCount;       // -1 for infinite
            public bool BeginAtRandomFrame;

            [NonSerialized]
            public int CurrentLoopIteration;

            public AnimationTrigger[] Triggers;
        }

        public SpriteRenderer Renderer;
        public Animation[] Animations;

        public bool Playing { get; private set; }
        public Animation CurrentAnimation { get; private set; }
        public int CurrentFrame { get; private set; }
        
        public string PlayAnimationOnStart;
        
        public bool FinishedPlaying {
            get { return IsFinished(); }
        }

        private void Awake() {
            if (!this.Renderer) {
                this.Renderer = GetComponent<SpriteRenderer>();
            }
        }

        private void OnEnable() {
            if (this.PlayAnimationOnStart != string.Empty) {
                Play(this.PlayAnimationOnStart, true, 0);
            }
        }

        private void OnDisable() {
            this.Playing = false;
            this.CurrentAnimation = null;
        }

        private bool IsFinished() {
            if (this.CurrentAnimation != null) {
                return this.CurrentAnimation.LoopCount != -1 &&
                       this.CurrentAnimation.CurrentLoopIteration == 0 &&
                       this.CurrentAnimation.Frames.Length - 1 == this.CurrentFrame;
            }
            else {
                return true;
            }
        }

        public void Play(string name, bool allowLoop, int startFrame) {
            Animation animation = GetAnimation(name);
            if (animation != null) {
                if (animation != this.CurrentAnimation) {
                    ForcePlay(name, allowLoop, startFrame);
                }
            }
            else {
                Debug.LogWarning("Could not find animation: " + name);
            }
        }

        public void Play(Animation animation, bool allowLoop, int startFrame) {
            if (animation != this.CurrentAnimation) {
                ForcePlay(animation, allowLoop, startFrame);
            }
        }

        public void ForcePlay(string name, bool loop, int startFrame) {
            Animation animation = GetAnimation(name);
            if (animation != null) {                
                this.CurrentAnimation = animation;
                this.CurrentAnimation.CurrentLoopIteration = loop ? this.CurrentAnimation.LoopCount : 0;
                this.Playing = true;
                this.CurrentFrame = startFrame;
                this.Renderer.sprite = animation.Frames[this.CurrentFrame];
                StopAllCoroutines();
                StartCoroutine(PlayAnimation(this.CurrentAnimation));
            }
        }

        public void ForcePlay(Animation animation, bool loop, int startFrame) {                        
            this.CurrentAnimation = animation;
            this.CurrentAnimation.CurrentLoopIteration = loop ? this.CurrentAnimation.LoopCount : 0;
            this.Playing = true;
            this.CurrentFrame = startFrame;
            this.Renderer.sprite = animation.Frames[this.CurrentFrame];
            StopAllCoroutines();
            StartCoroutine(PlayAnimation(this.CurrentAnimation));
        }

        public void SlipPlay(string name, int wantFrame, params string[] otherNames) {
            for (int i = 0; i < otherNames.Length; i++) {
                if (this.CurrentAnimation != null && this.CurrentAnimation.Name == otherNames[i]) {
                    Play(name, true, this.CurrentFrame);
                    break;
                }
            }
            Play(name, true, wantFrame);
        }

        public bool IsPlaying(string name) {
            return (this.CurrentAnimation != null && this.CurrentAnimation.Name == name);
        }

        public Animation GetAnimation(string name) {
            return this.Animations.DefaultIfEmpty(null).Single(o => o.Name.Equals(name));
        }

        public Animation GetAnimation(int id) {
            return this.Animations.DefaultIfEmpty(null).Single(o => o.ID == id);
        }

        private IEnumerator PlayAnimation(Animation animation) {
            float timer = 0f;

            if (animation.BeginAtRandomFrame) {
                this.CurrentFrame = UnityEngine.Random.Range(0, animation.Frames.Length);
            }

            if (animation.FPS != 0)
            {
                float delay = 1f / animation.FPS;

                while (animation.LoopCount > 0 || animation.LoopCount == -1 || this.CurrentFrame < animation.Frames.Length - 1)
                {
                    while (timer < delay)
                    {
                        timer += Time.deltaTime;
                        yield return 0f;
                    }
                    while (timer > delay)
                    {
                        timer -= delay;
                        NextFrame(animation);
                    }

                    this.Renderer.sprite = animation.Frames[this.CurrentFrame];
                }
            }
            this.CurrentAnimation = null;
        }

        private void NextFrame(Animation animation) {
            if (this.CurrentAnimation == null) {
                return;
            }

            this.CurrentFrame++;

            foreach (AnimationTrigger animationTrigger in this.CurrentAnimation.Triggers)
            {
                if (animationTrigger.Frame == CurrentFrame)
                {
                    gameObject.SendMessageUpwards(animationTrigger.Name);
                }
            }
            
            if (this.CurrentFrame >= animation.Frames.Length) {
                if (this.CurrentAnimation.LoopCount == -1 || this.CurrentAnimation.CurrentLoopIteration > 0) {
                    this.CurrentFrame = 0;
                    if (this.CurrentAnimation.CurrentLoopIteration > 0) {
                        this.CurrentAnimation.CurrentLoopIteration--;
                    }
                }
                else {
                    this.CurrentFrame = animation.Frames.Length - 1;
                }                    
            }
        }

        public int GetFacing() {
            return (int)Mathf.Sign(Renderer.transform.localScale.x);
        }

        public void FlipTo(float dir) {
            if (dir < 0f) {
                Renderer.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else {
                Renderer.transform.localScale = new Vector3(1f, 1f, 1f);
            }   
        }

        public void FlipTo(Vector3 position) {
            float diff = position.x - transform.position.x;
            if (diff < 0f) {
                this.Renderer.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else {
                this.Renderer.transform.localScale = new Vector3(1f, 1f, 1f);
            }   
        }

    }
}