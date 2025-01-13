using BossRushJam25.Character;
using BossRushJam25.Character.Bosses;

namespace BossRushJam25.BossFights {
   public static class BossFightInfo {
      public static CharacterCore Hero { get; private set; }
      public static BossCore Boss { get; private set; }

      public static void SetHero(CharacterCore hero) => Hero = hero;
      public static void SetBoss(BossCore boss) => Boss = boss;
   }
}