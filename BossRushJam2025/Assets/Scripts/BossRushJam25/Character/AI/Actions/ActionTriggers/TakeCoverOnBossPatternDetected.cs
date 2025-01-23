using System.Collections.Generic;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "TakeCoverOnBossPatternDetected", menuName = "ActionTriggers/TakeCoverOnBossPatternDetected")]
    public class TakeCoverOnBossPatternDetected : AActionTrigger
    {
        public override AAction Assess()
        {
            if(character.BossPatternDetector.CurrentThreateningPattern != null)
            {
                HashSet<Vector2Int> affectedHexes = character.BossPatternDetector.CurrentThreateningPattern.GetAffectedHexes();

                if(affectedHexes.Contains(character.HexLink.LinkedHex.Coordinates))
                {
                    return new TakeCoverAction(character);
                }
            }

            return null;
        }
    }
}
