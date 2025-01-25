using UnityEngine;

namespace BossRushJam25.Common {
   public class KeepWorldIdentityRotation : MonoBehaviour {
      private void FixedUpdate() {
         transform.rotation = Quaternion.identity;
      }
   }
}