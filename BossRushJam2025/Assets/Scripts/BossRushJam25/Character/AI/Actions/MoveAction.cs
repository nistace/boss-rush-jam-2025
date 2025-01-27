using BossRushJam25.Character.AI.Actions.ActionData;
using BossRushJam25.GameControllers;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character.AI.Actions
{
    public class MoveAction : AAction
    {
        protected MoveData data;
        protected LineRenderer pathLine;
        protected NavMeshPath path = new();

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

        public MoveAction(CharacterCore character, Vector3 destination, int basePriority = 0) : base(character, basePriority)
        {
            data = (MoveData)base.character.ActionPriorityHandler.ActionDataMap[EActionType.Move];

            Destination = destination;
        }

        public override void Execute()
        {
            base.Execute();

            // character.NavMeshAgent.SetDestination(Destination);
            character.NavMeshAgent.path = path;
        }

        public override void Cancel()
        {
            base.Cancel();

            character.NavMeshAgent.ResetPath();
        }

        public override void CleanUp()
        {
            base.CleanUp();

            if(pathLine != null)
            {
                Object.Destroy(pathLine.gameObject);
            }
        }

        public override void DrawPreview(float priorityValue01)
        {
            base.DrawPreview(priorityValue01);

            if(pathLine == null)
            {
                pathLine = Object.Instantiate(GameConfig.Instance.PathLinePrefab, base.character.transform);
            }

            PathDrawer.UpdatePath(pathLine, character.transform.position, path);

            Color color = GameConfig.Instance.ActionPreviewsGradient.Evaluate(priorityValue01);
            pathLine.startColor = color;
            pathLine.endColor = color;
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

        public override void ComputePriority()
        {
            base.ComputePriority();

            ComputePath();
            Priority -= (int)ComputeSqrPathLength(path) / data.PriorityPointsPerSqrMeter;
        }

        private void ComputePath()
        {
            //TODO: really necessary? with raw positions CalculatePath don't return anything
            NavMesh.SamplePosition(character.transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas);
            Vector3 source = hit.position;
            NavMesh.SamplePosition(Destination, out hit, 10f, NavMesh.AllAreas);
            Vector3 destination = hit.position;

            NavMesh.CalculatePath(source, destination, NavMesh.AllAreas, path);
        }

        private static float ComputeSqrPathLength(NavMeshPath path)
        {
            float sqrLength = 0;

            for(int cornerIndex = 1; cornerIndex < path.corners.Length; cornerIndex++)
            {
                sqrLength += (path.corners[cornerIndex] - path.corners[cornerIndex - 1]).sqrMagnitude;
            }

            return sqrLength;
        }

        public override string ToString()
        {
            return base.ToString() + $"Move to: {Destination}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not MoveAction action)
            {
                return false;
            }

            return action.Destination == Destination;
        }

        public override int GetHashCode()
        {
            return Destination.GetHashCode();
        }
    }
}
