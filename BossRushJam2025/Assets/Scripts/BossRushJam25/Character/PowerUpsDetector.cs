using System.Collections.Generic;
using System.Linq;
using BossRushJam25.PowerUps;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character
{
    public class PowerUpsDetector : MonoBehaviour
    {
        [SerializeField] protected float detectionRadius = 10f;
        [SerializeField] protected float collectionRadius = 1f;
        [SerializeField] protected SphereCollider detectionCollider;

        protected CharacterCore character;
        protected HashSet<PowerUp> detectedPowerUps = new();

        public PowerUp NearestPowerUp { get; private set; }

        public UnityEvent OnNearestPowerUpChanged { get; } = new();
        public UnityEvent OnPowerUpCollected { get; } = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }
        private void CollectNearPowerUps() {
            PowerUp powerUpToCollect = detectedPowerUps.FirstOrDefault(t => (t.transform.position - transform.position).sqrMagnitude - collectionRadius < .01f);
            if(powerUpToCollect)
            {
                OnPowerUpCollected.Invoke();
                powerUpToCollect.Collect(character);
                detectedPowerUps.Remove(powerUpToCollect);
            }
        }

        private void CheckNearestPowerUp()
        {
            float nearestPowerUpDistance = float.MaxValue;
            PowerUp newNearestPowerUp = null;

            foreach(PowerUp powerUp in detectedPowerUps)
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
                var powerUp = collider.gameObject.GetComponentInParent<PowerUp>();
                if (powerUp) {
                    detectedPowerUps.Add(powerUp);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
            {
                detectedPowerUps.RemoveWhere(t => collider.transform.IsChildOf(t.transform));
            }
        }

        private void Update()
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
