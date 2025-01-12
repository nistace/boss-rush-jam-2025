using UnityEngine;

namespace BossRushJam25.Cameras {
   public class CameraController : MonoBehaviour {
      public static CameraController Instance { get; private set; }

      [SerializeField] protected Vector3 menuPosition;
      [SerializeField] protected Vector3 gamePosition;
      [SerializeField] protected float smoothVelocity;
      [SerializeField] protected float maxSpeed;
      [SerializeField] protected Transform backgroundObject;
      [SerializeField] protected AnimationCurve zToBackgroundYCurve;

      private Vector3 CurrentTargetPosition { get; set; }
      private Vector3 currentVelocity;

      private void Awake() {
         Instance = this;
      }

      private void Update() {
         transform.position = Vector3.SmoothDamp(transform.position, CurrentTargetPosition, ref currentVelocity, smoothVelocity, maxSpeed);
         backgroundObject.localPosition = new Vector3(backgroundObject.localPosition.x, zToBackgroundYCurve.Evaluate(transform.position.z), backgroundObject.localPosition.z);
      }

      public void MoveToMenuPosition(bool snap) => MoveToPosition(menuPosition, snap);
      public void MoveToGamePosition(bool snap) => MoveToPosition(gamePosition, snap);

      private void MoveToPosition(Vector3 position, bool snap) {
         CurrentTargetPosition = position;

         if (snap) {
            transform.position = CurrentTargetPosition;
            currentVelocity = Vector3.zero;
         }
      }
   }
}