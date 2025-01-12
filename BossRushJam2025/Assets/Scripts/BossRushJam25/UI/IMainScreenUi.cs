using UnityEngine;

namespace BossRushJam25.UI {
   public interface IMainScreenUi {
      CanvasGroup CanvasGroup { get; }

      // ReSharper disable once InconsistentNaming
      GameObject gameObject { get; }
   }
}