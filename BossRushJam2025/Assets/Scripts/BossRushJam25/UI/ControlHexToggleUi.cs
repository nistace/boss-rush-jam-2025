using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BossRushJam25.UI {
   public class ControlHexToggleUi : MonoBehaviour {
      private static readonly int activeAnimParam = Animator.StringToHash("Active");
      [SerializeField] protected Button button;
      [SerializeField] protected Animator animator;

      public UnityEvent OnClicked => button.onClick;

      private void Update() {
         var active = HexGridController.Instance && HexGridController.Instance.ControlHex && HexGridController.Instance.ControlHex.Active;

         animator.SetBool(activeAnimParam, active);
      }
   }
}