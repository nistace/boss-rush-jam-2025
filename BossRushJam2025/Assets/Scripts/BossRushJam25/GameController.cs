using BossRushJam25.Character;
using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using BossRushJam25.PowerUps;
using BossRushJam25.SpinStrategies;
using UnityEngine;

namespace BossRushJam25 {
   public class GameController : MonoBehaviour {
      [SerializeField] protected GameObject spinStrategyHandler;
      [SerializeField] protected Transform cameraTransform;
      [SerializeField] protected CharacterCore heroPrefab;
      [SerializeField] protected CharacterCore bossPrefab;
      [SerializeField] protected PowerUpsManager powerUpsManager;
      private ISpinStrategy SpinStrategy { get; set; }

      private void Start() {
         SpinStrategy = spinStrategyHandler.GetComponent<ISpinStrategy>() ?? GetComponentInChildren<ISpinStrategy>();
         SpinStrategy.Initialize();
         HexGridController.Instance.Build();
         Instantiate(heroPrefab);
         Instantiate(bossPrefab);
         cameraTransform.position = HexGridController.Instance.GetCenterOfGridPosition();
         GameInputs.Controls.Player.Enable();
         powerUpsManager.Initialize();
      }

      private void Update() {
         SpinStrategy.Tick();
      }
   }
}