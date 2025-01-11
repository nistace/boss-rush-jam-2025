using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class MoveAction : APlannedAction
    {
        protected Vector3 destination;

        public override EActionStatus Status
        {
            get
            {
                bool destination_is_reached = Character.NavMeshAgent.remainingDistance <= 0.01f;

                if(status == EActionStatus.Started && destination_is_reached)
                {
                    status = EActionStatus.Finished;
                }

                return status;
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
        }

        public override string ToString()
        {
            return $"Move to: {destination}";
        }
    }
}
