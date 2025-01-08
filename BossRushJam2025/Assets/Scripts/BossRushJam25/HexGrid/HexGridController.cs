using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BossRushJam25 {
   public class HexGridController : MonoBehaviour {

      public static HexGridController Instance { get; private set; }

      [SerializeField] protected Vector2Int gridSize = new(10, 10);
      [SerializeField] protected float hexRadius = 1;
      [SerializeField] protected GridHex hexTilePrefab;
      [SerializeField] protected GridHexContentPattern[] patterns;
      [SerializeField] protected AnimationCurve normalRotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

      private Dictionary<Vector2Int, GridHex> Hexes { get; } = new Dictionary<Vector2Int, GridHex>();

      private float InnerRadius { get; set; }

      private void RefreshInnerRadius() => InnerRadius = Mathf.Sqrt(hexRadius * hexRadius * 3 / 4);

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         RefreshInnerRadius();
         Build();
      }

      private Vector3 CoordinatesToWorldPosition(Vector2Int coordinates) {
         var x = coordinates.x;
         var z = coordinates.y;
         return new Vector3((2 * x + Mathf.Abs(z % 2)) * InnerRadius, 0, 1.5f * z * hexRadius);
      }

      public Vector2Int WorldToCoordinates(Vector3 worldPosition) {
         var x = Mathf.FloorToInt((worldPosition.x / InnerRadius - .5f) / 2);
         var y = Mathf.FloorToInt(worldPosition.z / hexRadius / 1.5f);

         var surroundingHexesCoordinates = new[] { new Vector2Int(x, y), new Vector2Int(x + 1, y), new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1) };

         return surroundingHexesCoordinates.OrderBy(t => Vector3.SqrMagnitude(CoordinatesToWorldPosition(t) - worldPosition)).First();
      }

      public bool TryGetHex(Vector2Int coordinates, out GridHex hex) => Hexes.TryGetValue(coordinates, out hex);

      public void SetHighlightedHexAt(Vector2Int coordinates, bool highlighted) {
         if (!TryGetHex(coordinates, out var hex)) return;
         hex.SetHighlighted(highlighted);
      }

      public void UnHighlightAllHexes() {
         foreach (var hex in Hexes.Values) {
            hex.SetHighlighted(false);
         }
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

      public static IReadOnlyList<Vector2Int> GetRingClockwiseCoordinates(Vector2Int center) => new[] {
         new Vector2Int(center.x + center.y % 2 - 1, center.y + 1),
         new Vector2Int(center.x + center.y % 2, center.y + 1),
         new Vector2Int(center.x + 1, center.y),
         new Vector2Int(center.x + center.y % 2, center.y - 1),
         new Vector2Int(center.x + center.y % 2 - 1, center.y - 1),
         new Vector2Int(center.x - 1, center.y)
      };

      public void RotateRingAround(Vector2Int center, float duration) {
         var rotatingHexesCoordinates = GetRingClockwiseCoordinates(center);
         var rotatingHexesDestinations = rotatingHexesCoordinates
            .Select((originCoordinates, originIndex) => (coordinates: originCoordinates, defined: TryGetHex(originCoordinates, out var hex), hex, originIndex))
            .Where(t => t.defined)
            .ToDictionary(t => t.hex, t => rotatingHexesCoordinates[(t.originIndex + 1) % rotatingHexesCoordinates.Count]);

         StartCoroutine(DoMoveHexes(rotatingHexesDestinations, duration));
      }

      private IEnumerator DoMoveHexes(IReadOnlyDictionary<GridHex, Vector2Int> hexDestinationCoordinates, float duration) {
         foreach (var hex in hexDestinationCoordinates) {
            Hexes.Remove(hex.Key.Coordinates);
            hex.Key.SetCoordinates(hex.Value);
         }

         var hexMovements = hexDestinationCoordinates.ToDictionary(t => t.Key, t => (origin: t.Key.transform.position, destination: CoordinatesToWorldPosition(t.Value)));
         for (var time = 0f; time < duration; time += Time.deltaTime) {
            foreach (var hexMovement in hexMovements) {
               hexMovement.Key.transform.position = Vector3.Lerp(hexMovement.Value.origin, hexMovement.Value.destination, normalRotationCurve.Evaluate(time / duration));
            }
            yield return null;
         }

         foreach (var hexMovement in hexMovements) {
            hexMovement.Key.transform.position = hexMovement.Value.destination;
            Hexes[hexDestinationCoordinates[hexMovement.Key]] = hexMovement.Key;
         }
      }

      public HashSet<GridHex> GetNeighbours(Vector2Int hexCoordinates) =>
         GetRingClockwiseCoordinates(hexCoordinates).Where(t => Hexes.ContainsKey(t)).Select(t => Hexes[t]).ToHashSet();

      public HashSet<GridHex> GetNeighbours(GridHex hex) => GetNeighbours(hex.Coordinates);

      private void Build() {
         Hexes.Clear();

         for (var x = 0; x < gridSize.x; x++) {
            for (var z = 0; z < gridSize.y; z++) {
               var coordinates = new Vector2Int(x, z);
               var hex = Instantiate(hexTilePrefab, CoordinatesToWorldPosition(coordinates), Quaternion.identity, transform);
               hex.Setup(patterns[Random.Range(0, patterns.Length)]);
               Hexes[coordinates] = hex;
               hex.InitialName = $"Hex{x:00}{z:00}";
               hex.SetCoordinates(coordinates);
            }
         }
      }
   }
}
