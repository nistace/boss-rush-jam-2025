using UnityEngine;

namespace BossRushJam25 {
   public class HexGridController : MonoBehaviour {
      [SerializeField] protected Vector2Int gridSize = new(10, 10);
      [SerializeField] protected float hexRadius = 1;

      private float InnerRadius { get; set; }

      private void RefreshInnerRadius() => InnerRadius = Mathf.Sqrt(hexRadius * hexRadius * 3 / 4);

      private void Start() {
         RefreshInnerRadius();
      }

      private Vector3 GetHexCenterAtCoordinates(Vector2Int coordinates) {
         var x = coordinates.x;
         var z = coordinates.y;
         return new Vector3((2 * x + z % 2) * InnerRadius, 0, 1.5f * z * hexRadius);
      }

      private void OnDrawGizmosSelected() {
         Gizmos.color = Color.cyan;

         if (!Application.isPlaying) RefreshInnerRadius();

         for (var x = 0; x < gridSize.x; x++) {
            for (var z = 0; z < gridSize.y; z++) {
               var hexCenter = GetHexCenterAtCoordinates(new Vector2Int(x, z));
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
   }
}