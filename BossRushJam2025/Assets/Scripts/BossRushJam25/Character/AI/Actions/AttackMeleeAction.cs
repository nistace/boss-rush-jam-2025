using BossRushJam25.BossFights;
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
        protected float attackTimer;

        protected override EActionType Type => EActionType.AttackMelee;

        public AttackMeleeAction(CharacterCore character, GridHex targetHex) : base(character)
        {
            if(!targetHex.ContentsAreDamageable(character.Type.DamageInfo.DamageType))
            {
                Debug.Log("Nothing is attackable on this hex tile");

                return;
            }

            this.targetHex = targetHex;
            Vector3 attackSpot = ComputeClosestAttackSpot(targetHex.transform.position);
            moveAction = new(Character, attackSpot);
        }

        public AttackMeleeAction(CharacterCore character, BossAttackPattern targetBossPattern) : base(character)
        {
            if(!targetBossPattern.IsAttackable)
            {
                Debug.Log("Boss is not attackable");

                return;
            }

            this.targetBossPattern = targetBossPattern;
            Vector3 attackSpot = ComputeClosestAttackSpot(targetBossPattern.transform.position);
            moveAction = new(Character, attackSpot);
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

        public override void Update()
        {
            base.Update();

            TryAttack();
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

        private bool TryAttack()
        {
            if(moveAction.Status != EActionStatus.Finished)
            {
                return false;
            }

            if(!TargetIsInRange())
            {
                return false;
            }

            attackTimer += Time.deltaTime;

            if(attackTimer > Character.Type.DamageInfo.DamageTick)
            {
                DoAttack();
                attackTimer = 0f;
            }

            return true;
        }

        private void DoAttack()
        {
            if(targetHex != null)
            {
                targetHex.TryDamageContents(Character.Type.DamageInfo.Damage, Character.Type.DamageInfo.DamageType);
            }

            if(targetBossPattern != null)
            {
                BossFightInfo.Hero.Health.Damage(Character.Type.DamageInfo.Damage, Character.Type.DamageInfo.DamageType);
            }
        }

        private bool TargetIsInRange()
        {
            Vector3 targetPosition = Vector3.negativeInfinity;

            if(targetHex != null)
            {
                targetPosition = targetHex.transform.position;
            }

            if(targetBossPattern != null)
            {
                targetPosition = targetBossPattern.transform.position;
            }

            float sqrDistance = (targetPosition - Character.transform.position).sqrMagnitude;

            return targetPosition != Vector3.negativeInfinity && sqrDistance < Character.Type.SqrMaxAttackDistance;
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
                Gizmos.DrawCube(targetPosition + Vector3.up * 1f, new Vector3(0.2f, 2f, 0.2f));
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
