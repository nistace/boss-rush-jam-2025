using BossRushJam25.GameControllers;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character.AI.Actions
{
    public class MoveAction : APlannedAction
    {
        protected LineRenderer pathLine;

        protected override EActionType Type => EActionType.Move;
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
            Destination = destination;
            pathLine = Object.Instantiate(GameConfig.Instance.PathLinePrefab, Character.transform);
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

        public override void CleanUp()
        {
            base.CleanUp();

            Object.Destroy(pathLine);
        }

        public override void DrawPreview(float priorityValue01)
        {
            base.DrawPreview(priorityValue01);

            NavMeshPath path;

            if(Mathf.Approximately(priorityValue01, 0f))
            {
                path = Character.NavMeshAgent.path;
            }
            else
            {
                path = new();
                //TODO: really necessary? with raw positions CalculatePath don't return anything
                NavMesh.SamplePosition(Character.transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas);
                Vector3 source = hit.position;
                NavMesh.SamplePosition(Destination, out hit, 10f, NavMesh.AllAreas);
                Vector3 destination = hit.position;

                NavMesh.CalculatePath(source, destination, NavMesh.AllAreas, path);
            }

            PathDrawer.UpdatePath(pathLine, Character.transform.position, path);

            //TODO:no color is visible
            Color color = GameConfig.Instance.ActionPreviewsGradient.Evaluate(priorityValue01);
            pathLine.startColor = color;
            pathLine.startColor = color;
        }

        public override string ToString()
        {
            return $"Move to: {Destination}";
        }
    }
}
