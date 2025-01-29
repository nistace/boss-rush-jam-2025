using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "TakeCoverOnBossPatternDetected", menuName = "ActionTriggers/TakeCoverOnBossPatternDetected")]
    public class TakeCoverOnBossPatternDetected : AActionTrigger
    {
        public override bool TryGet(out AAction action)
        {
            action = null;

            if(!character.HexLink.LinkedHex.IsTargeted)
            {
                return false;
            }

            action = new TakeCoverAction(character, priority);

            return true;
        }
    }
}
