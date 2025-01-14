using System.Collections.Generic;
using BossRushJam25.BossFights;
using BossRushJam25.Character.Bosses;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character
{
    public class BossPatternDetector : MonoBehaviour
    {
        private CharacterCore character;

        public BossAttackPattern CurrentPattern { get; private set; }

        public UnityEvent OnSuccessfulAttackDetected { get; } = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
            BossFightInfo.Boss.PatternManager.OnAttackStarted.AddListener(BossPatternManager_OnAttackExecuting);
        }

        private void BossPatternManager_OnAttackExecuting()
        {
            CurrentPattern = BossFightInfo.Boss.PatternManager.CurrentAttack;

            if(CurrentPattern != null)
            {
                HashSet<Vector2Int> affectedHexes = CurrentPattern.GetAffectedHexes();

                if(affectedHexes.Contains(character.HexLink.LinkedHex.Coordinates))
                {
                    OnSuccessfulAttackDetected.Invoke();
                }
            }
        }
    }
}
