using System.Linq;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.ControlHex {
   public class HexContentDetector : MonoBehaviour {
      [SerializeField] protected float detectionRadius = 10f;
      [SerializeField] protected LayerMask detectionLayerMask = ~0;
      [SerializeField] protected LayerMask powerUpLayerMask = ~0;

      private readonly Collider[] results = new Collider[50];

      public float DetectionRadius => detectionRadius;

      public bool TryGetNearestDamageableHex(Vector3 fromPosition, DamageTypes damageTypes, out GridHex nearestHex) {
         var colliderCountInRange = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, detectionLayerMask, QueryTriggerInteraction.Collide);
         return nearestHex = results.Take(colliderCountInRange)
            .Select(t => t.gameObject.GetComponentInParent<GridHex>())
            .Where(t => t && t.ContentsAreDamageable(damageTypes))
            .OrderBy(t => (t.transform.position - fromPosition).sqrMagnitude)
            .FirstOrDefault();
      }

      public bool TryGetNearestPowerUp(Vector3 fromPosition, out PowerUp nearestPowerUp) {
         var colliderCountInRange = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, powerUpLayerMask, QueryTriggerInteraction.Collide);
         return nearestPowerUp = results.Take(colliderCountInRange)
            .Select(t => t.gameObject.GetComponentInParent<PowerUp>())
            .Where(t => t)
            .OrderBy(t => (t.transform.position - fromPosition).sqrMagnitude)
            .FirstOrDefault();
      }

      private void OnDrawGizmosSelected() {
         Gizmos.color = Color.red;
         Gizmos.DrawWireSphere(transform.position, detectionRadius);
      }
   }
}