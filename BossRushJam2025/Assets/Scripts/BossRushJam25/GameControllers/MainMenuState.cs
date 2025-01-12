using BossRushJam25.Cameras;
using BossRushJam25.UI;
using UnityEngine;

namespace BossRushJam25.GameControllers {
   public class MainMenuState : AbstractGameState {
      public static MainMenuState State { get; } = new MainMenuState();

      public override void Enable() {
         CameraController.Instance.MoveToMenuPosition(false);
         MainCanvas.Show<MainMenuUi>(false, HandleUiShown);
      }

      private static void HandleUiShown() {
         MainCanvas.MainMenu.OnStartButtonClicked.AddListener(HandleStartButtonClicked);
         MainCanvas.MainMenu.OnExitButtonClicked.AddListener(HandleEndButtonClicked);
      }

      public override void Disable() {
         MainCanvas.Hide(false, null);
         MainCanvas.MainMenu.OnStartButtonClicked.RemoveListener(HandleStartButtonClicked);
         MainCanvas.MainMenu.OnExitButtonClicked.RemoveListener(HandleEndButtonClicked);
      }

      private static void HandleStartButtonClicked() => ChangeState(MainGameState.State);
      private static void HandleEndButtonClicked() => Application.Quit();

      public override void Tick() { }
   }
}