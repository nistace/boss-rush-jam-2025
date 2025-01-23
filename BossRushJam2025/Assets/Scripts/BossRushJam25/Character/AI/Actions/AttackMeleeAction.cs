using BossRushJam25.Character.Bosses;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class AttackMeleeAction : APlannedAction
    {
        protected MoveAction moveAction;
        protected GridHex targetHex;
        protected BossAttackPattern targetBossPattern;

        protected override EActionType Type => EActionType.AttackMelee;
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

        public AttackMeleeAction(CharacterCore character, GridHex targetHex) : base(character)
        {
            this.targetHex = targetHex;
            Vector3 attackSpot = ComputeClosestAttackSpot(targetHex.transform.position);
            moveAction = new(Character, attackSpot);
            //TODO: attack
        }

        public AttackMeleeAction(CharacterCore character, BossAttackPattern targetBossPattern) : base(character)
        {
            this.targetBossPattern = targetBossPattern;
            Vector3 attackSpot = ComputeClosestAttackSpot(targetBossPattern.transform.position);
            moveAction = new(Character, attackSpot);
            //TODO: attack
        }


        private Vector3 ComputeClosestAttackSpot(Vector3 targetPosition)
        {
            return targetPosition;
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();
        }

        public override void Cancel()
        {
            base.Cancel();

            moveAction.Cancel();
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

            Vector3 targetPosition = Vector3.negativeInfinity;

            if(targetHex != null)
            {
                targetPosition = targetHex.transform.position;
            }

            if(targetBossPattern != null)
            {
                targetPosition = targetBossPattern.transform.position;
            }

            if(targetPosition != Vector3.negativeInfinity)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(targetPosition, Vector3.one * 0.2f);
            }

            moveAction.DrawGizmos();
        }

        public override string ToString()
        {
            string targetName = string.Empty;

            if(targetBossPattern != null)
            {
                targetName = "boss";
            }
            else if(targetHex != null)
            {
                targetName = "hex";
            }

            return $"Attack {targetName} at: {moveAction.Destination}";
        }
    }
}
