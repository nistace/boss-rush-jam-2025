using System.Collections.Generic;
using BossRushJam25.BossFights;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses.GoldFist {
   [RequireComponent(typeof(BossCore))]
   public class GoldFistCore : MonoBehaviour {
      [SerializeField] protected BossCore core;
      [SerializeField] protected GridHexContentType fistContentType;
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
      private HashSet<GridHexContent> ActiveBatteries { get; } = new HashSet<GridHexContent>();
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
         BossFightInfo.OnEnded.AddListener(HandleBossFightEnded);
         spawnBatteryPattern.OnBatterySpawned.AddListener(HandleBatterySpawned);
         GridHexContent.OnAnyContentHealthChanged.AddListener(HandleAnyContentHealthChanged);
      }

      private void HandleBossFightEnded() {
         if (patternManager.CurrentAttack) patternManager.CurrentAttack.Interrupt();
         GetComponentInChildren<GoldFistAnimator>().EndBattle(!core.Health.Empty);
      }

      private void HandleAnyContentHealthChanged(GridHexContent content, HealthSystem health, int healthDelta) {
         if (content.Type != fistContentType) return;
         if (healthDelta > 0) return;

         core.Health.DamagePure(-healthDelta);
      }

      private void HandleBatterySpawned(GridHexContent spawnedBattery) {
         if (ActiveBatteries.Add(spawnedBattery)) {
            spawnedBattery.HealthSystem.OnHealthChanged.AddListener(HandleAnyBatteryHealthChanged);
         }
      }

      private void HandleAnyBatteryHealthChanged(int newHealth, int healthDelta) {
         if (healthDelta >= 0) return;

         var destroyedBatteries = ActiveBatteries.RemoveWhere(t => t.HealthSystem.Empty);
         core.Health.DamagePure(destroyedBatteries * damageTakenByDestroyedBattery);
      }

      private void HandleBossHealthChanged(int newHealth, int delta) {
         if (newHealth > NextSpawnBatteryHealthThreshold) return;

         if (patternManager.CurrentAttack) patternManager.CurrentAttack.Interrupt();
         TimeBeforeNextPattern = 2;
      }

      private void Update() {
         if (!BossFightInfo.IsPlaying) return;
         if (patternManager.IsExecutingAttack) return;

         TimeBeforeNextPattern -= Time.deltaTime;

         if (TimeBeforeNextPattern <= 0) {
            if (core.Health.Current <= NextSpawnBatteryHealthThreshold) {
               NextSpawnBatteryHealthThreshold -= batteryAndTurretHealthThreshold;
               NextAttackIndex = 0;
               patternManager.ExecuteAttack(spawnBatteryPattern);
            }
            else {
               var sequence = ActiveBatteries.Count > 0 ? activeBatteryPatternSequence : defaultPatternSequence;
               var attackPattern = sequence[NextAttackIndex % sequence.Length];
               patternManager.ExecuteAttack(attackPattern);
               NextAttackIndex = (NextAttackIndex + 1) % sequence.Length;
            }
            TimeBeforeNextPattern = delayBetweenAttacksOverTime.Evaluate(BossFightInfo.BattleTime);
         }
      }
   }
}