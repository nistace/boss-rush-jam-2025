using BossRushJam25.BossFights;
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
        [SerializeField] protected DamageableHexDetector damageableHexDetector;
        [SerializeField] protected HexDetector batteryDetector;
        [SerializeField] protected BossPatternDetector bossPatternDetector;
        [SerializeField] protected DebugActionsTrigger actionsTrigger;
        [SerializeField] protected HeroAnimator animator;

        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public HexLink HexLink => hexLink;
        public ActionPriorityHandler ActionPriorityHandler => actionPriorityHandler;
        public PowerUpsDetector PowerUpsDetector => powerUpsDetector;
        public HexDetector BatteryDetector => batteryDetector;
        public DamageableHexDetector DamageableHexDetector => damageableHexDetector;
        public BossPatternDetector BossPatternDetector => bossPatternDetector;
        public DamageInfo DamageInfo { get; private set; }
        public HealthSystem Health { get; private set; }
        public CharacterType Type => type;
        public HeroAnimator Animator => animator;

        public void Initialize()
        {
            Health = new HealthSystem(type.MaxHealth, type.Vulnerabilities);
            DamageInfo = type.DamageInfo;
            actionPriorityHandler.Initialize(this);
            powerUpsDetector.Initialize(this);
            actionsTrigger?.Initialize(this);
            bossPatternDetector?.Initialize(this);
            navMeshAgent.enabled = BossFightInfo.IsPlaying;
        }

        private void Start()
        {
            BossFightInfo.OnStarted.AddListener(HandleBossFightStarted);
            BossFightInfo.OnEnded.AddListener(HandleBossFightEnded);
        }

        private void OnDestroy() {
            BossFightInfo.OnStarted?.RemoveListener(HandleBossFightStarted);
            BossFightInfo.OnEnded?.RemoveListener(HandleBossFightEnded);
        }

        private void HandleBossFightStarted() => navMeshAgent.enabled = BossFightInfo.IsPlaying;
        private void HandleBossFightEnded() => navMeshAgent.enabled = BossFightInfo.IsPlaying;

        public void ChangeDamageInfo(DamageInfo newDamageInfo) => DamageInfo = newDamageInfo;
    }
}
