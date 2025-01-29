using System.Collections.Generic;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   public class AffectedHexesManager : MonoBehaviour {
      [SerializeField] protected Transform affectedHexPrefab;

      private static AffectedHexesManager Instance { get; set; }

      private Dictionary<Vector2Int, Transform> ActiveAffectedHexes { get; } = new Dictionary<Vector2Int, Transform>();
      private Queue<Transform> Pool { get; } = new Queue<Transform>();

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         HexGridController.OnClearingGrid.AddListener(HandleClearingGrid);
      }

      private void OnDestroy() {
         HexGridController.OnClearingGrid.RemoveListener(HandleClearingGrid);
      }

      private static void HandleClearingGrid() => HideAllAffectedHexes();

      public static void HideAllAffectedHexes() {
         foreach(var activeAffectedHex in Instance.ActiveAffectedHexes)
         {
            Instance.PoolVisual(activeAffectedHex.Value);
            if(HexGridController.Instance.TryGetHex(activeAffectedHex.Key, out var hex))
            {
               hex.SetAsTargeted(false);
            }
         }

         Instance.ActiveAffectedHexes.Clear();
      }

      public static void SetAffectedHex(Vector2Int position, bool active) {
         if (Instance.ActiveAffectedHexes.ContainsKey(position) == active) return;
         if (HexGridController.Instance.TryGetHex(position, out var hex) && active) {
            var visual = Instance.GetVisualInstance();
            hex.ParentTransformToHexContent(visual, true, true);
            hex.SetAsTargeted(true);
            Instance.ActiveAffectedHexes.Add(position, visual);
         }
         else if (Instance.ActiveAffectedHexes.TryGetValue(position, out var existingVisual)) {
            Instance.PoolVisual(existingVisual);
            Instance.ActiveAffectedHexes.Remove(position);
         }
      }

      private void PoolVisual(Transform transform) {
         if (!transform) return;
         transform.SetParent(Instance.transform);
         transform.gameObject.SetActive(false);
         Pool.Enqueue(transform);
      }

      private Transform GetVisualInstance() {
         if (Pool.Count == 0) {
            Pool.Enqueue(Instantiate(affectedHexPrefab, transform));
         }
         var instance = Pool.Dequeue();
         instance.gameObject.SetActive(true);
         return instance;
      }
   }
}