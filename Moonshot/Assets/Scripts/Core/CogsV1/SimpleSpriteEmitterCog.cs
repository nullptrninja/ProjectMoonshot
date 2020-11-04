using Core.Behaviors;

namespace Core.CogsV1 {
    /// <summary>
    /// COMPANION COG. Works in conjunction with the SimpleSpriteEmitter. Activating this cog will activate the emitter
    /// on demand (the emitter should be disabled at the start).
    /// </summary>
    public class SimpleSpriteEmitterCog : CogAdapter {
        public SimpleSpriteEmitter LinkedSimpleSpriteEmitter;

        public override void Activate(object source, CogData data) {
            // Toggle behavior            
            if (this.LinkedSimpleSpriteEmitter != null) {
                if (this.LinkedSimpleSpriteEmitter.Started) {
                    this.LinkedSimpleSpriteEmitter.Stop();
                }
                else {
                    this.LinkedSimpleSpriteEmitter.Begin();
                }
            }

            base.Activate(source, data);
        }

        public override void Deactivate(object source, CogData data) {
            SimpleSpriteEmitter comp = this.gameObject.GetComponent<SimpleSpriteEmitter>();
            if (comp != null) {
                comp.Stop();
            }

            base.Deactivate(source, data);
        }
    }
}
