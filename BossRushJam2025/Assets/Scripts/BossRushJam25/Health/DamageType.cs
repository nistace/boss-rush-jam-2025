﻿using System;

namespace BossRushJam25.Health {
   public enum DamageType {
      Physical = 0,
      Laser = 1,
   }

   [Flags]
   public enum DamageTypes {
      Physical = 1 << DamageType.Physical,
      Laser = 1 << DamageType.Laser,
      Nothing = 0,
      Everything = ~0
   }

   public static class DamageTypeUtils {
      public static bool Contains(this DamageTypes damageTypes, DamageType damageType) => damageTypes.Overlaps(damageType.AsFlags());
      public static bool Overlaps(this DamageTypes damageTypes, DamageTypes others) => (int)(damageTypes & others) > 0;
      public static DamageTypes AsFlags(this DamageType damageType) => (DamageTypes)(1 << (int)damageType);
   }
}