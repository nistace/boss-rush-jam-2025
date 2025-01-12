using BossRushJam25.Cameras;
using BossRushJam25.UI;

namespace BossRushJam25.GameControllers {
   public class GameOverState : AbstractGameState {
      private bool Won { get; }

      public GameOverState(bool won) {
         Won = won;
      }

      public override void Enable() {
         CameraController.Instance.MoveToGamePosition(false);
         MainCanvas.GameOver.SetGameOverText(Won ? "Hero won" : "Boss won");
         MainCanvas.Show<GameOverUi>(false, HandleUiShown);
      }

      private void HandleUiShown() {
         MainCanvas.GameOver.OnBackToMainMenuClicked.AddListener(HandleBackToMainMenuClicked);
      }

      private void HandleBackToMainMenuClicked() => ChangeState(MainMenuState.State);

      public override void Disable() {
         MainCanvas.GameOver.OnBackToMainMenuClicked.RemoveListener(HandleBackToMainMenuClicked);
      }

      public override void Tick() { }
   }
}