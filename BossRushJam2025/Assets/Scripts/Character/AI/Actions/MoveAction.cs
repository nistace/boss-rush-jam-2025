using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class MoveAction : APlannedAction
    {
        protected Vector3 destination;
        protected bool cancelled;

        public override EActionStatus Status
        {
            get
            {
                bool destination_is_reached = (destination - Character.transform.position).sqrMagnitude <= 0.01f;

                if(destination_is_reached)
                {
                    return EActionStatus.Finished;
                }

                if(Character.NavMeshAgent.hasPath)
                {
                    return EActionStatus.Started;
                }

                return cancelled ? EActionStatus.Cancelled : EActionStatus.Pending;
            }
        }

        public MoveAction(Vector3 destination) : base()
        {
            this.destination = destination;
        }

        public override void Execute()
        {
            base.Execute();

            Character.NavMeshAgent.SetDestination(destination);
        }

        public override void Cancel()
        {
            base.Cancel();

            Character.NavMeshAgent.ResetPath();
            cancelled = true;
        }

        public override string ToString()
        {
            return $"Move to: {destination}";
        }
    }
}
