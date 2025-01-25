using UnityEngine;

public class AlignForwardWithWorldForward : MonoBehaviour {
   [SerializeField] protected float blobg;

   private void Update() {
      Debug.Log(blobg);
      transform.rotation = Quaternion.identity;
   }
}