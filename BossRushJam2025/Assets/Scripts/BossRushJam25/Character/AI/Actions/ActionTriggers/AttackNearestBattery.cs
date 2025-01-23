using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "AttackNearestBattery", menuName = "ActionTriggers/AttackNearestBattery")]
    public class AttackNearestBattery : AActionTrigger
    {
        public override AAction Assess()
        {
            if(character.BatteryDetector.NearestBatteryHex != null)
            {
                return new AttackMeleeAction(character, character.BatteryDetector.NearestBatteryHex);
            }

            return null;
        }
    }
}
