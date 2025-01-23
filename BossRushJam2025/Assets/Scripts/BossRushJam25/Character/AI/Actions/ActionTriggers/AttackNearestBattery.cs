using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "AttackNearestBattery", menuName = "ActionTriggers/AttackNearestBattery")]
    public class AttackNearestBattery : AActionTrigger
    {
        //TODO: make SO for each condition type to make it modular throughout all triggers
        [SerializeField] private float minHealthRatio = 0.5f;

        public override AAction Assess()
        {
            if(character.Health.HealthRatio < minHealthRatio)
            {
                return null;
            }

            if(character.BatteryDetector.NearestBatteryHex != null)
            {
                return new AttackMeleeAction(character, character.BatteryDetector.NearestBatteryHex);
            }

            return null;
        }
    }
}
