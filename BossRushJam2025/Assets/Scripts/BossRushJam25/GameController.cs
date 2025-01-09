using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using BossRushJam25.SpinStrategies;
using UnityEngine;

namespace BossRushJam25 {
   public class GameController : MonoBehaviour {
      [SerializeField] protected GameObject spinStrategyHandler;
      [SerializeField] protected Transform cameraTransform;
      private ISpinStrategy SpinStrategy { get; set; }

      private void Start() {
         SpinStrategy = spinStrategyHandler.GetComponent<ISpinStrategy>() ?? GetComponentInChildren<ISpinStrategy>();
         SpinStrategy.Initialize();
         HexGridController.Instance.Build();
         cameraTransform.position = HexGridController.Instance.GetCenterOfGridPosition();
         GameInputs.Controls.Player.Enable();
      }

      private void Update() {
         SpinStrategy.Tick();
      }
   }
}