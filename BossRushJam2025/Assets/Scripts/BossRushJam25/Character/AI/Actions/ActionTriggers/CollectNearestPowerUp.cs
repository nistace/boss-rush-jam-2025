using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "CollectNearestPowerUp", menuName = "ActionTriggers/CollectNearestPowerUp")]
    public class CollectNearestPowerUp : AActionTrigger
    {
        public override AAction Assess()
        {
            if(character.PowerUpsDetector.NearestPowerUp != null)
            {
                return new CollectPowerUpAction(character, character.PowerUpsDetector.NearestPowerUp.gameObject);
            }

            return null;
        }
    }
}
