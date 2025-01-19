using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   [RequireComponent(typeof(GridHex))]
   public class GlyphChunk : MonoBehaviour {
      private static readonly int emissiveColorShaderId = Shader.PropertyToID("_EmissiveColor");

      [SerializeField] protected GlyphChunkType type;
      [SerializeField] protected GridHex hex;
      [SerializeField] protected MeshRenderer[] glyphRenderers;

      public GlyphChunkType Type => type;
      public GridHex Hex => hex;
      private Material GlyphMaterial { get; set; }
      public float Progress { get; private set; }

      private void Reset() {
         hex = GetComponent<GridHex>();
      }

      private void Start() {
         if (glyphRenderers.Length == 0) return;

         GlyphMaterial = new Material(glyphRenderers[0].material);
         foreach (var glyphRenderer in glyphRenderers) {
            glyphRenderer.material = GlyphMaterial;
         }

         SetActivationProgress(0);
      }

      public void SetActivationProgress(float progress) {
         Progress = progress;

         if (GlyphMaterial) {
            GlyphMaterial.SetColor(emissiveColorShaderId, Color.magenta * Progress * 100);
         }
      }
   }
}