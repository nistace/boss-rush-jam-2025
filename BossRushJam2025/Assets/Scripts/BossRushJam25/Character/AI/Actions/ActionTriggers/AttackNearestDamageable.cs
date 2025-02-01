using BossRushJam25.Health;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = nameof(AttackNearestDamageable), menuName = "ActionTriggers/" + nameof(AttackNearestDamageable))]
    public class AttackNearestDamageable : AActionTrigger
    {
        //TODO: make SO for each condition type to make it modular throughout all triggers
        [SerializeField] private float minHealthRatio = 0.5f;

        public override bool TryGet(out AAction action)
        {
            action = null;

            if(character.Health.Ratio < minHealthRatio)
            {
                return false;
            }

            if(!character.GetCurrentDetector().TryGetNearestDamageableHex(character.transform.position, character.DamageInfo.DamageType.AsFlags(), out var nearestDamageableHex))
            {
                return false;
            }

            action = new AttackMeleeAction(character, nearestDamageableHex, priority);

            return true;
        }
    }
}
