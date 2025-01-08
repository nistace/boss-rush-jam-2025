using UnityEngine;
namespace BossRushJam25 {
   public class GridHexContent : MonoBehaviour {

      [SerializeField] protected GridHexContentType type;

      public GridHexContentType Type => type;
   }
}
