using System.Collections.Generic;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "TakeCoverOnBossPatternDetected", menuName = "ActionTriggers/TakeCoverOnBossPatternDetected")]
    public class TakeCoverOnBossPatternDetected : AActionTrigger
    {
        public override bool TryGet(out AAction action)
        {
            action = null;

            if(character.BossPatternDetector.CurrentThreateningPattern == null)
            {
                return false;
            }

            HashSet<Vector2Int> affectedHexes = character.BossPatternDetector.CurrentThreateningPattern.GetAffectedHexes();

            if(!affectedHexes.Contains(character.HexLink.LinkedHex.Coordinates))
            {
                return false;
            }

            action = new TakeCoverAction(character, priority);

            return true;
        }
    }
}
