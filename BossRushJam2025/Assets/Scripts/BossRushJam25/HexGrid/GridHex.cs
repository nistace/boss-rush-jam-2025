using System.Collections.Generic;
using System.Linq;
using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BossRushJam25.HexGrid {
   public class GridHex : MonoBehaviour {
      [SerializeField] protected GridHexType type;
      [SerializeField] protected Transform hexContentParent;

      [SerializeField] protected MeshRenderer highlightRenderer;
      [SerializeField] protected NavMeshObstacle navMeshObstacle;

      private List<GridHexContent> Contents { get; } = new List<GridHexContent>();
      public IReadOnlyList<GridHexContent> HexContents => Contents;

      public GridHexType Type => type;
      public Vector2Int Coordinates { get; private set; }
      public string InitialName { get; set; }
      public bool IsMoving { get; private set; }

      public UnityEvent<bool> OnMovingChanged { get; } = new UnityEvent<bool>();

      private void Awake() {
         navMeshObstacle.enabled = type.AlwaysAnObstacle;
      }

      private void Start() {
         SetNoHighlight();
      }

      public void SetNoHighlight() => SetHighlighted(null);

      public void SetHighlighted(HexHighlightType highlightType) {
         highlightRenderer.enabled = highlightType;
         if (highlightType) highlightRenderer.material = highlightType.HexMaterial;
      }

      public void SetCoordinates(Vector2Int coordinates) {
         Coordinates = coordinates;
         name = $"{InitialName}@{coordinates.x:00}{coordinates.y:00}";
      }

      public void Setup(GridHexContent contentInfo) {
         var rotationSteps = Mathf.Max(type.RotationSteps, 1);
         var rotation = Random.Range(0, Mathf.Max(type.RotationSteps, 1)) * 360f / rotationSteps;
         transform.rotation = Quaternion.Euler(0, rotation, 0);

         SetupContent(contentInfo);
      }

      public void SetupContent(GridHexContent contentInfo) {
         foreach (var content in Contents) {
            Destroy(content.gameObject);
         }
         Contents.Clear();
         if (!contentInfo) return;

         var rotationOptionsCount = Mathf.Max(contentInfo.Type.RotationStepsInHex, 1);
         var rotationPerStep = 360f / rotationOptionsCount;
         foreach (var rotationStep in Enumerable.Range(0, rotationOptionsCount).OrderBy(_ => Random.value).Take(contentInfo.Type.MaxToSpawn)) {
            var newContent = Instantiate(contentInfo, hexContentParent);
            newContent.transform.localRotation = Quaternion.Euler(0, rotationPerStep * rotationStep, 0);
            Contents.Add(newContent);
         }
      }

      public void SetAsMoving(bool isMoving) {
         if (isMoving == IsMoving) {
            return;
         }

         IsMoving = isMoving;
         navMeshObstacle.enabled = IsMoving || type.AlwaysAnObstacle;

         OnMovingChanged.Invoke(IsMoving);
      }

      public void TryDamageContents(int damageDealt, DamageType damageType) {
         var anyContentDestroyed = false;
         foreach (var content in Contents) {
            content.TryDamage(damageDealt, damageType);
            anyContentDestroyed |= content.HealthSystem.Empty;
         }

         if (anyContentDestroyed) {
            foreach (var contentToDestroy in Contents.Where(t => t.HealthSystem.Empty).ToArray()) {
               Destroy(contentToDestroy.gameObject);
               Contents.Remove(contentToDestroy);
            }
         }
      }
   }
}