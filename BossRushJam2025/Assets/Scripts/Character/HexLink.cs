using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character
{
    public class HexLink : MonoBehaviour
    {
        protected CharacterCore character;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }
        private void Update()
        {
            //could be found with hero coordinates instead
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10)
                && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
                )
            {
                GridHex hex = hit.collider.GetComponentInParent<GridHex>();
                character.NavMeshAgent.obstacleAvoidanceType = hex.IsMoving ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }
        }
    }
}
