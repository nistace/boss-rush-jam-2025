using UnityEngine;

namespace BossRushJam25.Health {
   public class DamageRunner {
      private readonly DamageInfo info;
      private float time = -1;
      private float delayBeforeNextDamageDealt;

      public bool HasStarted => time >= 0;
      public int DamageDealtThisFrame => info.Damage * Mathf.FloorToInt(delayBeforeNextDamageDealt / info.DamageTick);

      public DamageRunner(DamageInfo info) {
         this.info = info;
         delayBeforeNextDamageDealt = info.DamageTick + float.Epsilon;
      }

      public void Reset() => time = -1;

      public bool Done() => time > info.DamageDuration;

      public void Continue(float deltaTime) {
         time = Mathf.Max(time, 0) + deltaTime;
         delayBeforeNextDamageDealt = delayBeforeNextDamageDealt % info.DamageTick + deltaTime;
      }
   }
}