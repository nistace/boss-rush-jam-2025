using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class TakeCoverAction : APlannedAction
    {
        protected MoveAction moveAction;

        public TakeCoverAction(CharacterCore character) : base(character)
        {
            Vector3 coverPosition = FindCoverFromOpponent();

            moveAction = new(Character, coverPosition);
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();

            //TODO: rotate sprite towards opponent + cover idle

            status = EActionStatus.Finished;
        }

        public override void Cancel()
        {
            base.Cancel();

            moveAction.Cancel();

            //TODO: quit cover idle?
        }

        public override string ToString()
        {
            return $"Take cover at: {moveAction.Destination}";
        }

        private Vector3 FindCoverFromOpponent()
        {
            //TODO: find nearby walls instead
            Vector3 opponentPosition = Character.Opponent.transform.position;
            Vector3 oppositeDirection = (Character.transform.position - opponentPosition).normalized;
            Vector3 targetPosition = HexGridController.Instance.GetRandomPositionOnNavMesh(source: Character.transform.position, direction: oppositeDirection, amplitude: 60f);

            return targetPosition;
        }
    }
}
