using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.ControlHexes;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Utils;
using Random = UnityEngine.Random;

namespace BossRushJam25.HexGrid {
   public class HexGridController : MonoBehaviour {
      public static HexGridController Instance { get; private set; }

      [SerializeField] protected int gridRadius = 10;
      [SerializeField] protected float hexRadius = 1;
      [SerializeField] protected GridHexRotationConfig rotationConfig;
      [SerializeField] protected NavMeshSurface navMeshSurface;

      private Dictionary<Vector2Int, GridHex> Hexes { get; } = new Dictionary<Vector2Int, GridHex>();

      private float InnerRadius { get; set; }
      public int GridRadius => gridRadius;
      public float WorldRadius => gridRadius * hexRadius * 2;
      public ICollection<GridHex> AllHexes => Hexes.Values;
      public GridHex HeroSpawnHex { get; private set; }
      public ControlHex ControlHex { get; private set; }

      public static Vector2Int Center => Vector2Int.zero;
      public static Vector3 WorldCenter => Vector3.zero;
      public static UnityEvent OnBuilt { get; } = new UnityEvent();
      public static UnityEvent<HexGridPreset> OnBuiltWithPreset { get; } = new UnityEvent<HexGridPreset>();
      public static UnityEvent OnClearingGrid { get; } = new UnityEvent();
      public static UnityEvent<IReadOnlyCollection<GridHex>> OnSomeHexesStartedMoving { get; } = new UnityEvent<IReadOnlyCollection<GridHex>>();
      public static UnityEvent<IReadOnlyCollection<GridHex>> OnSomeHexesStoppedMoving { get; } = new UnityEvent<IReadOnlyCollection<GridHex>>();

      public void RefreshInnerRadius() => InnerRadius = hexRadius * .5f * Mathf.Sqrt(3);

      private void Awake() {
         Instance = this;
      }

      private void Update() {
         UpdateHexNavigation();
      }

      private void UpdateHexNavigation() {
         foreach (GridHex hex in Hexes.Values) {
            if (hex.IsDirty) {
               hex.UpdateHexNavigation();
            }
         }
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

      public bool TryGetHex(Vector3 worldPosition, out GridHex hex) => Hexes.TryGetValue(WorldToCoordinates(worldPosition), out hex);
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

      private void OnDrawGizmos() {
         Gizmos.color = Color.cyan;

         if (!Application.isPlaying) RefreshInnerRadius();

         for (var x = -gridRadius; x <= gridRadius; x++) {
            for (var z = -gridRadius; z <= gridRadius; z++) {
               var coordinates = new Vector2Int(x, z);
               if (IsCellInGrid(coordinates)) {
                  var hexCenter = CoordinatesToWorldPosition(new Vector2Int(x, z));
                  DrawGizmoHex(hexCenter);
               }
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

      public IReadOnlyList<Vector2Int> GetBorderRing() => GetRingClockwiseCoordinates(Vector2Int.zero, gridRadius);
      public HashSet<Vector2Int> GetBorderVertices() => GetBorderRing().Where(t => GetNeighbours(t).Count == 3).ToHashSet();

      public void RotateHex(Vector2Int hexCoordinates, int steps, UnityAction callback = null) {
         if (steps == 0) return;
         if (!TryGetHex(hexCoordinates, out var hex)) return;
         StartCoroutine(DoRotateHex(hex, steps, callback));
      }

      private IEnumerator DoRotateHex(GridHex hex, int steps, UnityAction callback = null) {
         if (steps == 0 || !hex) {
            yield return null;
         }
         else {
            var hexAsArray = new[] { hex };

            hex.SetAsMoving(true);
            OnSomeHexesStartedMoving.Invoke(hexAsArray);

            yield return new WaitForSeconds(rotationConfig.DelayBeforeRotation);

            var remainingRotation = steps * 60f;
            while (!Mathf.Approximately(remainingRotation, 0)) {
               var frameRotation = Mathf.MoveTowards(0, remainingRotation, rotationConfig.SingleHexRotationSpeed * Time.deltaTime);
               hex.transform.Rotate(Vector3.up, frameRotation);
               remainingRotation -= frameRotation;
               yield return null;
            }

            yield return new WaitForSeconds(rotationConfig.DelayAfterRotation);

            hex.SetAsMoving(false);
            OnSomeHexesStoppedMoving.Invoke(hexAsArray);
         }

         callback?.Invoke();
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

            OnSomeHexesStartedMoving.Invoke(movingHexes);

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

            OnSomeHexesStoppedMoving.Invoke(movingHexes);
         }

         callback?.Invoke();
      }

      private IEnumerator DoMoveHexes(IReadOnlyDictionary<GridHex, Vector2Int> hexDestinationCoordinates, bool withAcceleration) {
         var hexMovements = hexDestinationCoordinates.ToDictionary(t => t.Key, t => (origin: t.Key.transform.position, destination: CoordinatesToWorldPosition(t.Value)));
         var lerp = 0f;
         var time = 0f;
         var coordinatesChanged = false;
         while (lerp < 1) {
            lerp += rotationConfig.GetTranslationSpeedDelta(Time.deltaTime, withAcceleration, time);
            time += Time.deltaTime;

            if (!coordinatesChanged && lerp > .5f) {
               foreach (var hex in hexDestinationCoordinates) {
                  Hexes[hex.Value] = hex.Key;
                  hex.Key.SetCoordinates(hex.Value);
               }
               coordinatesChanged = true;
            }

            foreach (var hexMovement in hexMovements) {
               hexMovement.Key.transform.position = Vector3.Lerp(hexMovement.Value.origin, hexMovement.Value.destination, lerp);
            }

            yield return null;
         }

         if (!coordinatesChanged) {
            foreach (var hex in hexDestinationCoordinates) {
               Hexes[hex.Value] = hex.Key;
               hex.Key.SetCoordinates(hex.Value);
            }
         }

         foreach (var hexMovement in hexMovements) {
            hexMovement.Key.transform.position = hexMovement.Value.destination;
         }
      }

      public HashSet<GridHex> GetNeighbours(Vector2Int hexCoordinates, int steps = 1) =>
         GetRingClockwiseCoordinates(hexCoordinates, steps).Where(t => Hexes.ContainsKey(t)).Select(t => Hexes[t]).ToHashSet();

      public void ClearGrid() {
         OnClearingGrid.Invoke();

         foreach (var hex in Hexes.Values) {
            Destroy(hex.gameObject);
         }

         Hexes.Clear();
      }

      public void Build(HexGridPreset preset) {
         ClearGrid();
         RefreshInnerRadius();

         HeroSpawnHex = InstantiateHex(preset.HeroSpawnPosition, preset.HeroSpawnHexPreset);
         ControlHex = InstantiateHex(preset.HeroControlHexPosition, preset.HeroControlHexPreset).GetComponent<ControlHex>();

         var randomHexesQueue = GenerateRandomQueueOfMissingHexes();

         foreach (var glyph in preset.Glyphs) {
            InstantiateHex(randomHexesQueue.Dequeue(), glyph.Definition.OriginGlyphChunk.Hex);
            for (var i = 0; randomHexesQueue.Count > 0 && i < glyph.Definition.OtherGlyphParts.Count; ++i) {
               InstantiateHex(randomHexesQueue.Dequeue(), glyph.Definition.OtherGlyphParts[i].GlyphChunk.Hex);
            }
         }

         foreach (var presetHex in preset.Presets) {
            for (var i = 0; randomHexesQueue.Count > 0 && i < presetHex.Amount; ++i) {
               InstantiateHex(randomHexesQueue.Dequeue(), presetHex.GridHexPreset);
            }
         }

         while (randomHexesQueue.Count > 0) {
            InstantiateHex(randomHexesQueue.Dequeue(), preset.FillerPreset.HexPrefab, preset.FillerPreset.ContentPrefab);
         }

         navMeshSurface.BuildNavMesh();
         OnBuilt.Invoke();
         OnBuiltWithPreset.Invoke(preset);
      }

      private Queue<Vector2Int> GenerateRandomQueueOfMissingHexes() {
         var remainingHexes = new List<Vector2Int>();
         for (var x = -gridRadius; x <= gridRadius; x++) {
            for (var z = -gridRadius; z <= gridRadius; z++) {
               var coordinates = new Vector2Int(x, z);
               if (IsCellInGrid(coordinates) && !Hexes.ContainsKey(coordinates)) {
                  remainingHexes.Add(coordinates);
               }
            }
         }
         return new Queue<Vector2Int>(remainingHexes.OrderBy(_ => Random.value));
      }

      private GridHex InstantiateHex(Vector2Int coordinates, GridHexPreset preset) => InstantiateHex(coordinates, preset.HexPrefab, preset.ContentPrefab);
      private GridHex InstantiateHex(Vector2Int coordinates, GridHex prefab) => InstantiateHex(coordinates, prefab, null);

      private GridHex InstantiateHex(Vector2Int coordinates, GridHex prefab, GridHexContent content) {
         var hex = Instantiate(prefab, CoordinatesToWorldPosition(coordinates), Quaternion.identity, transform);
         Hexes[coordinates] = hex;
         hex.Setup(content);
         hex.InitialName = $"Hex{coordinates.x:00}{coordinates.y:00}";
         hex.SetCoordinates(coordinates);

         return hex;
      }

      public bool IsCellInGrid(Vector2Int coordinates) => HexCoordinates.HexDistance(coordinates, Vector2Int.zero) <= gridRadius;

      public Vector3 GetRandomPositionOnNavMesh() {
         Vector3 randomPositionOnCircle = (Random.insideUnitCircle * gridRadius).ToVector3(EAxis.Y);

         if (NavMesh.SamplePosition(randomPositionOnCircle, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
            return hit.position;
         }

         return Vector3.zero;
      }

      public Vector3 GetRandomPositionOnNavMesh(Vector3 source, Vector3 direction, float amplitude) {
         int attempts = 0;

         while (attempts < 100) {
            Vector3 randomPositionOnCircle = (Random.insideUnitCircle * gridRadius).ToVector3(EAxis.Y);
            Vector3 sourceToPosition = randomPositionOnCircle - source;
            float angleFromDirection = Vector3.Angle(direction, sourceToPosition);

            if (angleFromDirection < amplitude / 2 && NavMesh.SamplePosition(randomPositionOnCircle, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
               return hit.position;
            }

            attempts++;
         }

         return GetRandomPositionOnNavMesh();
      }

      public bool TryGetRandomGridHex(out GridHex gridHex) => gridHex = Hexes.Values.OrderBy(t => Random.value).FirstOrDefault();
      public bool TryGetRandomGridHex(Func<GridHex, bool> predicate, out GridHex gridHex) => gridHex = Hexes.Values.Where(predicate).OrderBy(t => Random.value).FirstOrDefault();

      public IEnumerable<GridHex> GetGridHexesInArea(Vector3 origin, float radius) {
         IEnumerable<GridHex> hexesInArea = Hexes.Values.Where(hex => (hex.transform.position - origin).sqrMagnitude - radius * radius < 0.01f);

         return hexesInArea;
      }

      public void SetAllLockedInPlaceRenderingEnabled(bool enabled) {
         foreach (var hex in Hexes.Values) {
            hex.SetLockedInPlaceRenderingEnabled(enabled);
         }
      }

      public Vector3 GetPeripheralPosition(Vector3 forward) => WorldCenter - forward * WorldRadius;

      public Vector3 GetClosestPointOnHexBorderFrom(Vector3 position, GridHex hex)
      {
         return hex.transform.position + (position - hex.transform.position).normalized * hexRadius;
      }
   }
}