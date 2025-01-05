using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BossRushJam25 {
   public class HexGridController : MonoBehaviour {
      [SerializeField] protected Vector2Int gridSize = new(10, 10);
      [SerializeField] protected float hexRadius = 1;
      [SerializeField] protected GameObject hexTilePrefab;
      [SerializeField] protected Transform cursor;

      private float InnerRadius { get; set; }

      private void RefreshInnerRadius() => InnerRadius = Mathf.Sqrt(hexRadius * hexRadius * 3 / 4);

      private void Start() {
         RefreshInnerRadius();
      }

      private Vector3 CoordinatesToWorldPosition(Vector2Int coordinates) {
         var x = coordinates.x;
         var z = coordinates.y;
         return new Vector3((2 * x + z % 2) * InnerRadius, 0, 1.5f * z * hexRadius);
      }

      private Vector2Int WorldToCoordinates(Vector3 worldPosition) {
         var x = Mathf.FloorToInt((worldPosition.x / InnerRadius - .5f) / 2);
         var y = Mathf.FloorToInt(worldPosition.z / hexRadius / 1.5f);

         var surroundingHexesCoordinates = new[] { new Vector2Int(x, y), new Vector2Int(x + 1, y), new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1) };

         return surroundingHexesCoordinates.OrderBy(t => Vector3.SqrMagnitude(CoordinatesToWorldPosition(t) - worldPosition)).First();
      }

      private bool IsHexInGrid(Vector2Int coordinates) {
         if (coordinates.x < 0) return false;
         if (coordinates.y < 0) return false;
         if (coordinates.x >= gridSize.x) return false;
         if (coordinates.y >= gridSize.y) return false;
         return true;
      }

      private void OnDrawGizmosSelected() {
         Gizmos.color = Color.cyan;

         if (!Application.isPlaying) RefreshInnerRadius();

         for (var x = 0; x < gridSize.x; x++) {
            for (var z = 0; z < gridSize.y; z++) {
               var hexCenter = CoordinatesToWorldPosition(new Vector2Int(x, z));
               DrawGizmoHex(hexCenter);
            }
         }
      }

      private void DrawGizmoHex(Vector3 center) {
         for (var angle = 0; angle < 360; angle += 120) {
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0, angle, 0), Vector3.one);
            Gizmos.DrawLine(new Vector3(InnerRadius, 0, -hexRadius / 2), new Vector3(InnerRadius, 0, hexRadius / 2));
            Gizmos.DrawLine(new Vector3(-InnerRadius, 0, -hexRadius / 2), new Vector3(-InnerRadius, 0, hexRadius / 2));
         }
         Gizmos.matrix = Matrix4x4.identity;
      }

      private void OnDrawGizmos() {
         if (!cursor) return;
         var cursorWorldCoordinates = WorldToCoordinates(cursor.position);
         var hexInGrid = IsHexInGrid(cursorWorldCoordinates);
         Gizmos.color = Color.yellow * (hexInGrid ? 1 : .5f);
         Gizmos.DrawCube(CoordinatesToWorldPosition(cursorWorldCoordinates), new Vector3(.5f, .5f, .5f));
         Gizmos.color = Color.green * (hexInGrid ? 1 : .5f);
         Gizmos.DrawCube(cursor.position, new Vector3(.1f, .7f, .1f));
      }

#if UNITY_EDITOR
      [ContextMenu("Build")]
      private void Build() {
         if (Application.isPlaying) return;

         while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
         }

         for (var x = 0; x < gridSize.x; x++) {
            for (var z = 0; z < gridSize.y; z++) {
               var hex = PrefabUtility.InstantiatePrefab(hexTilePrefab, transform) as GameObject;
               hex.name = $"Hex_{x}_{z}";
               hex.transform.position = CoordinatesToWorldPosition(new Vector2Int(x, z));
               hex.transform.rotation = Quaternion.identity;
            }
         }
      }
#endif
   }
}