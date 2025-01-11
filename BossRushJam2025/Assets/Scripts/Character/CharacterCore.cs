using BossRushJam25.Character.AI;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character
{
    public class CharacterCore : MonoBehaviour
    {
        [SerializeField] protected NavMeshAgent navMeshAgent;
        [SerializeField] protected HexLink hexLink;
        [SerializeField] protected ActionPriorityHandler actionPriorityHandler;
        [SerializeField] protected DebugDestinationAssigner destinationAssigner;
        [SerializeField] protected DebugDodgeSimulation dodgeSimulation;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public DebugDestinationAssigner DestinationAssigner => destinationAssigner;
        public DebugDodgeSimulation DodgeSimulation => dodgeSimulation;

        private void Awake()
        {
            hexLink.Initialize(this);
            actionPriorityHandler.Initialize(this);
            destinationAssigner.Initialize(this);
            dodgeSimulation.Initialize(this);
        }
    }
}
