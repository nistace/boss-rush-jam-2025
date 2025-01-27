using System.Collections.Generic;
using System.Linq;
using BossRushJam25.BossFights;
using BossRushJam25.Character.AI.Actions.ActionData;
using BossRushJam25.GameControllers;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class TakeCoverAction : AAction
    {
        protected TakeCoverData data;
        protected MoveAction moveAction;
        protected GridHex targetedCover;

        protected override EActionType Type => EActionType.TakeCover;
        public override EActionStatus Status
        {
            get
            {
                if (status == EActionStatus.Started && moveAction.Status == EActionStatus.Finished)
                {
                    status = EActionStatus.Finished;
                }

                return status;
            }
        }

        public TakeCoverAction(CharacterCore character, int basePriority) : base(character, basePriority)
        {
            data = (TakeCoverData)base.character.ActionPriorityHandler.ActionDataMap[EActionType.TakeCover];

            Vector3 coverPosition = FindCoverFromOpponent();
            moveAction = new(base.character, Priority, coverPosition);
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();

            //TODO: rotate sprite towards opponent + cover idle
        }

        public override void Cancel()
        {
            base.Cancel();

            moveAction.Cancel();

            //TODO: quit cover idle?
        }

        public override void CleanUp()
        {
            base.CleanUp();

            moveAction.CleanUp();
        }

        public override void DrawPreview(float priorityValue01)
        {
            base.DrawPreview(priorityValue01);

            moveAction.DrawPreview(priorityValue01);
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();

            if(targetedCover != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(targetedCover.transform.position + Vector3.up * 1f, new Vector3(0.2f, 2f, 0.2f));
            }

            moveAction.DrawGizmos();
        }

        public override string ToString()
        {
            return $"Take cover at: {moveAction.Destination}";
        }

        private Vector3 FindCoverFromOpponent()
        {
            IEnumerable<GridHex> nearbyHexes = HexGridController.Instance.GetGridHexesInArea(character.transform.position, data.CoverDetectionRadius);

            GridHex nearestCoverHex = nearbyHexes
                .Where(hex => hex.HexContents.Any(content => GameConfig.Instance.CoverTypes.Contains(content.Type)))
                .OrderBy(hex => (hex.transform.position - character.transform.position).sqrMagnitude)
                .FirstOrDefault();

            targetedCover = nearestCoverHex;

            Vector3 threatOrigin = BossFightInfo.Boss.PatternManager.CurrentAttack.transform.position;

            if(targetedCover != null)
            {
                Vector3 hexPosition = targetedCover.transform.position;

                return GetCoverPositionOnHex(hexPosition, threatOrigin);
            }
            else
            {
                //TODO: improve this use case
                Vector3 threatPerpendicular = new(threatOrigin.z, 0f, -threatOrigin.x);

                return character.transform.position + threatPerpendicular.normalized * 3f;
            }
        }

        private Vector3 GetCoverPositionOnHex(Vector3 hexPosition, Vector3 threatOrigin)
        {
            Vector3 oppositeDirection = (hexPosition - threatOrigin).normalized;
            Vector3 targetPosition = hexPosition + oppositeDirection * data.DistanceWithCover;

            return targetPosition;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not TakeCoverAction action)
            {
                return false;
            }

            return action.targetedCover == targetedCover;
        }

        public override int GetHashCode()
        {
            return targetedCover.GetHashCode();
        }
    }
}
