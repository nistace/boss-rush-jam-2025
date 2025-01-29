using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses {
   public class BossCore : MonoBehaviour {
      [SerializeField] protected BossType type;
      [SerializeField] protected BossPatternManager patternManager;

      public bool Initialized { get; private set; }
      public HealthSystem Health { get; private set; }
      public BossType Type => type;
      public BossPatternManager PatternManager => patternManager;
      public UnityEvent OnInitialized { get; } = new UnityEvent();

      public void Initialize() {
         if (Initialized) return;

         Health = new HealthSystem(type.MaxHealth, type.Vulnerabilities);
         Initialized = true;
         OnInitialized.Invoke();
      }
   }
}