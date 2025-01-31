using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.PowerUps {
   public class PowerUp : MonoBehaviour {
      [SerializeField] protected PowerUpType type;
      [SerializeField] protected MMF_Player feedbacksPlayer;

      public PowerUpType Type => type;

      public static UnityEvent<PowerUp> OnAnyCollected { get; } = new UnityEvent<PowerUp>();

      public void Collect()
      {
         OnAnyCollected.Invoke(this);
         feedbacksPlayer.PlayFeedbacks();
      }
   }
}