using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class DodgeAction : AAction
    {
        protected Vector3 damageSourceDirection;

        protected override EActionType Type => EActionType.Dodge;

        public DodgeAction(CharacterCore character, int basePriority, Vector3 damageSourceDirection) : base(character, basePriority)
        {
            this.damageSourceDirection = damageSourceDirection;
        }

        public override void Execute()
        {
            base.Execute();

            Vector3 dodgeDirection = Vector3.Cross(Vector3.up, damageSourceDirection).normalized;
            DoDodgeAnimation(dodgeDirection);
        }

        public override void Cancel()
        {
            base.Cancel();

            //TODO: cancel animation
        }

        private void DoDodgeAnimation(Vector3 dodgeDirection)
        {
            //TODO: animate
            Vector3 targetPosition = character.transform.position + dodgeDirection * 1f;
            character.transform.position = targetPosition;

            status = EActionStatus.Finished;
        }

        public override string ToString()
        {
            return base.ToString() + $"Dodge from: {damageSourceDirection}";
        }
    }
}
