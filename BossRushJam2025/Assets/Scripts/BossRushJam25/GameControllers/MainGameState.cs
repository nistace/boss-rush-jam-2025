using BossRushJam25.Cameras;
using BossRushJam25.Character;
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
      private CharacterCore Boss { get; set; }

      public override void Enable() {
         HexGridController.Instance.Build(new[] { GameConfig.Instance.HeroPrefab.Type.SpawnPosition, GameConfig.Instance.BossPrefab.Type.SpawnPosition });
         GameConfig.Instance.PowerUpsManager.Initialize();
         Hero = Object.Instantiate(GameConfig.Instance.HeroPrefab, HexGridController.Instance.CoordinatesToWorldPosition(GameConfig.Instance.HeroPrefab.Type.SpawnPosition), Quaternion.identity);
         Boss = Object.Instantiate(GameConfig.Instance.BossPrefab, HexGridController.Instance.CoordinatesToWorldPosition(GameConfig.Instance.BossPrefab.Type.SpawnPosition), Quaternion.identity);

         MainCanvas.Game.HeroHealthBar.Setup(Hero.Health);
         MainCanvas.Game.BossHealthBar.Setup(Boss.Health);

         CameraController.Instance.MoveToGamePosition(false);
         SpinStrategy = GameConfig.Instance.SpinStrategy.GetComponent<ISpinStrategy>();

         MainCanvas.Show<GameUi>(false, HandleUiShown);
      }

      private void HandleUiShown() {
         SpinStrategy.Enable();
         GameInputs.Controls.Player.Enable();
         GameInputs.Controls.Player.DamageHero.performed += HandleDamageHeroPerformed;
         GameInputs.Controls.Player.DamageBoss.performed += HandleDamageBossPerformed;
         Hero.Health.OnHealthChanged.AddListener(HandleHeroHealthChanged);
         Boss.Health.OnHealthChanged.AddListener(HandleBossHealthChanged);
      }

      private void HandleDamageHeroPerformed(InputAction.CallbackContext obj) => Hero.Health.Damage(1);
      private void HandleDamageBossPerformed(InputAction.CallbackContext obj) => Boss.Health.Damage(1);

      private static void HandleHeroHealthChanged(int newHealth, int _) {
         if (newHealth > 0) return;
         ChangeState(new GameOverState(false));
      }

      private static void HandleBossHealthChanged(int newHealth, int _) {
         if (newHealth > 0) return;
         ChangeState(new GameOverState(true));
      }

      public override void Disable() {
         SpinStrategy.Disable();
         MainCanvas.Hide(false, null);
         GameInputs.Controls.Player.Disable();
         GameInputs.Controls.Player.DamageHero.performed -= HandleDamageHeroPerformed;
         GameInputs.Controls.Player.DamageBoss.performed -= HandleDamageBossPerformed;
         Hero.Health.OnHealthChanged.RemoveListener(HandleHeroHealthChanged);
         Boss.Health.OnHealthChanged.RemoveListener(HandleBossHealthChanged);
      }

      public override void Tick() {
         SpinStrategy.Tick();
      }
   }
}