using System;
using UnityEngine;

namespace BossRushJam25.PowerUps {
   [Serializable]
   public class PowerUpType {
      [SerializeField] protected float spawnZOffset = 1;
      [SerializeField] protected int healAmount;
      [SerializeField] protected int damageUpAmount;
      [SerializeField] protected float damageSpeedUpAmount;

      public float SpawnZOffset => spawnZOffset;
      public int HealAmount => healAmount;
      public int DamageUpAmount => damageUpAmount;
      public float DamageSpeedUpAmount => damageSpeedUpAmount;
   }
}