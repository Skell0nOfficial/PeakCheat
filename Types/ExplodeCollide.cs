using PeakCheat.Utilities;
using UnityEngine;

namespace PeakCheat.Types
{
    internal class ExplodeCollide : MonoBehaviour
    {
        private float _spawnTime = 0f, _spawnDelay = 0f;
        public void Init(float delay)
        {
            _spawnTime = Time.time;
            _spawnDelay = delay;
        }
        public void OnCollisionEnter(Collision collision)
        {
            if (Time.time - _spawnTime < _spawnDelay) return;
            if (!TimeUtil.CheckTime(.2f)) return;
            GameObject.Destroy(this);
            PrefabUtil.SummonExplosion(transform.position);
        }
    }
}