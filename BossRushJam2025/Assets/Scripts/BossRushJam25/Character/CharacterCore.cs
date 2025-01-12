using BossRushJam25.Character.AI;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character
{
    public class CharacterCore : MonoBehaviour
    {
        [SerializeField] protected CharacterType type;
        [SerializeField] protected NavMeshAgent navMeshAgent;
        [SerializeField] protected HexLink hexLink;
        [SerializeField] protected ActionPriorityHandler actionPriorityHandler;
        [SerializeField] protected PowerUpsCollector powerUpsCollector;
        [SerializeField] protected DebugDestinationAssigner destinationAssigner;
        [SerializeField] protected DebugDodgeSimulation dodgeSimulation;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public HealthSystem Health { get; private set; }
        public CharacterCore Opponent { get; private set; }
        public CharacterType Type => type;

        public void Initialize(CharacterCore opponent)
        {
            Health = new HealthSystem(type.MaxHealth);
            Opponent = opponent;
            hexLink.Initialize(this);
            actionPriorityHandler.Initialize(this);
            powerUpsCollector.Initialize(this);
            destinationAssigner?.Initialize(this);
            dodgeSimulation?.Initialize(this);
        }
    }
}
