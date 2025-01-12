using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class MoveAction : APlannedAction
    {
        public Vector3 Destination { get; private set; }

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

        public MoveAction(CharacterCore character, Vector3 destination) : base(character)
        {
            this.Destination = destination;
        }

        public override void Execute()
        {
            base.Execute();

            Character.NavMeshAgent.SetDestination(Destination);
        }

        public override void Cancel()
        {
            base.Cancel();

            Character.NavMeshAgent.ResetPath();
        }

        public override string ToString()
        {
            return $"Move to: {Destination}";
        }
    }
}
