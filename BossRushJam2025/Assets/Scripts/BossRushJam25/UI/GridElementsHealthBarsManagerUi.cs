using System.Collections.Generic;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.UI {
   public class GridElementsHealthBarsManagerUi : MonoBehaviour {
      [SerializeField] protected HealthBarUi healthBarPrefab;

      private Dictionary<GridHexContent, HealthBarUi> ActiveHealthBars { get; } = new Dictionary<GridHexContent, HealthBarUi>();
      private Queue<HealthBarUi> PooledHealthBarUis { get; } = new Queue<HealthBarUi>();

      private void Start() {
         GridHexContent.OnAnyContentHealthChanged.AddListener(HandleAnyContentHealthChanged);
      }

      private void OnDestroy() {
         GridHexContent.OnAnyContentHealthChanged.RemoveListener(HandleAnyContentHealthChanged);
      }

      private void HandleAnyContentHealthChanged(GridHexContent gridHexContent, HealthSystem gridHexContentHealth) {
         if (ActiveHealthBars.TryGetValue(gridHexContent, out var barUi)) {
            if (gridHexContentHealth.Empty || gridHexContentHealth.Full) {
               PooledHealthBarUis.Enqueue(barUi);
               barUi.gameObject.SetActive(false);
               ActiveHealthBars.Remove(gridHexContent);
            }
         }
         else {
            if (!Mathf.Approximately(gridHexContentHealth.HealthRatio, 0) && !Mathf.Approximately(gridHexContentHealth.HealthRatio, 1)) {
               barUi = PooledHealthBarUis.Count > 0 ? PooledHealthBarUis.Dequeue() : Instantiate(healthBarPrefab, transform);
               barUi.gameObject.SetActive(true);
               barUi.Setup(gridHexContentHealth);
               ActiveHealthBars.Add(gridHexContent, barUi);
            }
         }
      }

      private void Update() {
         var camera = Camera.main;
         foreach (var activeHealthBar in ActiveHealthBars) {
            activeHealthBar.Value.RectTransform.position = camera.WorldToScreenPoint(activeHealthBar.Key.transform.position);
         }
      }
   }
}