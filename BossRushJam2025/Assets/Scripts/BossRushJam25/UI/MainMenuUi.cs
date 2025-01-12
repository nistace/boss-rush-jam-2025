using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BossRushJam25.UI {
   public class MainMenuUi : MonoBehaviour, IMainScreenUi {
      [SerializeField] protected CanvasGroup canvasGroup;
      [SerializeField] protected Button startButton;
      [SerializeField] protected Button exitButton;

      public UnityEvent OnStartButtonClicked => startButton.onClick;
      public UnityEvent OnExitButtonClicked => exitButton.onClick;
      public CanvasGroup CanvasGroup => canvasGroup;
   }
}