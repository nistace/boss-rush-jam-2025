using BossRushJam25.Cameras;
using BossRushJam25.UI;
using UnityEngine;

namespace BossRushJam25.GameControllers {
   public class GameController : MonoBehaviour {
      private void Start() {
         CameraController.Instance.MoveToMenuPosition(true);
         MainCanvas.Hide(true, null);
         AbstractGameState.ChangeState(MainMenuState.State);
      }

      private void Update() => AbstractGameState.TickState();
   }
}