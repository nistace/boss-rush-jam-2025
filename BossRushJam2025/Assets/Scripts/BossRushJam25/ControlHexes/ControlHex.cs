using UnityEngine;

namespace BossRushJam25.ControlHexes {
   public class ControlHex : MonoBehaviour {
      private static readonly int activeAnimParam = Animator.StringToHash("Active");
      [SerializeField] protected bool active = true;
      [SerializeField] protected HexContentDetector detector;
      [SerializeField] protected Animator animator;
      [SerializeField] protected MeshRenderer meshRenderer;
      [SerializeField] protected Material activeMaterial;
      [SerializeField] protected Material inactiveMaterial;

      public bool Active => active;

      public HexContentDetector Detector => detector;

      private void Reset() {
         detector = GetComponentInChildren<HexContentDetector>();
      }

      private void Start() {
         SetActive(active);
      }

      public void SetActive(bool active) {
         this.active = active;
         meshRenderer.material = active ? activeMaterial : inactiveMaterial;
         animator.SetBool(activeAnimParam, active);
      }
   }
}