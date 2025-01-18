using BossRushJam25.Character.AI;
using BossRushJam25.Health;
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
        [SerializeField] protected PowerUpsDetector powerUpsDetector;
        [SerializeField] protected DebugActionsTrigger actionsTrigger;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public PowerUpsDetector PowerUpsDetector => powerUpsDetector;
        public HealthSystem Health { get; private set; }
        public CharacterType Type => type;

        public void Initialize()
        {
            Health = new HealthSystem(type.MaxHealth);
            hexLink.Initialize(this);
            actionPriorityHandler.Initialize(this);
            actionsTrigger?.Initialize(this);
        }
    }
}
