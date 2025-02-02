using BossRushJam25.Character.AI.Actions.ActionData;
using BossRushJam25.GameControllers;
using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character.AI.Actions
{
    public class MoveAction : AAction
    {
        protected MoveData data;
        protected LineRenderer pathLine;
        protected NavMeshPath path = new();
        protected bool distanceImpactsPriority;

        protected override EActionType Type => EActionType.Move;
        protected bool targetIsHex;
        protected Vector2Int hexCoordinates;
        protected Color color;

        public Vector3 Destination { get; private set; }

        public override EActionStatus Status
        {
            get
            {
                bool destination_is_reached = !character.NavMeshAgent.pathPending && character.NavMeshAgent.remainingDistance <= 0.1f;

                if(status == EActionStatus.Started && destination_is_reached)
                {
                    status = EActionStatus.Finished;
                    character.NavMeshAgent.ResetPath();
                }

                return status;
            }
        }

        //HACK: only used to get close to a non accessible hex
        public MoveAction(CharacterCore character, GridHex hex, Color color, int basePriority = 0, bool distanceImpactsPriority = true) : this(character, HexGridController.Instance.GetClosestPointOnHexBorderFrom(character.transform.position, hex), color, basePriority, distanceImpactsPriority)
        {
            //this ensures that Equals will always return true if the same hex is targeted
            targetIsHex = true;
            hexCoordinates = hex.Coordinates;
        }

        public MoveAction(CharacterCore character, Vector3 destination, Color color, int basePriority = 0, bool distanceImpactsPriority = true) : base(character, basePriority)
        {
            data = (MoveData)base.character.ActionPriorityHandler.ActionDataMap[EActionType.Move];

            Destination = destination;
            this.color = color;
            this.distanceImpactsPriority = distanceImpactsPriority;
            targetIsHex = false;
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

        public override void DrawPreview()
        {
            if (pathLine == null)
            {
                pathLine = Object.Instantiate(GameConfig.Instance.PathLinePrefab, character.transform);
            }

            PathDrawer.UpdatePath(pathLine, character.transform.position, path);

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

            if(distanceImpactsPriority)
            {
                Priority -= (int)ComputeSqrPathLength(path) / data.PriorityPointsPerSqrMeter;
            }
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

            return action.targetIsHex == targetIsHex && (targetIsHex ? action.hexCoordinates == hexCoordinates : action.Destination == Destination);
        }

        public override int GetHashCode()
        {
            return Destination.GetHashCode();
        }
    }
}
