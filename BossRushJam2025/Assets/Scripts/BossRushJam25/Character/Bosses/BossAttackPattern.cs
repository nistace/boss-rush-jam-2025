using System.Collections;
using System.Collections.Generic;
using BossRushJam25.Character.Bosses.GoldFist;
using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses {
   public abstract class BossAttackPattern : MonoBehaviour {
      [SerializeField] private GoldFistAnimator animator;
      [SerializeField] protected GameObject gameObjectToActivationDuringExecution;

      public bool IsExecuting { get; private set; }
      protected bool InterruptAsap { get; private set; }
      protected GoldFistAnimator Animator => animator;
      public bool IsAttackable => false;

      private UnityEvent OnExecuting { get; } = new UnityEvent();
      public UnityEvent OnExecuted { get; } = new UnityEvent();
      public UnityEvent OnInterrupted { get; } = new UnityEvent();

      private void Reset() {
         animator = transform.root.GetComponentInChildren<GoldFistAnimator>();
      }

      private void Start() {
         gameObjectToActivationDuringExecution.SetActive(false);
      }

      public void Execute() {
         InterruptAsap = false;
         StartCoroutine(DoExecution());
      }

      private IEnumerator DoExecution() {
         animator.Target = transform;
         gameObjectToActivationDuringExecution.gameObject.SetActive(true);
         IsExecuting = true;
         InterruptAsap = false;
         OnExecuting.Invoke();

         yield return StartCoroutine(Play());

         IsExecuting = false;
         gameObjectToActivationDuringExecution.gameObject.SetActive(false);
         AffectedHexesManager.HideAllAffectedHexes(this);

         if (InterruptAsap) OnInterrupted.Invoke();
         else OnExecuted.Invoke();

         InterruptAsap = false;
         animator.Target = null;
      }

      protected abstract IEnumerator Play();

      public abstract HashSet<Vector2Int> GetAffectedHexes();

      public void Interrupt() {
         InterruptAsap = true;
      }

      protected void HideAreaOfEffectHexes() { }
   }
}