namespace BossRushJam25.GameControllers {
   public abstract class AbstractGameState {
      private static AbstractGameState CurrentState { get; set; }

      public abstract void Enable();
      public abstract void Disable();
      public abstract void Tick();

      public static void ChangeState(AbstractGameState newState) {
         CurrentState?.Disable();
         CurrentState = newState;
         CurrentState?.Enable();
      }

      public static void TickState() => CurrentState?.Tick();
   }
}