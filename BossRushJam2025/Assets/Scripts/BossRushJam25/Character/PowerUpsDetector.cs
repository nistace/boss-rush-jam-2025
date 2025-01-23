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

        public UnityEvent OnNearestPowerUpChanged { get; } = new();
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
            Collider newNearestPowerUp = NearestPowerUp;

            foreach(Collider powerUp in detectedPowerUps)
            {
                float sqrDistance = (powerUp.transform.position - transform.position).sqrMagnitude;

                if (sqrDistance < nearestPowerUpDistance)
                {
                    nearestPowerUpDistance = sqrDistance;
                    newNearestPowerUp = powerUp;
                }
            }

            if (newNearestPowerUp != NearestPowerUp)
            {
                NearestPowerUp = newNearestPowerUp;
                OnNearestPowerUpChanged.Invoke();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.Add(collider);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.Remove(collider);
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
