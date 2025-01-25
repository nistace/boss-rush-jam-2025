using UnityEngine;

namespace BossRushJam25.Common {
   public class AlignWithCameraForward : MonoBehaviour {
      protected enum EStrategy {
         Clamp = 0,
         Remap = 1,
      }

      [SerializeField] protected EStrategy strategy;
      [SerializeField] protected float minAngleWithForward = 30;
      [SerializeField] protected float maxAngleWithForward = 60;

      private void Update() {
         var cameraForward = Camera.main.transform.forward;

         var angleBetweenForwards = Vector3.Angle(cameraForward, Vector3.forward);

         var selfAngleWithForward = strategy switch {
            EStrategy.Clamp => Mathf.Clamp(angleBetweenForwards, minAngleWithForward, maxAngleWithForward),
            EStrategy.Remap => minAngleWithForward + Mathf.InverseLerp(0, 90, angleBetweenForwards) * (maxAngleWithForward - minAngleWithForward),
            _ => angleBetweenForwards
         };

         transform.forward = Vector3.Slerp(Vector3.forward, Vector3.down, selfAngleWithForward / 90);
      }
   }
}