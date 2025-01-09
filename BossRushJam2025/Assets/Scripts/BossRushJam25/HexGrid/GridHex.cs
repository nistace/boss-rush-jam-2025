using System.Collections.Generic;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   public class GridHex : MonoBehaviour {
      [SerializeField] protected MeshRenderer hexRenderer;
      [SerializeField] protected Material defaultMaterial;
      [SerializeField] protected Material highlightMaterial;

      private List<GridHexContent> Contents { get; } = new List<GridHexContent>();

      public bool Highlighted { get; private set; }
      public Vector2Int Coordinates { get; private set; }
      public string InitialName { get; set; }

      private void Start() {
         SetHighlighted(false);
      }

      public void SetHighlighted(bool highlighted) {
         Highlighted = highlighted;
         hexRenderer.material = Highlighted ? highlightMaterial : defaultMaterial;
      }

      public void SetCoordinates(Vector2Int coordinates) {
         Coordinates = coordinates;
         name = $"{InitialName}@{coordinates.x:00}{coordinates.y:00}";
      }

      public void Setup(GridHexContentPattern pattern) {
         foreach (var content in Contents) {
            Destroy(content.gameObject);
         }
         Contents.Clear();
         foreach (var contentPrefab in pattern.Contents) {
            Contents.Add(Instantiate(contentPrefab, transform));
         }
      }
   }
}