using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.PowerUps {
   public class PowerUp : MonoBehaviour {
      [SerializeField] protected PowerUpType type;

      public PowerUpType Type => type;

      public static UnityEvent<PowerUp> OnAnyCollected { get; } = new UnityEvent<PowerUp>();

      public void Destroy()
      {
         OnAnyCollected.Invoke(this);
         Destroy(gameObject);
      }
   }
}