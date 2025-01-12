using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class CollectPowerUpAction : APlannedAction
    {
        protected GameObject powerUp;
        protected MoveAction moveAction;

        protected override EActionType Type => EActionType.TakeCover;

        public CollectPowerUpAction(CharacterCore character, GameObject powerUp) : base(character)
        {
            this.powerUp = powerUp;
            moveAction = new(Character, powerUp.transform.position);
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();

            status = EActionStatus.Finished;
        }

        public override void Cancel()
        {
            base.Cancel();

            moveAction.Cancel();
        }

        public override void CleanUp()
        {
            base.CleanUp();

            moveAction.CleanUp();
        }

        public override void DrawPreview(float priorityValue01)
        {
            base.DrawPreview(priorityValue01);

            moveAction.DrawPreview(priorityValue01);
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();

            moveAction.DrawGizmos();
        }

        public override string ToString()
        {
            return $"Collect power up at: {moveAction.Destination}";
        }
    }
}
