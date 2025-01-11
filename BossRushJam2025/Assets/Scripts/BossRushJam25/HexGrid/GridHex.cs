using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BossRushJam25.HexGrid {
   public class GridHex : MonoBehaviour {
      [SerializeField] protected Transform hexContentParent;

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
            var rotationOptionsCount = Mathf.Max(contentPrefab.Type.RotationStepsInHex, 1);
            var rotationPerStep = 360f / rotationOptionsCount;
            foreach (var rotationStep in Enumerable.Range(0, rotationOptionsCount).OrderBy(_ => Random.value).Take(contentPrefab.Type.MaxToSpawn)) {
               var newContent = Instantiate(contentPrefab, hexContentParent);
               newContent.transform.localRotation = Quaternion.Euler(0, rotationPerStep * rotationStep, 0);
               Contents.Add(newContent);
            }
         }
      }

      public void SetAsMoving(bool isMoving) {
         if (isMoving == IsMoving) {
            return;
         }

         IsMoving = isMoving;
         navMeshObstacle.enabled = IsMoving;

         OnMovingChanged.Invoke(IsMoving);
      }
   }
}