using System.Linq;
using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   [RequireComponent(typeof(GridHex))]
   public class GlyphChunk : MonoBehaviour {
      private static readonly int emissiveColorShaderId = Shader.PropertyToID("_EmissiveColor");

      [SerializeField] protected GlyphChunkType type;
      [SerializeField] protected GridHex hex;
      [SerializeField] protected MeshRenderer[] glyphRenderers;
      [SerializeField] protected float intensity = 100;

      public GlyphChunkType Type => type;
      public GridHex Hex => hex;
      private GlyphMaterial[] Materials { get; set; }
      public float Progress { get; private set; }

      private void Reset() {
         hex = GetComponent<GridHex>();
      }

      private void Start() {
         if (glyphRenderers.Length == 0) return;

         Materials = glyphRenderers.Select(InitializeMeshMaterial).ToArray();
         SetActivationProgress(0);
      }

      public void SetActivationProgress(float progress) {
         Progress = progress;

         foreach (var glyphMaterial in Materials) {
            glyphMaterial.Material.SetColor(emissiveColorShaderId, glyphMaterial.DefaultColor * Progress * intensity);
         }
      }

      public void ScheduleDeletion() {
         foreach (var glyphRenderer in glyphRenderers) {
            Destroy(glyphRenderer.gameObject, .3f);
            Destroy(this);
         }
      }

      private static GlyphMaterial InitializeMeshMaterial(MeshRenderer meshRenderer) {
         var glyphMaterial = new GlyphMaterial(meshRenderer.material);
         meshRenderer.material = glyphMaterial.Material;
         return glyphMaterial;
      }

      private struct GlyphMaterial {
         public Material Material { get; }
         public Color DefaultColor { get; }

         public GlyphMaterial(Material source) {
            Material = new Material(source);
            DefaultColor = Material.GetColor(emissiveColorShaderId);
         }
      }
   }
}