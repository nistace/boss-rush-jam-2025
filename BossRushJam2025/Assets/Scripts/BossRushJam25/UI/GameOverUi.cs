using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BossRushJam25.UI {
   public class GameOverUi : MonoBehaviour, IMainScreenUi {
      [SerializeField] protected CanvasGroup canvasGroup;
      [SerializeField] protected TMP_Text gameOverText;
      [SerializeField] protected Button backToMainMenuButton;

      public UnityEvent OnBackToMainMenuClicked => backToMainMenuButton.onClick;
      public CanvasGroup CanvasGroup => canvasGroup;

      public void SetGameOverText(string text) => gameOverText.text = text;
   }
}