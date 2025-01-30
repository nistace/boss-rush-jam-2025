using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    [CreateAssetMenu(fileName = "GoToControlHexWhenTooFar", menuName = "ActionTriggers/GoToControlHexWhenTooFar")]
    public class GoToControlHexWhenTooFar : AActionTrigger
    {
        [SerializeField] private float maxDistanceWithControlHex;
        [SerializeField] private GridHexType controlHexType;

        public float MaxDistanceWithControlHex => maxDistanceWithControlHex;

        public override bool TryGet(out AAction action)
        {
            action = null;

            Vector3 controlHexPosition = HexGridController.Instance.RequiredHexes[controlHexType].transform.position;

            float sqrDistanceWithControlHex = (controlHexPosition - character.transform.position).sqrMagnitude;

            if(sqrDistanceWithControlHex < maxDistanceWithControlHex * maxDistanceWithControlHex)
            {
                return false;
            }

            action = new MoveAction(character, controlHexPosition, priority, distanceImpactsPriority: false);

            return true;
        }
    }
}
