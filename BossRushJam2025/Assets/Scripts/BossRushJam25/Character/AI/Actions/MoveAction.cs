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
                bool destination_is_reached = !character.NavMeshAgent.pathPending && character.NavMeshAgent.remainingDistance <= 0.01f;

                if(status == EActionStatus.Started && destination_is_reached)
                {
                    status = EActionStatus.Finished;
                    character.NavMeshAgent.ResetPath();
                }

                return status;
            }
        }

        public MoveAction(CharacterCore character, Vector3 destination) : base(character)
        {
            Destination = destination;
            pathLine = Object.Instantiate(GameConfig.Instance.PathLinePrefab, base.character.transform);
        }

        public override void Execute()
        {
            base.Execute();

            character.NavMeshAgent.SetDestination(Destination);
        }

        public override void Cancel()
        {
            base.Cancel();

            character.NavMeshAgent.ResetPath();
        }

        public override void CleanUp()
        {
            base.CleanUp();

            Object.Destroy(pathLine.gameObject);
        }

        public override void DrawPreview(float priorityValue01)
        {
            base.DrawPreview(priorityValue01);

            NavMeshPath path;

            if(Mathf.Approximately(priorityValue01, 0f))
            {
                path = character.NavMeshAgent.path;
            }
            else
            {
                path = new();
                //TODO: really necessary? with raw positions CalculatePath don't return anything
                NavMesh.SamplePosition(character.transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas);
                Vector3 source = hit.position;
                NavMesh.SamplePosition(Destination, out hit, 10f, NavMesh.AllAreas);
                Vector3 destination = hit.position;

                NavMesh.CalculatePath(source, destination, NavMesh.AllAreas, path);
            }

            PathDrawer.UpdatePath(pathLine, character.transform.position, path);

            //TODO:no color is visible
            Color color = GameConfig.Instance.ActionPreviewsGradient.Evaluate(priorityValue01);
            pathLine.startColor = color;
            pathLine.startColor = color;
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();

            if (character.NavMeshAgent.hasPath)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(character.NavMeshAgent.destination, 0.2f);
            }
        }

        public override string ToString()
        {
            return $"Move to: {Destination}";
        }
    }
}
