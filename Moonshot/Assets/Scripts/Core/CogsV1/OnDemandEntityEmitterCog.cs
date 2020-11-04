using Core.Common;
using Core.Pooling;
using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// On trigger, this will launch an entity with data taken from the CogData passed in.
    /// The CogData needs to contain:
    /// - ObjectData -> reference to an EmitterData object that describes the projectile in more detail
    /// This COG supports Object Pooling.
    /// </summary>
    public class OnDemandEntityEmitterCog : CogAdapter {
        public ObjectPool ObjectPool;

        public void Awake() {
            ObjectPoolLink poolLink = GetComponent<ObjectPoolLink>();
            if (poolLink != null && poolLink.LinkedObjectPool != null) {
                this.ObjectPool = poolLink.LinkedObjectPool;
            }
        }

        public override void Activate(object source, CogData data) {
            if (data != null && data.ObjectData != null) {
                // VectorData will always be available even if it's a (0, 0) value
                EmitterData ed = data.ObjectData as EmitterData;
                GameObject projectile = CreateInstance(ed.TemplateObject);
                Rigidbody2D phys = projectile.GetComponent<Rigidbody2D>();

                projectile.transform.position = ed.Position;
                phys.AddForce(ed.Magnitude, ForceMode2D.Impulse);
                // TODO: Need to attach the context to the projectile somehow
            }

            base.Activate(source, data);
        }

        private GameObject CreateInstance(GameObject templateObject) {
            GameObject g = null;
            if (this.ObjectPool != null && this.ObjectPool.CanAcquireObject) {
                // NOTE: We make a very big assumption here that the specified pool contains
                // instances of the correct game object that we want. Don't fuck this up.
                g = this.ObjectPool.GetObject();
            }
            else {
                g = GameObject.Instantiate(templateObject);
            }

            return g;
        }
    }
}
