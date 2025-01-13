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

      public static void HideAllAffectedHexes() {
         foreach (var activeAffectedHex in Instance.ActiveAffectedHexes.Values) {
            Instance.PoolVisual(activeAffectedHex);
         }

         Instance.ActiveAffectedHexes.Clear();
      }

      public static void SetAffectedHex(Vector2Int position, bool active) {
         if (Instance.ActiveAffectedHexes.ContainsKey(position) == active) return;
         if (active) {
            var visual = Instance.GetVisualInstance();
            visual.position = HexGridController.Instance.CoordinatesToWorldPosition(position);
            Instance.ActiveAffectedHexes.Add(position, visual);
         }
         else {
            Instance.PoolVisual(Instance.ActiveAffectedHexes[position]);
            Instance.ActiveAffectedHexes.Remove(position);
         }
      }

      private void PoolVisual(Transform transform) {
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