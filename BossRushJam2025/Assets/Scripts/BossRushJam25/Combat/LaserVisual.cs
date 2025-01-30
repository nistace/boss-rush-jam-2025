using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Combat {
   public class LaserVisual : MonoBehaviour {
      [SerializeField] protected MeshRenderer meshRenderer;

      private ILaserCaster Caster { get; set; }

      private void OnEnable() {
         meshRenderer.enabled = false;
         Caster = GetComponentInParent<ILaserCaster>();
      }

      private void Update() {
         meshRenderer.enabled = Caster.IsShooting;
         if (Caster.IsShooting) {
            var targetHexWorldPosition = HexGridController.Instance.CoordinatesToWorldPosition(Caster.CoordinatesWhereShotIsBlocked);
            var scale = Vector3.Distance(targetHexWorldPosition, new Vector3(transform.position.x, targetHexWorldPosition.y, transform.position.z));

            meshRenderer.transform.localScale = new Vector3(meshRenderer.transform.localScale.x, meshRenderer.transform.localScale.y, scale);
            meshRenderer.transform.localPosition = new Vector3(0, 0, scale * .5f);
         }
      }
   }
}