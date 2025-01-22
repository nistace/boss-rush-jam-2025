using System.Collections.Generic;
using BossRushJam25.Character.AI.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "TakeCoverOnBossPatternDetected", menuName = "ActionTriggers/TakeCoverOnBossPatternDetected")]
public class TakeCoverOnBossPatternDetected : AActionTrigger
{
    public override void Assess()
    {
        if(character.BossPatternDetector.CurrentThreateningPattern != null)
        {
            HashSet<Vector2Int> affectedHexes = character.BossPatternDetector.CurrentThreateningPattern.GetAffectedHexes();

            if(affectedHexes.Contains(character.HexLink.LinkedHex.Coordinates))
            {
                new TakeCoverAction(character).Assign();
            }
        }
    }
}
