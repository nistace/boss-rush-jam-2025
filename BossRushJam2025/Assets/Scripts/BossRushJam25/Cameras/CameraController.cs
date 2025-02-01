using BossRushJam25.BossFights;
using BossRushJam25.HexGrid;
using Unity.Mathematics;
using UnityEngine;

namespace BossRushJam25.Cameras {
   public class CameraController : MonoBehaviour {
      public static CameraController Instance { get; private set; }

      [SerializeField] protected Transform gameCameraPositioning;
      [SerializeField] protected Transform menuCameraPositioning;
      [SerializeField] protected float smoothVelocity;
      [SerializeField] protected float maxSpeed;
      [SerializeField] protected float minPitch;
      [SerializeField] protected float maxPitch;
      [SerializeField] protected Transform cameraRotation;

      protected float farthestGridZPosition;
      protected float closestGridZPosition;

      private Transform CurrentTargetPosition { get; set; }
      public float SqrDistanceWithTargetPosition => CurrentTargetPosition ? (transform.position - CurrentTargetPosition.position).sqrMagnitude : 0;

      private Vector3 currentVelocity;
      private Vector3 currentRotationVelocity;
      private float pitchCurrentVelocity;

      private void Awake() {
         Instance = this;
      }

      private void Start() {
         farthestGridZPosition = HexGridController.Instance.CoordinatesToWorldPosition(new(0, HexGridController.Instance.GridRadius)).z;
         closestGridZPosition = -farthestGridZPosition;
      }

      private void Update() {
         if (CurrentTargetPosition) {
            transform.position = Vector3.SmoothDamp(transform.position, CurrentTargetPosition ? CurrentTargetPosition.position : transform.position, ref currentVelocity, smoothVelocity, maxSpeed);
            transform.forward = Vector3.SmoothDamp(transform.forward, CurrentTargetPosition ? CurrentTargetPosition.forward : transform.forward, ref currentRotationVelocity, smoothVelocity, maxSpeed);
         }

         if (BossFightInfo.Hero != null) {
            float targetedPitch = math.remap(farthestGridZPosition, closestGridZPosition, maxPitch, minPitch, BossFightInfo.Hero.transform.position.z);
            cameraRotation.localEulerAngles = new(Mathf.SmoothDampAngle(cameraRotation.localEulerAngles.x, targetedPitch, ref pitchCurrentVelocity, smoothVelocity),
               cameraRotation.localEulerAngles.y,
               cameraRotation.localEulerAngles.z);
         }
      }

      public void MoveToMenuPosition(bool snap) => MoveToPosition(menuCameraPositioning, snap);
      public void MoveToGamePosition(bool snap) => MoveToPosition(gameCameraPositioning, snap);

      private void MoveToPosition(Transform positioning, bool snap) {
         CurrentTargetPosition = positioning;

         if (CurrentTargetPosition && snap) {
            transform.position = CurrentTargetPosition.position;
            transform.rotation = CurrentTargetPosition.rotation;
            currentVelocity = Vector3.zero;
         }
      }
   }
}