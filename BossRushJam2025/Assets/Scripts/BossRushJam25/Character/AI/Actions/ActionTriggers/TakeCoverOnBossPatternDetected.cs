using System.Collections.Generic;
using BossRushJam25.Character.AI.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "TakeCoverOnBossPatternDetected", menuName = "ActionTriggers/TakeCoverOnBossPatternDetected")]
public class TakeCoverOnBossPatternDetected : AActionTrigger
{
    public override void Assess()
    {
        if(character.BossPatternDetector.CurrentPattern != null)
        {
            HashSet<Vector2Int> affectedHexes = character.BossPatternDetector.CurrentPattern.GetAffectedHexes();

            if(affectedHexes.Contains(character.HexLink.LinkedHex.Coordinates))
            {
                new TakeCoverAction(character).Assign();
            }
        }
    }
}
