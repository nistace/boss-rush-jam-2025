using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class CollectPowerUpAction : APlannedAction
    {
        protected GameObject powerUp;
        protected MoveAction moveAction;

        public CollectPowerUpAction(CharacterCore character, GameObject powerUp) : base(character)
        {
            this.powerUp = powerUp;
        }

        public override void Execute()
        {
            base.Execute();

            moveAction = new(Character, powerUp.transform.position);
            moveAction.Execute();
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
