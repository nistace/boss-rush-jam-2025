using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Combat {
   public class LaserVisual : MonoBehaviour {
      [SerializeField] protected LineRenderer lineRenderer;

      private ILaserCaster Caster { get; set; }

      private void OnEnable() {
         lineRenderer.gameObject.SetActive(false);
         Caster = GetComponentInParent<ILaserCaster>();
      }

      private void Update() {
         lineRenderer.gameObject.SetActive(Caster.IsShooting);
         if (Caster.IsShooting) {
            var targetHexWorldPosition = HexGridController.Instance.CoordinatesToWorldPosition(Caster.CoordinatesWhereShotIsBlocked);
            lineRenderer.SetPosition(0, lineRenderer.transform.position);
            lineRenderer.SetPosition(1, new(targetHexWorldPosition.x, lineRenderer.transform.position.y, targetHexWorldPosition.z));
         }
      }
   }
}