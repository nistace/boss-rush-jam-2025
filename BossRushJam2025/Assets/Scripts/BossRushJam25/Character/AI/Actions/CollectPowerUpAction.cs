using UnityEngine;

namespace BossRushJam25.Character.AI.Actions
{
    public class CollectPowerUpAction : APlannedAction
    {
        protected GameObject powerUp;
        protected MoveAction moveAction;

        public CollectPowerUpAction(CharacterCore character, GameObject powerUp) : base(character)
        {
            this.powerUp = powerUp;
            moveAction = new(Character, powerUp.transform.position);
        }

        public override void Execute()
        {
            base.Execute();

            moveAction.Execute();

            status = EActionStatus.Finished;
        }

        public override void Cancel()
        {
            base.Cancel();

            moveAction.Cancel();
        }

        public override string ToString()
        {
            return $"Collect power up at: {moveAction.Destination}";
        }
    }
}
