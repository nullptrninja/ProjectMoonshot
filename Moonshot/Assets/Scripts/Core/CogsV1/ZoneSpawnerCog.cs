using Core.Pooling;
using Core.Utility;
using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// Enables spawning of a fixed object name within the specified Collider zone. This component also
    /// supports ObjectPooling. If an ObjectPool is specified, then the ResourceName is not needed as the
    /// spawning object type is inferred from the linked pool. The object being spawned from the pool must
    /// also support pooling (e.g.: must release itself back into it's own ObjectPoolLink).
    /// 
    /// As a COG, activating this will either spawn all objects (if enabled and if we haven't spawned them all
    /// yet), or it will start the spawn timer. Deactivation will stop the spawn timer.
    /// </summary>
    public class ZoneSpawnerCog : CogAdapter {
        public bool ActiveOnStart;
        public bool AllowReactivation;
        public Collider SpawningZone;
        public ObjectPool SourceObjectPool;
        public string ResourceName;             // What we're spawning. Not needed if we're using a pool
        public float SpawnFrequency;            // Defined as 1 object every X seconds
        public bool SpawnAllObjectsAtOnce;      // Make all objects at once (uses MaxObjectsToSpawn, don't use -1)
        public int MaxObjectsToSpawn = -1;      // -1 is unlimited
        public bool AttachObjectsToZoneParent;

        private FrequencyTimer mSpawnTimer;
        private Object mSpawnTemplate;
        private int mObjectsSpawnedSoFar;

        public void StartSpawning() {
            if (mSpawnTimer != null) {
                mSpawnTimer.Start(this.SpawnFrequency);
            }
        }

        public void Start() {
            mObjectsSpawnedSoFar = 0;
            
            if (this.SourceObjectPool == null && !string.IsNullOrEmpty(this.ResourceName)) {
                ChangeTemplate(this.ResourceName);
            }

            if (!this.SpawnAllObjectsAtOnce) {
                mSpawnTimer = new FrequencyTimer(Common.TimeScale.Scaled);
                mSpawnTimer.Tick += SpawnObject;
            }

            if (this.ActiveOnStart) {
                if (!this.SpawnAllObjectsAtOnce) {                    
                    mSpawnTimer.Start(this.SpawnFrequency);
                }
                else {
                    SpawnAllObjects();
                }
            }
        }
        
        public void Update() {
            if (!this.SpawnAllObjectsAtOnce) {
                mSpawnTimer.Update();
            }
        }

        public override void Activate(object source, CogData data) {
            if (this.SpawnAllObjectsAtOnce) {
                SpawnAllObjects();
            }
            else {
                mSpawnTimer.Start(this.SpawnFrequency);
            }
        }

        public override void Deactivate(object source, CogData data) {
            if (mSpawnTimer != null) {
                mSpawnTimer.Stop();
            }
        }

        public void ChangeTemplate(string resourceName) {
            if (!resourceName.Equals(this.ResourceName) && !string.IsNullOrEmpty(resourceName)) {
                this.ResourceName = resourceName;
                mSpawnTemplate = Resources.Load(resourceName);
            }
        }

        private void SpawnAllObjects() {
            if (mObjectsSpawnedSoFar != this.MaxObjectsToSpawn) {
                for (int i = 0; i < this.MaxObjectsToSpawn; i++) {
                    SpawnObject();
                }

                mObjectsSpawnedSoFar = this.AllowReactivation ? 0 : this.MaxObjectsToSpawn;
            }
        }

        private void SpawnObject() {
            GameObject o = null;
            if (this.SourceObjectPool != null) {
                o = this.SourceObjectPool.GetObject();
            }
            else if (mSpawnTemplate != null) {
                o = GameObject.Instantiate(mSpawnTemplate) as GameObject;
            }

            // Do null check in the event we couldn't spawn obj from the pool
            if (o != null) {
                o.transform.position = GetPointInZone(this.SpawningZone, o.transform.position.z);

                if (this.AttachObjectsToZoneParent) {
                    o.transform.parent = this.SpawningZone.gameObject.transform;
                }

                // Test for stop condition
                if (!this.SpawnAllObjectsAtOnce && this.MaxObjectsToSpawn >= 0 && ++mObjectsSpawnedSoFar >= this.MaxObjectsToSpawn) {
                    mSpawnTimer.Stop();

                    if (this.AllowReactivation) {
                        mObjectsSpawnedSoFar = 0;
                    }
                }
            }
        }

        private static Vector3 GetPointInZone(Collider zone, float zOffset) {
            float x = Random.Range(zone.bounds.min.x, zone.bounds.max.x);
            float y = Random.Range(zone.bounds.min.y, zone.bounds.max.y);            

            return new Vector3(x, y, zOffset);
        }
    }
}
