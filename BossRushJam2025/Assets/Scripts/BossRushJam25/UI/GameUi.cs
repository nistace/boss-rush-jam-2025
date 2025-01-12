using UnityEngine;

namespace BossRushJam25.UI {
   public class GameUi : MonoBehaviour, IMainScreenUi {
      [SerializeField] protected CanvasGroup canvasGroup;
      [SerializeField] protected HealthBarUi heroHealthBar;
      [SerializeField] protected HealthBarUi bossHealthBar;

      public HealthBarUi HeroHealthBar => heroHealthBar;
      public HealthBarUi BossHealthBar => bossHealthBar;
      public CanvasGroup CanvasGroup => canvasGroup;
   }
}