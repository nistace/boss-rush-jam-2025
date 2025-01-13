using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistIndexShotRayVisual : MonoBehaviour {
      [SerializeField] protected MeshRenderer meshRenderer;
      [SerializeField] protected GoldFistIndexShotAttackPattern attackPattern;

      private void OnEnable() {
         meshRenderer.enabled = false;
      }

      private void Update() {
         meshRenderer.enabled = attackPattern.IsShooting;
         if (attackPattern.IsShooting) {
            var scale = Vector3.Distance(HexGridController.Instance.CoordinatesToWorldPosition(attackPattern.ShotBlockingHexCoordinates), transform.position);

            meshRenderer.transform.localScale = new Vector3(meshRenderer.transform.localScale.x, meshRenderer.transform.localScale.y, scale);
            meshRenderer.transform.localPosition = new Vector3(0, 0, scale * .5f);
         }
      }
   }
}