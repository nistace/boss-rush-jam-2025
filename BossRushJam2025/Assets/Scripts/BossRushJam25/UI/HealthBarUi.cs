using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.UI;

namespace BossRushJam25.UI {
   [RequireComponent(typeof(RectTransform))]
   public class HealthBarUi : MonoBehaviour {
      [SerializeField] protected Image healthBarFillImage;
      [SerializeField] protected Image healthBarDiffImage;
      [SerializeField] protected AnimationCurve adjustmentOverTime;

      private HealthSystem ObservedHealthSystem { get; set; }
      private float AdjustmentStartTime { get; set; }
      public RectTransform RectTransform => transform as RectTransform;

      public void Setup(HealthSystem healthSystem) {
         ObservedHealthSystem?.OnHealthChanged.RemoveListener(HandleHealthChanged);
         ObservedHealthSystem = healthSystem;
         ObservedHealthSystem.OnHealthChanged.AddListener(HandleHealthChanged);
         healthBarFillImage.fillAmount = healthSystem.Ratio;
         healthBarDiffImage.fillAmount = healthSystem.Ratio;
         AdjustmentStartTime = 0;
      }

      private void HandleHealthChanged(int newHealth, int diff) {
         if (ObservedHealthSystem == null) return;
         if (diff < 0) {
            healthBarFillImage.fillAmount = ObservedHealthSystem.Ratio;
         }
         else {
            healthBarDiffImage.fillAmount = ObservedHealthSystem.Ratio;
         }
         AdjustmentStartTime = Time.time;
      }

      private void Update() {
         if (ObservedHealthSystem == null) return;
         var adjustmentDelta = adjustmentOverTime.Evaluate(Time.time - AdjustmentStartTime) * Time.deltaTime;
         healthBarFillImage.fillAmount = Mathf.MoveTowards(healthBarFillImage.fillAmount, ObservedHealthSystem.Ratio, adjustmentDelta);
         healthBarDiffImage.fillAmount = Mathf.MoveTowards(healthBarDiffImage.fillAmount, ObservedHealthSystem.Ratio, adjustmentDelta);
      }
   }
}