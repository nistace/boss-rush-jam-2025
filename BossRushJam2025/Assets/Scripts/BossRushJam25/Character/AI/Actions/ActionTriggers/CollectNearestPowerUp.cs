using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "CollectNearestPowerUp", menuName = "ActionTriggers/CollectNearestPowerUp")]
    public class CollectNearestPowerUp : AActionTrigger
    {
        public override bool TryGet(out AAction action)
        {
            action = null;

            if(character.PowerUpsDetector.NearestPowerUp == null)
            {
                return false;
            }

            action = new CollectPowerUpAction(character, priority, character.PowerUpsDetector.NearestPowerUp.gameObject);

            return true;
        }
    }
}
