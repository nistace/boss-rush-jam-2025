namespace BossRushJam25.Character.AI.Actions
{
    public abstract class APlannedAction : AAction
    {
        public APlannedAction(CharacterCore character) : base(character)
        {

        }

        public override void Assign()
        {
            base.Assign();

            Character.ActionPriorityHandler.ForceAction(this);
        }
    }
}
