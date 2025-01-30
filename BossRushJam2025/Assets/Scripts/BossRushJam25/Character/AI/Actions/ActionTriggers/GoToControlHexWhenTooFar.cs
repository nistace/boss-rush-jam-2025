using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "GoToControlHexWhenTooFar", menuName = "ActionTriggers/GoToControlHexWhenTooFar")]
    public class GoToControlHexWhenTooFar : AActionTrigger
    {
        [SerializeField] private GridHexType controlHexType;

        public override bool TryGet(out AAction action)
        {
            Vector3 controlHexPosition = HexGridController.Instance.RequiredHexes[controlHexType].transform.position;

            action = new GoToControlHexAction(character, controlHexPosition, priority);

            return true;
        }
    }
}
