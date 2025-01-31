using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   public class AffectedHexesManager : MonoBehaviour {
      [SerializeField] protected Transform affectedHexPrefab;

      private static AffectedHexesManager Instance { get; set; }

      private Dictionary<Vector2Int, HashSet<Object>> AffectedHexesPerSource { get; } = new Dictionary<Vector2Int, HashSet<Object>>();
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

      public static void HideAllAffectedHexes(Object fromSource) {
         var hexesToHide = new HashSet<Vector2Int>();
         foreach (var affectedHexPerSource in Instance.AffectedHexesPerSource.Where(affectedHexPerSource => affectedHexPerSource.Value.Remove(fromSource) && affectedHexPerSource.Value.Count == 0)) {
            hexesToHide.Add(affectedHexPerSource.Key);
         }

         foreach (var hexToHide in hexesToHide) {
            Instance.UnTargetFromAllSourcesAtPosition(hexToHide);
         }
      }

      public static void HideAllAffectedHexes() {
         foreach (var activeAffectedHex in Instance.ActiveAffectedHexes) {
            Instance.PoolVisual(activeAffectedHex.Value);
            if (HexGridController.Instance.TryGetHex(activeAffectedHex.Key, out var hex)) {
               hex.SetAsTargeted(false);
            }
         }

         Instance.ActiveAffectedHexes.Clear();
         Instance.AffectedHexesPerSource.Clear();
      }

      private void UnTargetFromAllSourcesAtPosition(Vector2Int position) {
         if (ActiveAffectedHexes.TryGetValue(position, out var activeAffectedHex)) {
            PoolVisual(activeAffectedHex);
            ActiveAffectedHexes.Remove(position);
         }

         if (HexGridController.Instance.TryGetHex(position, out var hex)) {
            hex.SetAsTargeted(false);
         }

         AffectedHexesPerSource.Remove(position);
      }

      public static void SetAffectedHex(Object source, Vector2Int position, bool active) {
         if (active) {
            if (!HexGridController.Instance.TryGetHex(position, out var hex)) return;

            if (!Instance.AffectedHexesPerSource.TryGetValue(position, out var sources)) {
               sources = new HashSet<Object>();
               Instance.AffectedHexesPerSource.Add(position, sources);
            }
            if (!sources.Add(source)) return;
            if (Instance.ActiveAffectedHexes.ContainsKey(position)) return;

            var visual = Instance.GetVisualInstance();
            hex.ParentTransformToHexContent(visual, true, true);
            hex.SetAsTargeted(true);
            Instance.ActiveAffectedHexes.Add(position, visual);
         }
         else {
            if (!Instance.AffectedHexesPerSource.TryGetValue(position, out var sources)) return;
            if (!sources.Remove(source)) return;
            if (sources.Count > 0) return;

            Instance.UnTargetFromAllSourcesAtPosition(position);
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