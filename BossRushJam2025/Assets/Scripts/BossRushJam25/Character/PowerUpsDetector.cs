using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character
{
    public class PowerUpsDetector : MonoBehaviour
    {
        [SerializeField] protected float detectionRadius = 10f;
        [SerializeField] protected float collectionRadius = 1f;
        [SerializeField] protected SphereCollider detectionCollider;

        protected List<Collider> detectedPowerUps = new();

        public UnityEvent OnPowerUpsDetectedChanged { get; } = new();
        public UnityEvent OnPowerUpCollected { get; } = new();

        private void CollectNearPowerUps()
        {
            for(int powerUpIndex = detectedPowerUps.Count - 1; powerUpIndex > -1; powerUpIndex--)
            {
                Collider powerUp = detectedPowerUps[powerUpIndex];

                if((powerUp.transform.position - transform.position).sqrMagnitude - collectionRadius < 0.01f)
                {
                    OnPowerUpCollected.Invoke();
                    detectedPowerUps.Remove(powerUp);
                    Destroy(powerUp.gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.Add(collider);
                OnPowerUpsDetectedChanged.Invoke();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.Remove(collider);
                OnPowerUpsDetectedChanged.Invoke();
            }
        }

        private void FixedUpdate()
        {
            CollectNearPowerUps();
        }

        private void OnValidate()
        {
            if(detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius;
            }
        }
    }
}
