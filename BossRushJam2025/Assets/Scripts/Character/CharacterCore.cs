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

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public DebugDestinationAssigner DestinationAssigner => destinationAssigner;

        private void Awake()
        {
            hexLink.Initialize(this);
            actionPriorityHandler.Initialize(this);
            destinationAssigner.Initialize(this);
        }
    }
}
