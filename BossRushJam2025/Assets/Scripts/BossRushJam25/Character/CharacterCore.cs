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
        [SerializeField] protected PowerUpsDetector powerUpsDetector;
        [SerializeField] protected BossPatternDetector bossPatternDetector;
        [SerializeField] protected DebugActionsTrigger actionsTrigger;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public PowerUpsDetector PowerUpsDetector => powerUpsDetector;
        public BossPatternDetector BossPatternDetector => bossPatternDetector;
        public HealthSystem Health { get; private set; }
        public CharacterType Type => type;

        public void Initialize()
        {
            Health = new HealthSystem(type.MaxHealth);
            hexLink.Initialize(this);
            actionPriorityHandler.Initialize(this);
            actionsTrigger?.Initialize(this);
            bossPatternDetector?.Initialize(this);
        }
    }
}
