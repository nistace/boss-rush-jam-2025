using BossRushJam25.Character;
using BossRushJam25.Character.Bosses;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.BossFights {
   public static class BossFightInfo {
      public static CharacterCore Hero { get; private set; }
      public static BossCore Boss { get; private set; }
      public static float StartTime { get; private set; }
      public static float EndTime { get; private set; }
      public static float BattleTime => EndTime > 0 ? EndTime - StartTime : Time.time - StartTime;
      public static bool IsPlaying => StartTime > 0 && EndTime < 0;
      public static bool IsOver => StartTime > 0 && EndTime > 0;

      public static UnityEvent OnStarted { get; } = new UnityEvent();
      public static UnityEvent OnEnded { get; } = new UnityEvent();

      public static void Setup(CharacterCore hero, BossCore boss) {
         Hero = hero;
         Boss = boss;
         StartTime = -1;
         EndTime = -1;
      }

      public static void StartBattle() {
         StartTime = Time.time;
         OnStarted.Invoke();
      }

      public static void EndBattle() {
         EndTime = Time.time;
         OnEnded.Invoke();
      }
   }
}