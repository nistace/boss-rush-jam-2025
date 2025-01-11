using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class MoveAction : APlannedAction
    {
        protected Vector3 destination;

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
            Character.NavMeshAgent.ResetPath();
        }

        public override string ToString()
        {
            return $"Move to: {destination}";
        }
    }
}
