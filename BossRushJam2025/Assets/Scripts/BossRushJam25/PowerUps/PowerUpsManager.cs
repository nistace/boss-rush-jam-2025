using System.Collections.Generic;
using System.Linq;
using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.PowerUps {
   public class PowerUpsManager : MonoBehaviour {
      [SerializeField] private PowerUp powerUpPrefab;

      private HashSet<PowerUp> PowerUps { get; } = new();
      public IReadOnlyCollection<PowerUp> ActivePowerUps => PowerUps;

      public UnityEvent OnSetOfActivePowerUpsChanged { get; } = new UnityEvent();

      public void Initialize() {
         PowerUps.Clear();
         SpawnPowerUpOnRandomEmptyHex(powerUpPrefab);
      }

      private void Start() {
         PowerUp.OnAnyCollected.AddListener(HandlePowerUpCollected);
      }

      private void HandlePowerUpCollected(PowerUp powerUp) {
         if (PowerUps.Remove(powerUp)) {
            OnSetOfActivePowerUpsChanged.Invoke();
         }
      }

      public void SpawnDefaultPowerUpOnRandomEmptyHex() => SpawnPowerUpOnRandomEmptyHex(powerUpPrefab);

      public void SpawnPowerUpOnRandomEmptyHex(PowerUp powerUp) {
         if (HexGridController.Instance.TryGetRandomGridHex(hex => !hex.Type.AlwaysAnObstacle && !hex.HexContents.Any(content => content.Type.PreventPowerUpSpawning), out GridHex randomHex)) {
            SpawnPowerUp(randomHex, powerUp);
         }
      }

      public void SpawnPowerUp(GridHex hex, PowerUp prefab) {
         var newInstance = Instantiate(prefab);
         hex.ParentTransformToHexContent(newInstance.transform, true, true);

         PowerUps.Add(newInstance);
         OnSetOfActivePowerUpsChanged.Invoke();
      }
   }
}