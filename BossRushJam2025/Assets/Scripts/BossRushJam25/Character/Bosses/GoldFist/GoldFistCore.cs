using BossRushJam25.BossFights;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses.GoldFist {
   [RequireComponent(typeof(BossCore))]
   public class GoldFistCore : MonoBehaviour {
      [SerializeField] protected BossCore core;
      [SerializeField] protected BossPatternManager patternManager;
      [SerializeField] protected BossAttackPattern[] defaultPatternSequence;
      [SerializeField] protected BossAttackPattern[] activeBatteryPatternSequence;
      [SerializeField] protected GoldFistSpawnTurretAndBatteryAttackPattern spawnBatteryPattern;
      [SerializeField] protected int damageTakenByDestroyedBattery = 20;
      [SerializeField] protected float batteryAndTurretHealthThreshold = 50;
      [SerializeField] protected float delayBeforeFirstAttack = 5;
      [SerializeField] protected AnimationCurve delayBetweenAttacksOverTime;

      private int NextAttackIndex { get; set; }
      private float NextSpawnBatteryHealthThreshold { get; set; }
      private GridHexContent CurrentBattery { get; set; }
      private float TimeBeforeNextPattern { get; set; }

      private void Reset() {
         core = GetComponent<BossCore>();
      }

      private void Start() {
         if (core.Initialized) {
            Initialize();
         }
         else {
            core.OnInitialized.AddListener(Initialize);
         }
      }

      private void OnDestroy() {
         if (core) core.OnInitialized.RemoveListener(Initialize);
      }

      private void Initialize() {
         NextSpawnBatteryHealthThreshold = core.Health.Max - batteryAndTurretHealthThreshold;
         NextAttackIndex = 0;
         TimeBeforeNextPattern = delayBeforeFirstAttack;
         core.Health.OnHealthChanged.AddListener(HandleBossHealthChanged);
         spawnBatteryPattern.OnBatterySpawned.AddListener(HandleBatterySpawned);
      }

      private void HandleBatterySpawned(GridHexContent spawnedBattery) {
         CurrentBattery = spawnedBattery;
         CurrentBattery.HealthSystem.OnHealthChanged.AddListener(HandleBatteryHealthChanged);
      }

      private void HandleBatteryHealthChanged(int newHealth, int healthDelta) {
         if (healthDelta >= 0) return;

         core.Health.DamagePure(damageTakenByDestroyedBattery);
         CurrentBattery = default;
      }

      private void HandleBossHealthChanged(int newHealth, int delta) {
         if (newHealth > NextSpawnBatteryHealthThreshold) return;

         if (patternManager.CurrentAttack) patternManager.CurrentAttack.Interrupt();
         TimeBeforeNextPattern = 0;
      }

      private void Update() {
         if (!BossFightInfo.IsPlaying) return;
         if (patternManager.IsExecutingAttack) return;

         TimeBeforeNextPattern -= Time.deltaTime;

         if (TimeBeforeNextPattern <= 0) {
            if (core.Health.Current < NextSpawnBatteryHealthThreshold) {
               NextSpawnBatteryHealthThreshold -= batteryAndTurretHealthThreshold;
               NextAttackIndex = 0;
               patternManager.ExecuteAttack(spawnBatteryPattern, null);
            }
            else {
               var sequence = CurrentBattery ? activeBatteryPatternSequence : defaultPatternSequence;
               var attackPattern = sequence[NextAttackIndex % sequence.Length];
               patternManager.ExecuteAttack(attackPattern, null);
               NextAttackIndex = (NextAttackIndex + 1) % sequence.Length;
            }
            TimeBeforeNextPattern = delayBetweenAttacksOverTime.Evaluate(BossFightInfo.BattleTime);
         }
      }
   }
}