using System;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "AttackNearestBattery", menuName = "ActionTriggers/AttackNearestBattery")]
    public class AttackNearestBattery : AActionTrigger
    {
        //TODO: make SO for each condition type to make it modular throughout all triggers
        [SerializeField] private float minHealthRatio = 0.5f;

        public override bool TryGet(out AAction action)
        {
            action = null;

            if(character.Health.HealthRatio < minHealthRatio)
            {
                return false;
            }

            if(character.BatteryDetector.NearestBatteryHex == null)
            {
                return false;
            }

            action = new AttackMeleeAction(character, character.BatteryDetector.NearestBatteryHex, priority);

            return true;
        }
    }
}
