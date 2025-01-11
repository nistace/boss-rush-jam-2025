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
            if (HexGridController.Instance.TryGetHex(HexGridController.Instance.WorldToCoordinates(transform.position), out var hex))
            {
                character.NavMeshAgent.obstacleAvoidanceType = hex.IsMoving ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            }
        }
    }
}
