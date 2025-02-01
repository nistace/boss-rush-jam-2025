using UnityEngine;

namespace BossRushJam25.Common {
   public class AlignForwardWithWorldForward : MonoBehaviour {
      private void Update() => transform.rotation = Quaternion.identity;
   }
}