namespace BossRushJam25.SpinStrategies {
   public interface ISpinStrategy {
      void Enable();
      void Disable();
      void Tick();
      void Reset();
   }
}