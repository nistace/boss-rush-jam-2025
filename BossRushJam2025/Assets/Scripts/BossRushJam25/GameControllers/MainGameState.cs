﻿using BossRushJam25.BossFights;
using BossRushJam25.Cameras;
using BossRushJam25.Character;
using BossRushJam25.Character.Bosses;
using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using BossRushJam25.SpinStrategies;
using BossRushJam25.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BossRushJam25.GameControllers {
   public class MainGameState : AbstractGameState {
      public static MainGameState State { get; } = new MainGameState();
      private ISpinStrategy SpinStrategy { get; set; }
      private CharacterCore Hero { get; set; }
      private BossCore Boss { get; set; }
      private float DelayBeforeNextAttack { get; set; }

      public override void Enable() {
         HexGridController.Instance.ClearGrid();
         if (Hero) Object.Destroy(Hero.gameObject);
         if (Boss) Object.Destroy(Boss.gameObject);

         HexGridController.Instance.Build(GameConfig.Instance.BossPrefab.Type.HexGridPreset, new[] { GameConfig.Instance.HeroPrefab.Type.SpawnPosition });
         GameConfig.Instance.PowerUpsManager.Initialize();
         Hero = Object.Instantiate(GameConfig.Instance.HeroPrefab, HexGridController.Instance.CoordinatesToWorldPosition(GameConfig.Instance.HeroPrefab.Type.SpawnPosition), Quaternion.identity);
         Boss = Object.Instantiate(GameConfig.Instance.BossPrefab);
         BossFightInfo.SetHero(Hero);
         BossFightInfo.SetBoss(Boss);
         Hero.Initialize();
         Boss.Initialize();

         MainCanvas.Game.HeroHealthBar.Setup(Hero.Health);
         MainCanvas.Game.BossHealthBar.Setup(Boss.Health);

         CameraController.Instance.MoveToGamePosition(false);
         SpinStrategy = GameConfig.Instance.SpinStrategy.GetComponent<ISpinStrategy>();

         MainCanvas.Show<GameUi>(false, HandleUiShown);
         DelayBeforeNextAttack = 2;
      }

      private void HandleUiShown() {
         SpinStrategy.Enable();
         GameInputs.Controls.Player.Enable();
         GameInputs.Controls.Player.DamageHero.performed += HandleDamageHeroPerformed;
         GameInputs.Controls.Player.DamageBoss.performed += HandleDamageBossPerformed;
         Hero.Health.OnHealthChanged.AddListener(HandleHeroHealthChanged);
         Boss.Health.OnHealthChanged.AddListener(HandleBossHealthChanged);
      }

      private void HandleDamageHeroPerformed(InputAction.CallbackContext obj) => Hero.Health.DamagePure(1);
      private void HandleDamageBossPerformed(InputAction.CallbackContext obj) => Boss.Health.DamagePure(1);

      private static void HandleHeroHealthChanged(int newHealth, int _) {
         if (newHealth > 0) return;
         ChangeState(new GameOverState(false));
      }

      private static void HandleBossHealthChanged(int newHealth, int _) {
         if (newHealth > 0) return;
         ChangeState(new GameOverState(true));
      }

      public override void Disable() {
         SpinStrategy?.Disable();
         MainCanvas.Hide(false, null);
         GameInputs.Controls.Player.Disable();
         GameInputs.Controls.Player.DamageHero.performed -= HandleDamageHeroPerformed;
         GameInputs.Controls.Player.DamageBoss.performed -= HandleDamageBossPerformed;

         if (Hero != null) {
            Hero.Health.OnHealthChanged.RemoveListener(HandleHeroHealthChanged);
         }

         if (Boss != null) {
            Boss.Health.OnHealthChanged.RemoveListener(HandleBossHealthChanged);
         }
      }

      public override void Tick() {
         SpinStrategy.Tick();

         if (!Boss.PatternManager.IsExecutingAttack) {
            DelayBeforeNextAttack -= Time.deltaTime;
            if (DelayBeforeNextAttack < 0 && Boss.PatternManager.HasPatterns()) {
               Boss.PatternManager.ExecuteNextAttack(null);
               DelayBeforeNextAttack = 2;
            }
         }
      }
   }
}