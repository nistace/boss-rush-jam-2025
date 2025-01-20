using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character {
   public class HexLink : MonoBehaviour {
      [SerializeField] protected NavMeshAgent agent;
      [SerializeField] protected Transform hexHighlight;

      private void Reset() {
         agent = GetComponentInParent<NavMeshAgent>();
      }

      private void Update() {
         if (HexGridController.Instance.TryGetHex(HexGridController.Instance.WorldToCoordinates(transform.position), out var hex)) {
            agent.obstacleAvoidanceType = hex.IsMoving ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.isStopped = hex.IsMoving;
            hex.ParentTransformToHexContent(transform, false, false);
            if (hexHighlight) {
               hex.ParentTransformToHexContent(hexHighlight, true, true);
               hexHighlight.gameObject.SetActive(true);
            }
         }
         else {
            transform.SetParent(null);
            hexHighlight.gameObject.SetActive(false);
         }
      }
   }
}
