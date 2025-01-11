using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BossRushJam25.HexGrid {
   public class GridHex : MonoBehaviour {
      [SerializeField] protected Transform hexContentParent;
      [SerializeField] protected Transform hexOffsetTransform;
      [SerializeField] protected Vector3 defaultOffset = Vector3.zero;
      [SerializeField] protected Vector3 movingOffset = new Vector3(0, .1f, 0);

      [SerializeField] protected MeshRenderer hexRenderer;
      [SerializeField] protected NavMeshObstacle navMeshObstacle;
      [SerializeField] protected HexHighlightType noHighlight;

      private List<GridHexContent> Contents { get; } = new List<GridHexContent>();

      public bool Highlighted { get; private set; }
      public Vector2Int Coordinates { get; private set; }
      public string InitialName { get; set; }
      public bool IsMoving { get; private set; }

      public UnityEvent<bool> OnMovingChanged { get; } = new UnityEvent<bool>();

      private void Awake() {
         navMeshObstacle.enabled = false;
      }

      private void Start() {
         SetNoHighlight();
      }

      public void SetNoHighlight() => SetHighlighted(null);

      public void SetHighlighted(HexHighlightType highlightType) {
         Highlighted = highlightType;
         hexRenderer.material = (Highlighted ? highlightType : noHighlight).HexMaterial;
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
            Contents.Add(Instantiate(contentPrefab, hexContentParent));
         }
      }

      public void SetAsMoving(bool isMoving) {
         if (isMoving == IsMoving) {
            return;
         }

         IsMoving = isMoving;
         hexOffsetTransform.localPosition = IsMoving ? movingOffset : defaultOffset;
         navMeshObstacle.enabled = IsMoving;

         OnMovingChanged.Invoke(IsMoving);
      }
   }
}