using BossRushJam25.Character.AI;
using UnityEngine;

namespace BossRushJam25.Character
{
    public class DebugDestinationAssigner : MonoBehaviour
    {
        protected CharacterCore character;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(1))
            {
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    new MoveAction(character, hit.point).Assign();
                }
            }
        }

        //:TODO: draw all queued destinations (maybe in MoveAction instead?)
        private void OnDrawGizmos()
        {
            if(character.NavMeshAgent.hasPath)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(character.NavMeshAgent.destination, 0.3f);
            }
        }
    }
}
