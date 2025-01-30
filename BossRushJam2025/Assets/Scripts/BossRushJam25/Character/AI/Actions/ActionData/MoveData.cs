using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionData
{
    [CreateAssetMenu(fileName = "MoveData", menuName = "Actions/MoveData")]
    public class MoveData : AActionData
    {
        [SerializeField] private int priorityPointsPerMeter = 1;

        public int PriorityPointsPerMeter => priorityPointsPerMeter;
    }
}
