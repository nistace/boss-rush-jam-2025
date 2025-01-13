using UnityEngine;

namespace BossRushJam25.Character.Bosses
{
    public class BossCore : MonoBehaviour
    {
        [SerializeField] protected CharacterType type;
        [SerializeField] protected BossPatternManager patternManager;

        public HealthSystem Health { get; private set; }
        public CharacterType Type => type;
        public BossPatternManager PatternManager => patternManager;

        public void Initialize()
        {
            Health = new HealthSystem(type.MaxHealth);
        }
    }
}
