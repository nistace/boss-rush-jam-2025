using BossRushJam25.Character;

namespace BossRushJam25.BossFights {
   public static class BossFightInfo {
      public static CharacterCore Hero { get; private set; }

      public static void Set(CharacterCore hero) => Hero = hero;
   }
}