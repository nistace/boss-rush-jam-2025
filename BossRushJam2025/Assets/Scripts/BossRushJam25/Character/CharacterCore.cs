using BossRushJam25.Character.AI;
using BossRushJam25.Character.Heroes;
using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character
{
    public class CharacterCore : MonoBehaviour
    {
        [SerializeField] protected CharacterType type;
        [SerializeField] protected HexLink hexLink;
        [SerializeField] protected NavMeshAgent navMeshAgent;
        [SerializeField] protected ActionPriorityHandler actionPriorityHandler;
        [SerializeField] protected PowerUpsDetector powerUpsDetector;
        [SerializeField] protected BatteryDetector batteryDetector;
        [SerializeField] protected BossPatternDetector bossPatternDetector;
        [SerializeField] protected DebugActionsTrigger actionsTrigger;
        [SerializeField] protected HeroAnimator animator;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public PowerUpsDetector PowerUpsDetector => powerUpsDetector;
        public BatteryDetector BatteryDetector => batteryDetector;
        public BossPatternDetector BossPatternDetector => bossPatternDetector;
        public HealthSystem Health { get; private set; }
        public CharacterType Type => type;
        public HeroAnimator Animator => animator;

        public void Initialize()
        {
            Health = new HealthSystem(type.MaxHealth, type.Vulnerabilities);
            actionPriorityHandler.Initialize(this);
            powerUpsDetector.Initialize(this);
            batteryDetector.Initialize(this);
            actionsTrigger?.Initialize(this);
            bossPatternDetector?.Initialize(this);
        }
    }
}
