using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class GoToControlHexAction : AAction
    {
        protected MoveAction moveAction;

        protected override EActionType Type => EActionType.GoToControlHex;

        public GoToControlHexAction(CharacterCore character, Vector3 controlHexPosition, int basePriority = 0) : base(character, basePriority)
        {
            moveAction = new(base.character, controlHexPosition, basePriority, distanceImpactOnPriority: EDistanceImpactOnPriority.LongHasHighPriority);
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

            //moveAction.ComputePriority();

            //Priority += moveAction.Priority;
        }

        public override string ToString()
        {
            return base.ToString() + $"Move to control hex at: {moveAction.Destination}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not GoToControlHexAction action)
            {
                return false;
            }

            return action.moveAction == moveAction;
        }

        public override int GetHashCode()
        {
            return moveAction.GetHashCode();
        }
    }
}
