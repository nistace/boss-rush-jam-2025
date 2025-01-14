using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character
{
    public class HexLink : MonoBehaviour
    {
        protected CharacterCore character;

        public GridHex LinkedHex { get; private set; }

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void Update()
        {
            if(HexGridController.Instance.TryGetHex(HexGridController.Instance.WorldToCoordinates(transform.position), out var hex))
            {
                LinkedHex = hex;
                character.NavMeshAgent.obstacleAvoidanceType = LinkedHex.IsMoving ? ObstacleAvoidanceType.NoObstacleAvoidance : ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                character.NavMeshAgent.isStopped = LinkedHex.IsMoving;
                character.transform.SetParent(LinkedHex.transform);
            }
        }
    }
}
