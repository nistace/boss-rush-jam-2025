using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character {
   public class HexLink : MonoBehaviour {
      [SerializeField] protected NavMeshAgent agent;
      [SerializeField] protected Transform hexHighlight;

      public GridHex LinkedHex { get; private set; }

      private void Reset() {
         agent = GetComponentInParent<NavMeshAgent>();
      }

      private void Update() {
         if (HexGridController.Instance.TryGetHex(HexGridController.Instance.WorldToCoordinates(transform.position), out var hex)) {
            LinkedHex = hex;
            agent.obstacleAvoidanceType = LinkedHex.IsMoving ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.isStopped = LinkedHex.IsMoving;
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
