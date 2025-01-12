using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.UI {
   public class MainCanvas : MonoBehaviour {
      [SerializeField] protected float fadeSpeed = 1;

      private static MainCanvas Instance { get; set; }
      public static MainMenuUi MainMenu { get; private set; }
      public static GameOverUi GameOver { get; private set; }
      public static GameUi Game { get; private set; }

      private static HashSet<IMainScreenUi> Screens { get; set; }
      private static Coroutine FadeCoroutine { get; set; }

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         MainMenu = GetComponentInChildren<MainMenuUi>();
         Game = GetComponentInChildren<GameUi>();
         GameOver = GetComponentInChildren<GameOverUi>();
         Screens = GetComponentsInChildren<IMainScreenUi>().ToHashSet();
      }

      public static void Show<T>(bool snap, UnityAction callback) where T : IMainScreenUi => ChangeVisibleScreen(Screens.FirstOrDefault(t => t is T), snap, callback);
      public static void Hide(bool snap, UnityAction callback) => ChangeVisibleScreen(default, snap, callback);

      private static void ChangeVisibleScreen(IMainScreenUi screenToFadeIn, bool snap, UnityAction callback) {
         if (FadeCoroutine != null) Instance.StopCoroutine(FadeCoroutine);
         if (snap) {
            foreach (var screen in Screens) {
               screen.CanvasGroup.alpha = screen == screenToFadeIn ? 1 : 0;
               screen.gameObject.SetActive(screen == screenToFadeIn);
            }
            callback?.Invoke();
         }
         else {
            FadeCoroutine = Instance.StartCoroutine(DoFadeTowards(screenToFadeIn, callback));
         }
      }

      private static IEnumerator DoFadeTowards(IMainScreenUi screenToFadeIn, UnityAction callback) {
         var screensInFinalState = new HashSet<IMainScreenUi>();
         while (screensInFinalState.Count < Screens.Count) {
            foreach (var screen in Screens) {
               if (screensInFinalState.Contains(screen)) continue;
               var targetAlpha = screen == screenToFadeIn ? 1 : 0;
               screen.CanvasGroup.alpha = Mathf.MoveTowards(screen.CanvasGroup.alpha, targetAlpha, Instance.fadeSpeed * Time.deltaTime);
               screen.gameObject.SetActive(screen.CanvasGroup.alpha > 0);
               if (Mathf.Approximately(screen.CanvasGroup.alpha, targetAlpha)) {
                  screensInFinalState.Add(screen);
               }
            }

            yield return null;
         }
         callback?.Invoke();
      }
   }
}