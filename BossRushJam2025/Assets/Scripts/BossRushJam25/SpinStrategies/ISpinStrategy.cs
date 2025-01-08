using UnityEngine;
namespace BossRushJam25.SpinStrategies {
   public interface ISpinStrategy {
      void Initialize();
      void Tick();
   }
}
