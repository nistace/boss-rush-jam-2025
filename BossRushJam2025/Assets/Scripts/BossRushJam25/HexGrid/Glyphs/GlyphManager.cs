using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   public class GlyphManager : MonoBehaviour {
      [SerializeField] protected float deactivatingChunkDuration = .2f;

      private struct ActivatingGlyphInfo {
         public readonly GlyphChunk origin;
         public readonly IReadOnlyCollection<GlyphChunk> allChunks;
         public readonly float activatingStartTime;

         public ActivatingGlyphInfo(GlyphChunk origin, IReadOnlyCollection<GlyphChunk> allChunks, float activatingStartTime) {
            this.origin = origin;
            this.allChunks = allChunks;
            this.activatingStartTime = activatingStartTime;
         }
      }

      private HashSet<GlyphChunk> OriginGlyphChunks { get; } = new HashSet<GlyphChunk>();
      private HashSet<Glyph> AvailableGlyphs { get; } = new HashSet<Glyph>();
      private HashSet<GlyphChunk> InactiveChunks { get; } = new HashSet<GlyphChunk>();
      private Dictionary<Glyph, ActivatingGlyphInfo> ActivatingGlyphs { get; } = new Dictionary<Glyph, ActivatingGlyphInfo>();
      private HashSet<GlyphChunk> DeactivatingChunks { get; } = new HashSet<GlyphChunk>();

      private void Start() {
         HexGridController.OnBuiltWithPreset.AddListener(HandleGridBuiltWithPreset);
         HexGridController.OnClearingGrid.AddListener(ClearAll);
         HexGridController.OnSomeHexesStartedMoving.AddListener(HandleSomeHexesStartedMoving);
         HexGridController.OnSomeHexesStoppedMoving.AddListener(HandleSomeHexesStoppedMoving);
      }

      private void OnDestroy() {
         HexGridController.OnBuiltWithPreset.RemoveListener(HandleGridBuiltWithPreset);
         HexGridController.OnClearingGrid.RemoveListener(ClearAll);
         HexGridController.OnSomeHexesStartedMoving.RemoveListener(HandleSomeHexesStartedMoving);
         HexGridController.OnSomeHexesStoppedMoving.RemoveListener(HandleSomeHexesStoppedMoving);
      }

      private void HandleGridBuiltWithPreset(HexGridPreset preset) {
         ClearAll();

         foreach (var glyph in preset.Glyphs) {
            AvailableGlyphs.Add(glyph);
         }

         foreach (var glyphChunk in HexGridController.Instance.AllHexes.Select(t => t.GetComponent<GlyphChunk>()).Where(t => t)) {
            InactiveChunks.Add(glyphChunk);

            if (AvailableGlyphs.Any(t => t.Definition.OriginGlyphChunk.Type == glyphChunk.Type)) {
               OriginGlyphChunks.Add(glyphChunk);
            }
         }

         RecalculateGlyphs();
      }

      private void ClearAll() {
         AvailableGlyphs.Clear();
         InactiveChunks.Clear();
         ActivatingGlyphs.Clear();
         DeactivatingChunks.Clear();
      }

      private void HandleSomeHexesStartedMoving(IReadOnlyCollection<GridHex> hexes) {
         var brokenGlyphs = ActivatingGlyphs.Where(t => t.Value.allChunks.Any(activeChunk => hexes.Any(movingHex => movingHex == activeChunk.Hex))).ToArray();
         foreach (var brokenGlyph in brokenGlyphs) {
            AvailableGlyphs.Add(brokenGlyph.Key);
            ActivatingGlyphs.Remove(brokenGlyph.Key);
            foreach (var chunkToDeactivate in brokenGlyph.Value.allChunks) {
               InactiveChunks.Add(chunkToDeactivate);
               DeactivatingChunks.Add(chunkToDeactivate);
            }
         }
      }

      private void HandleSomeHexesStoppedMoving(IReadOnlyCollection<GridHex> arg0) => RecalculateGlyphs();

      private void RecalculateGlyphs() {
         foreach (var candidateOrigin in OriginGlyphChunks) {
            if (TryFindCompleteGlyphWithOrigin(candidateOrigin, out var glyph, out var chunksInGlyph)) {
               AvailableGlyphs.Remove(glyph);
               ActivatingGlyphs.Add(glyph, new ActivatingGlyphInfo(candidateOrigin, chunksInGlyph, Time.time));

               foreach (var activatingChunk in chunksInGlyph) {
                  InactiveChunks.Remove(activatingChunk);
                  DeactivatingChunks.Remove(activatingChunk);
               }
            }
         }
      }

      private bool TryFindCompleteGlyphWithOrigin(GlyphChunk origin, out Glyph glyph, out IReadOnlyCollection<GlyphChunk> chunksInGlyph) {
         glyph = default;
         chunksInGlyph = default;

         if (!InactiveChunks.Contains(origin)) return false;

         foreach (var aGlyph in AvailableGlyphs) {
            if (TryGlyphWithOrigin(aGlyph.Definition, origin, out chunksInGlyph)) {
               glyph = aGlyph;
               return true;
            }
         }

         return false;
      }

      private bool TryGlyphWithOrigin(GlyphDefinition glyph, GlyphChunk origin, out IReadOnlyCollection<GlyphChunk> chunksInGlyph) {
         var setOfChunksInGlyph = new HashSet<GlyphChunk> { origin };
         chunksInGlyph = setOfChunksInGlyph;

         if (glyph.OriginGlyphChunk.Type != origin.Type) return false;

         var originTransform = origin.transform;

         foreach (var attachedGlyphPart in glyph.OtherGlyphParts) {
            if (!HexGridController.Instance.TryGetHex(originTransform.TransformPoint(attachedGlyphPart.OffsetWithOrigin), out var otherHex)) return false;

            var otherChunk = InactiveChunks.FirstOrDefault(t => t.Hex == otherHex);

            if (!otherChunk) return false;
            if (otherChunk.Type != attachedGlyphPart.GlyphChunk.Type) return false;
            if (Vector3.Angle(otherChunk.transform.forward, originTransform.TransformDirection(Quaternion.Euler(0, attachedGlyphPart.RotationWithOrigin, 0) * Vector3.forward)) > 5) return false;

            setOfChunksInGlyph.Add(otherChunk);
         }

         return true;
      }

      private void Update() {
         foreach (var deactivatingChunk in DeactivatingChunks) {
            deactivatingChunk.SetActivationProgress(deactivatingChunkDuration <= float.Epsilon ? 0 : deactivatingChunk.Progress - Time.time / deactivatingChunkDuration);
         }

         DeactivatingChunks.RemoveWhere(t => Mathf.Approximately(t.Progress, 0));

         var activatedGlyphs = new HashSet<Glyph>();

         foreach (var activatingGlyph in ActivatingGlyphs) {
            var progress = Mathf.Clamp01(activatingGlyph.Key.CastingTime > float.Epsilon ? (Time.time - activatingGlyph.Value.activatingStartTime) / activatingGlyph.Key.CastingTime : 1);
            foreach (var chunk in activatingGlyph.Value.allChunks) {
               chunk.SetActivationProgress(progress);
            }
            if (Mathf.Approximately(progress, 1)) {
               activatingGlyph.Key.Spawn(activatingGlyph.Value.origin.transform);
               activatedGlyphs.Add(activatingGlyph.Key);
            }
         }

         foreach (var activatedGlyph in activatedGlyphs) {
            ActivatingGlyphs.Remove(activatedGlyph);
         }
      }
   }
}