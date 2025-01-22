using BossRushJam25.Character.AI.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectNearestPowerUp", menuName = "ActionTriggers/CollectNearestPowerUp")]
public class CollectNearestPowerUp : AActionTrigger
{
    public override void Assess()
    {
        if(character.PowerUpsDetector.NearestPowerUp != null)
        {
            new CollectPowerUpAction(character, character.PowerUpsDetector.NearestPowerUp.gameObject).Assign();
        }
    }
}
