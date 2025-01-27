using System;
using BossRushJam25.BossFights;
using BossRushJam25.Character.Bosses;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class AttackMeleeAction : AAction
    {
        protected MoveAction moveAction;
        protected GridHex targetHex;
        protected BossAttackPattern targetBossPattern;
        protected float attackTimer;

        protected override EActionType Type => EActionType.AttackMelee;

        public AttackMeleeAction(CharacterCore character, int basePriority, GridHex targetHex) : base(character, basePriority)
        {
            if(!targetHex.ContentsAreDamageable(character.Type.DamageInfo.DamageType))
            {
                Debug.Log("Nothing is attackable on this hex tile");

                return;
            }

            this.targetHex = targetHex;
            Vector3 attackSpot = ComputeClosestAttackSpot(targetHex.transform.position);
            moveAction = new(base.character, Priority, attackSpot);
        }

        public AttackMeleeAction(CharacterCore character, int basePriority, BossAttackPattern targetBossPattern) : base(character, basePriority)
        {
            if(!targetBossPattern.IsAttackable)
            {
                Debug.Log("Boss is not attackable");

                return;
            }

            this.targetBossPattern = targetBossPattern;
            Vector3 attackSpot = ComputeClosestAttackSpot(targetBossPattern.transform.position);
            moveAction = new(base.character, Priority, attackSpot);
        }

        private Vector3 ComputeClosestAttackSpot(Vector3 targetPosition)
        {
            return targetPosition;
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();
            attackTimer = character.Type.DamageInfo.DamageTick;
        }

        public override void Update()
        {
            base.Update();

            if(targetBossPattern == null
                && targetHex != null
                && !targetHex.ContentsAreDamageable(character.Type.DamageInfo.DamageType)
                )
            {
                status = EActionStatus.Finished;

                return;
            }

            if(moveAction.Status == EActionStatus.Finished
                && !TargetIsInRange()
                )
            {
                status = EActionStatus.Finished;

                return;
            }

            bool isAttacking = TryAttack();
            character.Animator.SetAttackParameter(isAttacking);
        }

        public override void Cancel()
        {
            base.Cancel();

            moveAction.Cancel();
            character.Animator.SetAttackParameter(false);
        }

        public override void CleanUp()
        {
            base.CleanUp();

            moveAction.CleanUp();
            character.Animator.SetAttackParameter(false);
        }

        //TODO: move in BatteryDetector if we want the hero to automatically attack the batteries when only passing by
        private bool TryAttack()
        {
            if(moveAction.Status != EActionStatus.Finished
                || !TargetIsInRange()
                )
            {
                return false;
            }

            attackTimer += Time.deltaTime;

            if(attackTimer > character.Type.DamageInfo.DamageTick)
            {
                DoAttack();
                attackTimer -= character.Type.DamageInfo.DamageTick;
            }

            return true;
        }

        private void DoAttack()
        {
            if(targetHex != null)
            {
                targetHex.TryDamageContents(character.Type.DamageInfo.Damage, character.Type.DamageInfo.DamageType);
            }

            if(targetBossPattern != null)
            {
                BossFightInfo.Hero.Health.Damage(character.Type.DamageInfo.Damage, character.Type.DamageInfo.DamageType);
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

            float sqrDistance = (targetPosition - character.transform.position).sqrMagnitude;

            return targetPosition != Vector3.negativeInfinity && sqrDistance < character.Type.SqrMaxAttackDistance;
        }

        public override void DrawPreview(float priorityValue01)
        {
            base.DrawPreview(priorityValue01);

            if(moveAction.Status != EActionStatus.Finished)
            {
                moveAction.DrawPreview(priorityValue01);
            }
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

            if(moveAction.Status != EActionStatus.Finished)
            {
                moveAction.DrawGizmos();
            }
        }

        public override void ComputePriority()
        {
            base.ComputePriority();

            //TODO: compute path and modify priority;
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

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not AttackMeleeAction action)
            {
                return false;
            }

            return action.targetHex == targetHex && action.targetBossPattern == targetBossPattern;
        }

        public override int GetHashCode()
        {
            return targetHex.GetHashCode() * targetBossPattern.GetHashCode();
        }
    }
}
