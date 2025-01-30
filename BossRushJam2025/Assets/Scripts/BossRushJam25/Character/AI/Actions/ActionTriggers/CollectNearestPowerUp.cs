using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "CollectNearestPowerUp", menuName = "ActionTriggers/CollectNearestPowerUp")]
    public class CollectNearestPowerUp : AActionTrigger
    {
        public override bool TryGet(out AAction action)
        {
            action = null;

            if (!character.HexContentDetector.TryGetNearestPowerUp(character.transform.position, out var nearestPowerUp))
            {
                return false;
            }

            action = new CollectPowerUpAction(character, nearestPowerUp, priority);

            return true;
        }
    }
}
