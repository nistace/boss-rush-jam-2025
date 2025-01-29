using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses.GoldFist {
   [RequireComponent(typeof(Animator))]
   public class GoldFistAnimator : MonoBehaviour {
      [SerializeField] protected Animator animator;
      [SerializeField] protected Transform objectAnchorBone;
      [SerializeField] protected float movementSmoothTime = .1f;
      [SerializeField] protected float movementMaxSpeed = 2;
      [SerializeField] protected float slerpDamp = 3f;
      [SerializeField] protected float movingParamTrueOverSpeed = .5f;
      [SerializeField] protected float targetDistanceWithTarget = .2f;

      private static readonly int movingAnimParam = Animator.StringToHash("Moving");
      private static readonly int cancelAllAnimParam = Animator.StringToHash("CancelAll");
      private static readonly int nextPhaseAnimParam = Animator.StringToHash("NextPhase");

      public enum AttackAnimation {
         None = 0,
         IndexShot = 1,
         Stomp = 2,
         PlaceBatteryAndTurret = 3
      }

      public Transform Target { get; set; }
      private Vector3 TargetPosition => Target ? Target.position : HexGridController.Instance.GetPeripheralPosition(transform.forward);
      public Transform AnchoredObject { get; set; }
      public bool IsAtTarget => !Target || (transform.position - Target.position).sqrMagnitude < targetDistanceWithTarget * targetDistanceWithTarget;
      public UnityEvent OnKeyPointReached { get; } = new UnityEvent();

      private Vector3 velocity;

      private void Reset() {
         animator = GetComponent<Animator>();
      }

      private void Start() {
         transform.forward = Vector3.back;
         transform.position = TargetPosition;
      }

      private void Update() {
         transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref velocity, movementSmoothTime, movementMaxSpeed);
         if (Target) transform.forward = Vector3.Slerp(transform.forward, Target.forward, slerpDamp * Time.deltaTime);
         animator.SetBool(movingAnimParam, velocity.sqrMagnitude > movingParamTrueOverSpeed * movingParamTrueOverSpeed);
      }

      private void LateUpdate() {
         if (AnchoredObject) {
            AnchoredObject.transform.position = objectAnchorBone.position;
            AnchoredObject.transform.rotation = objectAnchorBone.rotation;
         }
      }

      public void StartAttack(AttackAnimation attackAnimation) {
         animator.SetTrigger($"{attackAnimation}");
         animator.ResetTrigger(nextPhaseAnimParam);
      }

      public void NextPhase() => animator.SetTrigger(nextPhaseAnimParam);
      public void EndAttack() => animator.SetTrigger(cancelAllAnimParam);

      public void KeyPointAnimEvent() => OnKeyPointReached.Invoke();
   }
}