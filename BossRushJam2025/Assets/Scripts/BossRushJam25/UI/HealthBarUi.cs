using BossRushJam25.Character;
using UnityEngine;
using UnityEngine.UI;

namespace BossRushJam25.UI {
   public class HealthBarUi : MonoBehaviour {
      [SerializeField] protected Image healthBarFillImage;
      [SerializeField] protected Image healthBarDiffImage;
      [SerializeField] protected AnimationCurve adjustmentOverTime;

      private HealthSystem ObservedHealthSystem { get; set; }
      private float AdjustmentStartTime { get; set; }

      public void Setup(HealthSystem healthSystem) {
         ObservedHealthSystem?.OnHealthChanged.RemoveListener(HandleHealthChanged);
         ObservedHealthSystem = healthSystem;
         ObservedHealthSystem.OnHealthChanged.AddListener(HandleHealthChanged);
         healthBarFillImage.fillAmount = healthSystem.HealthRatio;
         healthBarDiffImage.fillAmount = healthSystem.HealthRatio;
         AdjustmentStartTime = 0;
      }

      private void HandleHealthChanged(int newHealth, int diff) {
         if (ObservedHealthSystem == null) return;
         if (diff < 0) {
            healthBarFillImage.fillAmount = ObservedHealthSystem.HealthRatio;
         }
         else {
            healthBarDiffImage.fillAmount = ObservedHealthSystem.HealthRatio;
         }
         AdjustmentStartTime = Time.time;
      }

      private void Update() {
         if (ObservedHealthSystem == null) return;
         var adjustmentDelta = adjustmentOverTime.Evaluate(Time.time - AdjustmentStartTime) * Time.deltaTime;
         healthBarFillImage.fillAmount = Mathf.MoveTowards(healthBarFillImage.fillAmount, ObservedHealthSystem.HealthRatio, adjustmentDelta);
         healthBarDiffImage.fillAmount = Mathf.MoveTowards(healthBarDiffImage.fillAmount, ObservedHealthSystem.HealthRatio, adjustmentDelta);
      }
   }
}