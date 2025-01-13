using System;
using System.Collections;
using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses {
   [RequireComponent(typeof(Animator))]
   public abstract class BossAttackPattern : MonoBehaviour {
      [SerializeField] protected Animator animator;
      [SerializeField] protected GameObject gameObjectToActivationDuringExecution;

      public bool IsExecuting { get; private set; }
      protected bool InterruptAsap { get; private set; }

      private UnityEvent OnExecuting { get; } = new UnityEvent();
      public UnityEvent OnExecuted { get; } = new UnityEvent();
      public UnityEvent OnInterrupted { get; } = new UnityEvent();

      private void Reset() {
         animator = GetComponent<Animator>();
      }

      private void Start() {
         gameObjectToActivationDuringExecution.SetActive(false);
      }

      public void Execute() {
         InterruptAsap = false;
         StartCoroutine(DoExecution());
      }

      private IEnumerator DoExecution() {
         gameObjectToActivationDuringExecution.gameObject.SetActive(true);
         IsExecuting = true;
         InterruptAsap = false;
         OnExecuting.Invoke();

         yield return StartCoroutine(Play());

         IsExecuting = false;
         gameObjectToActivationDuringExecution.gameObject.SetActive(false);
         AffectedHexesManager.HideAllAffectedHexes();

         if (InterruptAsap) OnInterrupted.Invoke();
         else OnExecuted.Invoke();

         InterruptAsap = false;
      }

      protected abstract IEnumerator Play();

      public void Interrupt() {
         InterruptAsap = true;
      }

      protected void HideAreaOfEffectHexes() { }
   }
}