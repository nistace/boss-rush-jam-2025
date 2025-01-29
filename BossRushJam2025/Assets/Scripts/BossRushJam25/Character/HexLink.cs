using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character {
   public class HexLink : MonoBehaviour {
      [SerializeField] protected NavMeshAgent agent;
      [SerializeField] protected Transform hexHighlight;
      [SerializeField] protected bool lockHex;

      public GridHex LinkedHex { get; private set; }

      private void Reset() {
         agent = GetComponentInParent<NavMeshAgent>();
      }

      private void Update() {
         if (LinkedHex) {
            LinkedHex.SetLockedInPlaceBy(this, false);
         }

         if (HexGridController.Instance.TryGetHex(HexGridController.Instance.WorldToCoordinates(transform.position), out var hex)) {
            LinkedHex = hex;
            LinkedHex.SetLockedInPlaceBy(this, lockHex);

            if (agent.enabled) {
               agent.obstacleAvoidanceType = LinkedHex.IsMoving ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
               agent.isStopped = LinkedHex.IsMoving;
            }

            LinkedHex.ParentTransformToHexContent(transform, false, false);
            if (hexHighlight) {
               LinkedHex.ParentTransformToHexContent(hexHighlight, true, true);
               hexHighlight.gameObject.SetActive(true);
            }
         }
         else {
            LinkedHex = null;
            transform.SetParent(null);
            hexHighlight.gameObject.SetActive(false);
         }
      }
   }
}