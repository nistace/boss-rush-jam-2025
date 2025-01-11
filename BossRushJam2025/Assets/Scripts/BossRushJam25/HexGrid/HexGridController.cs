using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BossRushJam25.HexGrid {
   public class HexGridController : MonoBehaviour {
      public static HexGridController Instance { get; private set; }

      [SerializeField] protected Vector2Int gridSize = new(10, 10);
      [SerializeField] protected float hexRadius = 1;
      [SerializeField] protected GridHex hexTilePrefab;
      [SerializeField] protected GridHexContentPattern[] patterns;
      [SerializeField] protected GridHexRotationConfig rotationConfig;
      [SerializeField] protected NavMeshSurface navMeshSurface;

      private Dictionary<Vector2Int, GridHex> Hexes { get; } = new Dictionary<Vector2Int, GridHex>();

      private float InnerRadius { get; set; }

      private void RefreshInnerRadius() => InnerRadius = hexRadius * .5f * Mathf.Sqrt(3);

      private void Awake() {
         Instance = this;
      }

      public Vector3 CoordinatesToWorldPosition(Vector2Int coordinates) {
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

      public void SetHighlightedHexAt(Vector2Int coordinates, HexHighlightType highlightType) {
         if (!TryGetHex(coordinates, out var hex)) return;
         hex.SetHighlighted(highlightType);
      }

      public void UnHighlightAllHexes() {
         foreach (var hex in Hexes.Values) {
            hex.SetNoHighlight();
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

      public static IReadOnlyList<Vector2Int> GetRingClockwiseCoordinates(Vector2Int center, int radius) => GetRingCoordinates(center, radius, t => t.RotateClockwise());
      public static IReadOnlyList<Vector2Int> GetRingAntiClockwiseCoordinates(Vector2Int center, int radius) => GetRingCoordinates(center, radius, t => t.RotateAntiClockwise());

      public static IReadOnlyList<Vector2Int> GetRingCoordinates(Vector2Int center, int ringRadius, Func<HexCoordinates.EDirection, HexCoordinates.EDirection> rotateDirectionFunc) {
         var result = new List<Vector2Int>();
         var hex = center.Left(ringRadius);
         var initialDirection = rotateDirectionFunc(rotateDirectionFunc(HexCoordinates.EDirection.Left));
         var direction = initialDirection;
         do {
            for (var i = 0; i < ringRadius; ++i) {
               result.Add(hex);
               hex = hex.Neighbour(direction);
            }
            direction = rotateDirectionFunc(direction);
         } while (direction != initialDirection);

         return result;
      }

      public void TranslateRingAround(Vector2Int center, int ringRadius = 1, int translationsSteps = 1, UnityAction callback = null) =>
         StartCoroutine(DoTranslateRingAround(center, ringRadius, translationsSteps, callback));

      private IEnumerator DoTranslateRingAround(Vector2Int center, int ringRadius = 1, int translationsSteps = 1, UnityAction callback = null) {
         if (translationsSteps == 0) {
            yield return null;
         }
         else {
            var movingHexes = GetRingClockwiseCoordinates(center, ringRadius)
               .Select((originCoordinates, originIndex) => (coordinates: originCoordinates, defined: TryGetHex(originCoordinates, out var hex), hex, originIndex))
               .Where(t => t.defined)
               .Select(t => t.hex)
               .ToArray();
            foreach (var movingHex in movingHexes) {
               movingHex.SetAsMoving(true);
            }

            yield return new WaitForSeconds(rotationConfig.DelayBeforeRotation);

            var getRingFunc = translationsSteps > 0 ? (Func<Vector2Int, int, IReadOnlyList<Vector2Int>>)GetRingClockwiseCoordinates : GetRingAntiClockwiseCoordinates;
            for (var i = 0; i < Mathf.Abs(translationsSteps); ++i) {
               var rotatingHexesCoordinates = getRingFunc(center, ringRadius);
               var rotatingHexesDestinations = rotatingHexesCoordinates
                  .Select((originCoordinates, originIndex) => (coordinates: originCoordinates, defined: TryGetHex(originCoordinates, out var hex), hex, originIndex))
                  .Where(t => t.defined)
                  .ToDictionary(t => t.hex, t => rotatingHexesCoordinates[(t.originIndex + 1) % rotatingHexesCoordinates.Count]);

               yield return StartCoroutine(DoMoveHexes(rotatingHexesDestinations, i == 0));
            }

            yield return new WaitForSeconds(rotationConfig.DelayAfterRotation);

            foreach (var movingHex in movingHexes) {
               movingHex.SetAsMoving(false);
            }
         }

         callback?.Invoke();
      }

      private IEnumerator DoMoveHexes(IReadOnlyDictionary<GridHex, Vector2Int> hexDestinationCoordinates, bool withAcceleration) {
         foreach (var hex in hexDestinationCoordinates) {
            Hexes.Remove(hex.Key.Coordinates);
            hex.Key.SetCoordinates(hex.Value);
         }

         var hexMovements = hexDestinationCoordinates.ToDictionary(t => t.Key, t => (origin: t.Key.transform.position, destination: CoordinatesToWorldPosition(t.Value)));
         var lerp = 0f;
         var time = 0f;
         while (lerp < 1) {
            lerp += rotationConfig.GetTranslationSpeedDelta(Time.deltaTime, withAcceleration, time);
            time += Time.deltaTime;

            foreach (var hexMovement in hexMovements) {
               hexMovement.Key.transform.position = Vector3.Lerp(hexMovement.Value.origin, hexMovement.Value.destination, lerp);
            }

            yield return null;
         }

         foreach (var hexMovement in hexMovements) {
            hexMovement.Key.transform.position = hexMovement.Value.destination;
            Hexes[hexDestinationCoordinates[hexMovement.Key]] = hexMovement.Key;
         }
      }

      public HashSet<GridHex> GetNeighbours(Vector2Int hexCoordinates, int steps = 1) =>
         GetRingClockwiseCoordinates(hexCoordinates, steps).Where(t => Hexes.ContainsKey(t)).Select(t => Hexes[t]).ToHashSet();

      public HashSet<GridHex> GetNeighbours(GridHex hex, int steps = 1) => GetNeighbours(hex.Coordinates, steps);

      public Vector3 GetCenterOfGridPosition() => Vector3.Lerp(CoordinatesToWorldPosition(Vector2Int.zero), CoordinatesToWorldPosition(gridSize), .5f);

      public void Build() {
         RefreshInnerRadius();
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

         navMeshSurface.BuildNavMesh();
      }

      public bool IsCellInGrid(Vector2Int coordinates) => coordinates.x >= 0 && coordinates.x < gridSize.x && coordinates.y >= 0 && coordinates.y < gridSize.y;
   }
}