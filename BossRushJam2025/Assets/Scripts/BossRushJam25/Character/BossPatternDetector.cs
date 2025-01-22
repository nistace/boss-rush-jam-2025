using BossRushJam25.BossFights;
using BossRushJam25.Character.Bosses;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character
{
    public class BossPatternDetector : MonoBehaviour
    {
        private CharacterCore character;

        public BossAttackPattern CurrentThreateningPattern { get; private set; }

        public UnityEvent OnDetectedSuccessfulAttackChanged { get; } = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void Update()
        {
            BossAttackPattern currentPattern = BossFightInfo.Boss.PatternManager.CurrentAttack;

            if(currentPattern != null && currentPattern.GetAffectedHexes().Contains(character.HexLink.LinkedHex.Coordinates))
            {
                if(CurrentThreateningPattern != currentPattern)
                {
                    CurrentThreateningPattern = currentPattern;
                    OnDetectedSuccessfulAttackChanged.Invoke();
                }
            }
            else
            {
                CurrentThreateningPattern = null;
            }
        }
    }
}
