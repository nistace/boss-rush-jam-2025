using BossRushJam25.Inputs;
using BossRushJam25.SpinStrategies;
using UnityEngine;

public class GameController : MonoBehaviour {
   [SerializeField] protected GameObject spinStrategyHandler;
   private ISpinStrategy SpinStrategy { get; set; }

   private void Start() {
      SpinStrategy = spinStrategyHandler.GetComponent<ISpinStrategy>() ?? GetComponentInChildren<ISpinStrategy>();
      SpinStrategy.Initialize();
      GameInputs.Controls.Player.Enable();
   }

   private void Update() {
      SpinStrategy.Tick();
   }

}
