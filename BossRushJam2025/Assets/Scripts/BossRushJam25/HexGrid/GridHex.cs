using System.Collections.Generic;
using System.Linq;
using BossRushJam25.BossFights;
using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace BossRushJam25.HexGrid {
   public class GridHex : MonoBehaviour {
      [SerializeField] protected GridHexType type;
      [SerializeField] protected Transform hexContentParent;
      [SerializeField] protected MeshRenderer highlightRenderer;
      [SerializeField] protected GameObject lockedRenderer;
      [SerializeField] protected NavMeshObstacle navMeshObstacle;

      private HashSet<GridHexContent> Contents { get; } = new HashSet<GridHexContent>();
      public IReadOnlyCollection<GridHexContent> HexContents => Contents;

      public GridHexType Type => type;
      public Vector2Int Coordinates { get; private set; }
      public string InitialName { get; set; }
      public bool IsMoving { get; private set; }
      public bool IsTargeted { get; private set; }
      public bool IsDirty { get; private set; }

      public UnityEvent<bool> OnMovingChanged { get; } = new UnityEvent<bool>();
      public HashSet<object> SourcesLockingInPlace { get; } = new HashSet<object>();
      public bool LockedInPlace => SourcesLockingInPlace.Count > 0;
      private bool LockedInPlaceRenderingEnabled { get; set; }

      private void Awake() {
         navMeshObstacle.enabled = type.AlwaysAnObstacle;
      }

      private void Start() {
         SetNoHighlight();
         RefreshLockedInPlaceRenderer();

         foreach (var alreadyAttachedContents in GetComponentsInChildren<GridHexContent>()) {
            Contents.Add(alreadyAttachedContents);
            SetLockedInPlaceBy(alreadyAttachedContents, alreadyAttachedContents.Type.LocksHexInPlace);
         }
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

      private void SetupContent(GridHexContent contentInfo) {
         if (!contentInfo) return;

         var rotationOptionsCount = Mathf.Max(contentInfo.Type.RotationStepsInHex, 1);
         var rotationPerStep = 360f / rotationOptionsCount;
         foreach (var rotationStep in Enumerable.Range(0, rotationOptionsCount).OrderBy(_ => Random.value).Take(contentInfo.Type.MaxToSpawn)) {
            var newContent = Instantiate(contentInfo, hexContentParent);
            newContent.transform.localRotation = Quaternion.Euler(0, rotationPerStep * rotationStep, 0);
            Contents.Add(newContent);
            SetLockedInPlaceBy(newContent, newContent.Type.LocksHexInPlace);
         }
      }

      public void ParentTransformToHexContent(Transform transform, bool resetPosition, bool resetRotation) {
         if (!transform) return;
         transform.SetParent(hexContentParent);
         if (resetPosition) {
            transform.localPosition = Vector3.zero;
         }
         if (resetRotation) {
            transform.localRotation = Quaternion.identity;
         }
      }

      public void SetAsMoving(bool isMoving) {
         if (isMoving == IsMoving) {
            return;
         }

         IsMoving = isMoving;
         IsTargeted = false;
         IsDirty = true;
         OnMovingChanged.Invoke(IsMoving);
      }

      public void SetAsTargeted(bool isTargeted) {
         if (isTargeted == IsTargeted) {
            return;
         }

         IsTargeted = isTargeted;
         IsDirty = true;
      }

      public void UpdateHexNavigation() {
         GridHex hero_linked_hex = BossFightInfo.Hero.HexLink.LinkedHex;
         bool neighbour_is_hero_linked_hex = HexGridController.Instance.GetNeighbours(Coordinates).Any(neighbour => neighbour == hero_linked_hex);
         navMeshObstacle.enabled = this != hero_linked_hex && (IsMoving || (IsTargeted && !neighbour_is_hero_linked_hex) || type.AlwaysAnObstacle);
         IsDirty = false;
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
               SetLockedInPlaceBy(contentToDestroy, false);
            }
         }
      }

      public void AddContent(GridHexContent content, bool parentAndResetPosition) {
         if (Contents.Add(content)) {
            SetLockedInPlaceBy(content, content.Type.LocksHexInPlace);
         }

         if (parentAndResetPosition) {
            ParentTransformToHexContent(content.transform, true, true);
         }
      }

      public void RemoveContent(GridHexContent content) {
         if (Contents.Remove(content)) {
            SetLockedInPlaceBy(content, false);
         }
      }

      public bool ContentsAreDamageable(DamageTypes damageType) {
         foreach (var content in Contents) {
            if (content.IsDamageable(damageType)) {
               return true;
            }
         }

         return false;
      }

      public void SetLockedInPlaceBy(object source, bool lockedInPlaceByThisSource) {
         if (lockedInPlaceByThisSource) {
            SourcesLockingInPlace.Add(source);
         }
         else {
            SourcesLockingInPlace.Remove(source);
         }
         RefreshLockedInPlaceRenderer();
      }

      public void SetLockedInPlaceRenderingEnabled(bool enabled) {
         LockedInPlaceRenderingEnabled = enabled;
         RefreshLockedInPlaceRenderer();
      }

      private void RefreshLockedInPlaceRenderer() => lockedRenderer.SetActive(LockedInPlaceRenderingEnabled && LockedInPlace);
   }
}