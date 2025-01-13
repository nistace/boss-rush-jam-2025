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

        public Collider NearestPowerUp { get; private set; }

        public UnityEvent OnDetectedPowerUpsChanged { get; } = new();
        public UnityEvent OnPowerUpCollected { get; } = new();

        private void CollectNearPowerUps()
        {
            for(int powerUpIndex = detectedPowerUps.Count - 1; powerUpIndex > -1; powerUpIndex--)
            {
                Collider powerUp = detectedPowerUps[powerUpIndex];
                float sqrDistance = (powerUp.transform.position - transform.position).sqrMagnitude;

                if(sqrDistance - collectionRadius < 0.01f)
                {
                    OnPowerUpCollected.Invoke();
                    detectedPowerUps.Remove(powerUp);
                    Destroy(powerUp.gameObject);

                    continue;
                }
            }
        }

        private void CheckNearestPowerUp()
        {
            float nearestPowerUpDistance = float.MaxValue;

            for (int powerUpIndex = detectedPowerUps.Count - 1; powerUpIndex > -1; powerUpIndex--)
            {
                Collider powerUp = detectedPowerUps[powerUpIndex];
                float sqrDistance = (powerUp.transform.position - transform.position).sqrMagnitude;

                if (sqrDistance < nearestPowerUpDistance)
                {
                    nearestPowerUpDistance = sqrDistance;
                    NearestPowerUp = powerUp;
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.Add(collider);
                CheckNearestPowerUp();
                OnDetectedPowerUpsChanged.Invoke();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.Remove(collider);
                CheckNearestPowerUp();
                OnDetectedPowerUpsChanged.Invoke();
            }
        }

        private void FixedUpdate()
        {
            CollectNearPowerUps();
            CheckNearestPowerUp();
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
