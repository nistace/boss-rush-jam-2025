using UnityEngine;

public class AlignWithCameraForward : MonoBehaviour {
   [SerializeField] protected bool keepVertical;
   private void Update() {
      var forward = Camera.main.transform.forward;
      if (keepVertical) forward.y = 0;

      transform.forward = forward;
   }
}
