using System;
using UnityEngine;

namespace BossRushJam25.Health {
   [Serializable]
   public class DamageInfo {
      [SerializeField] protected int damage = 1;
      [SerializeField] protected DamageType damageType = DamageType.Physical;
      [SerializeField] protected float damageTick = -1;
      [SerializeField] protected float damageDuration = 0;

      public int Damage => damage;
      public DamageType DamageType => damageType;
      public float DamageTick => damageTick;
      public float DamageDuration => damageDuration;

      public DamageInfo() { }

      public DamageInfo(int damage, DamageType damageType, float damageTick, float damageDuration) : this() {
         this.damage = damage;
         this.damageType = damageType;
         this.damageTick = damageTick;
         this.damageDuration = damageDuration;
      }
   }
}