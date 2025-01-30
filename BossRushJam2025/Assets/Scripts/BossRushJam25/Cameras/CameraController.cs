using BossRushJam25.BossFights;
using BossRushJam25.HexGrid;
using Unity.Mathematics;
using UnityEngine;

namespace BossRushJam25.Cameras {
   public class CameraController : MonoBehaviour {
      public static CameraController Instance { get; private set; }

      [SerializeField] protected Vector3 menuPosition;
      [SerializeField] protected Vector3 gamePosition;
      [SerializeField] protected float smoothVelocity;
      [SerializeField] protected float maxSpeed;
      [SerializeField] protected float minPitch;
      [SerializeField] protected float maxPitch;
      [SerializeField] protected Transform cameraRotation;

      protected float farthestGridZPosition;
      protected float closestGridZPosition;

      private Vector3 CurrentTargetPosition { get; set; }
      public float SqrDistanceWithTargetPosition => (transform.position - CurrentTargetPosition).sqrMagnitude;

      private Vector3 currentVelocity;
      private float pitchCurrentVelocity;

      private void Awake() {
         Instance = this;
      }

      private void Start()
      {
         farthestGridZPosition = HexGridController.Instance.CoordinatesToWorldPosition(new(0, HexGridController.Instance.GridRadius)).z;
         closestGridZPosition = -farthestGridZPosition;
      }

      private void Update() {
         transform.position = Vector3.SmoothDamp(transform.position, CurrentTargetPosition, ref currentVelocity, smoothVelocity, maxSpeed);

         if(BossFightInfo.Hero != null)
         {
            float targetedPitch = math.remap(farthestGridZPosition, closestGridZPosition, maxPitch, minPitch, BossFightInfo.Hero.transform.position.z);
            cameraRotation.localEulerAngles = new(Mathf.SmoothDampAngle(cameraRotation.localEulerAngles.x, targetedPitch, ref pitchCurrentVelocity, smoothVelocity), cameraRotation.localEulerAngles.y, cameraRotation.localEulerAngles.z);
         }
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