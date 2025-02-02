using BossRushJam25.BossFights;
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

      public override void Enable() {
         AffectedHexesManager.HideAllAffectedHexes();
         HexGridController.Instance.ClearGrid();
         if (Hero) Object.Destroy(Hero.gameObject);
         if (Boss) Object.Destroy(Boss.gameObject);

         HexGridController.Instance.Build(GameConfig.Instance.BossPrefab.Type.HexGridPreset);
         GameConfig.Instance.PowerUpsManager.Initialize();
         var heroSpawnPosition = HexGridController.Instance.CoordinatesToWorldPosition(GameConfig.Instance.BossPrefab.Type.HexGridPreset.HeroSpawnPosition);
         Hero = Object.Instantiate(GameConfig.Instance.HeroPrefab, heroSpawnPosition, Quaternion.identity);
         Boss = Object.Instantiate(GameConfig.Instance.BossPrefab);
         BossFightInfo.Setup(Hero, Boss);
         Hero.Initialize();
         Boss.Initialize();

         MainCanvas.Game.HeroHealthBar.Setup(Hero.Health);
         MainCanvas.Game.BossHealthBar.Setup(Boss.Health);

         CameraController.Instance.MoveToGamePosition(false);
         SpinStrategy = GameConfig.Instance.SpinStrategy.GetComponent<ISpinStrategy>();
         SpinStrategy.Reset();

         MainCanvas.Show<GameUi>(false, HandleUiShown);

         SoundManager.Instance.PlayMusic(volumeRatio: 1f);
      }

      private void HandleUiShown() {
         SpinStrategy.Enable();
         GameInputs.Controls.Player.Enable();
         // GameInputs.Controls.Player.DamageHero.performed += HandleDamageHeroPerformed;
         // GameInputs.Controls.Player.DamageBoss.performed += HandleDamageBossPerformed;
         GameInputs.Controls.Player.ToggleControlHex.performed += HandleToggleControlHexPerformed;
         MainCanvas.Game.ControlHexToggle.OnClicked.AddListener(HandleControlHexToggleClicked);

         Hero.Health.OnHealthChanged.AddListener(HandleHeroHealthChanged);
         Boss.Health.OnHealthChanged.AddListener(HandleBossHealthChanged);
      }

      private void HandleToggleControlHexPerformed(InputAction.CallbackContext obj) => ToggleControlHexActive();

      private static void HandleControlHexToggleClicked() => ToggleControlHexActive();

      private static void ToggleControlHexActive() {
         if (!HexGridController.Instance) return;
         if (!HexGridController.Instance.ControlHex) return;
         HexGridController.Instance.ControlHex.SetActive(!HexGridController.Instance.ControlHex.Active);
      }

      private void HandleDamageHeroPerformed(InputAction.CallbackContext obj) => Hero.Health.DamagePure(1);
      private void HandleDamageBossPerformed(InputAction.CallbackContext obj) => Boss.Health.DamagePure(1);

      private static void HandleHeroHealthChanged(int newHealth, int _) {
         if (newHealth > 0) return;
         BossFightInfo.EndBattle();
         ChangeState(new GameOverState(false));
      }

      private static void HandleBossHealthChanged(int newHealth, int _) {
         if (newHealth > 0) return;
         BossFightInfo.EndBattle();
         ChangeState(new GameOverState(true));
      }

      public override void Disable() {
         SpinStrategy?.Disable();
         MainCanvas.Hide(false, null);
         GameInputs.Controls.Player.Disable();
         GameInputs.Controls.Player.DamageHero.performed -= HandleDamageHeroPerformed;
         GameInputs.Controls.Player.DamageBoss.performed -= HandleDamageBossPerformed;
         GameInputs.Controls.Player.ToggleControlHex.performed -= HandleToggleControlHexPerformed;
         MainCanvas.Game.ControlHexToggle.OnClicked.RemoveListener(HandleControlHexToggleClicked);

         if (Hero != null) {
            Hero.Health.OnHealthChanged.RemoveListener(HandleHeroHealthChanged);
         }

         if (Boss != null) {
            Boss.Health.OnHealthChanged.RemoveListener(HandleBossHealthChanged);
         }
      }

      public override void Tick() {
         if (BossFightInfo.IsOver) return;

         if (!BossFightInfo.IsPlaying && CameraController.Instance.SqrDistanceWithTargetPosition < 1) {
            BossFightInfo.StartBattle();
         }

         if (!BossFightInfo.IsPlaying) return;

         SpinStrategy.Tick();
      }
   }
}