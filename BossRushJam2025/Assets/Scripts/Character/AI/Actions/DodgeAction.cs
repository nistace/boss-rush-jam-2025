using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class DodgeAction : AReflexAction
    {
        protected Vector3 damageSourceDirection;

        public DodgeAction(Vector3 damageSourceDirection)
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
            Vector3 targetPosition = Character.transform.position + dodgeDirection * 1f;
            Character.transform.position = targetPosition;

            status = EActionStatus.Finished;
        }

        public override string ToString()
        {
            return $"Dodge from: {damageSourceDirection}";
        }
    }
}
