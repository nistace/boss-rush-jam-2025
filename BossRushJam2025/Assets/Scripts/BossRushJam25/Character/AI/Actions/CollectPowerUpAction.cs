using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class CollectPowerUpAction : AAction
    {
        protected GameObject powerUp;
        protected MoveAction moveAction;

        protected override EActionType Type => EActionType.TakeCover;
        public override EActionStatus Status
        {
            get
            {
                if (status == EActionStatus.Started && moveAction.Status == EActionStatus.Finished)
                {
                    status = EActionStatus.Finished;
                }

                return status;
            }
        }

        public CollectPowerUpAction(CharacterCore character, GameObject powerUp, int basePriority = 0) : base(character, basePriority)
        {
            this.powerUp = powerUp;
            moveAction = new(base.character, powerUp.transform.position);
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();
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

            if (moveAction.Status != EActionStatus.Finished)
            {
                moveAction.DrawPreview(priorityValue01);
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();

            if (moveAction.Status != EActionStatus.Finished)
            {
                moveAction.DrawGizmos();
            }
        }

        public override void ComputePriority()
        {
            base.ComputePriority();

            moveAction.ComputePriority();

            Priority += moveAction.Priority;
        }

        public override string ToString()
        {
            return base.ToString() + $"Collect power up at: {moveAction.Destination}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not CollectPowerUpAction action)
            {
                return false;
            }

            return action.powerUp == powerUp;
        }

        public override int GetHashCode()
        {
            return powerUp.GetHashCode();
        }
    }
}
