using System;
using BossRushJam25.GameControllers;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class CollectPowerUpAction : AAction
    {
        protected PowerUp powerUp;
        protected MoveAction moveAction;

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

        public CollectPowerUpAction(CharacterCore character, PowerUp powerUp, int basePriority = 0) : base(character, basePriority)
        {
            this.powerUp = powerUp;
            Color color = powerUp.Type switch
            {
                PowerUpType type when type.HealAmount > 0 => GameConfig.Instance.HealthPowerUpColor,
                PowerUpType type when type.DamageUpAmount > 0 => GameConfig.Instance.DamagePowerUpColor,
                PowerUpType type when type.DamageSpeedUpAmount > 0 => GameConfig.Instance.SpeedPowerUpColor,
                _ => throw new NotImplementedException()
            };
            moveAction = new(base.character, powerUp.transform.position, color);
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

        public override void DrawPreview()
        {
            base.DrawPreview();

            if (moveAction.Status != EActionStatus.Finished)
            {
                moveAction.DrawPreview();
            }
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();

            if (moveAction.Status != EActionStatus.Finished)
            {
                moveAction.DrawGizmos();
            }
        }

        public override void ComputePriority()
        {
            base.ComputePriority();

            moveAction.ComputePriority();

            Priority += moveAction.Priority;
        }

        public override string ToString()
        {
            return base.ToString() + $"Collect power up at: {moveAction.Destination}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not CollectPowerUpAction action)
            {
                return false;
            }

            return action.powerUp == powerUp;
        }

        public override int GetHashCode()
        {
            return powerUp.GetHashCode();
        }
    }
}
