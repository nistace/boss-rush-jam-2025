namespace BossRushJam25.Character.AI
{
    public abstract class AReflexAction : AAction
    {
        public AReflexAction(CharacterCore character) : base(character)
        {

        }

        public override void Assign()
        {
            base.Assign();

            Character.ActionPriorityHandler.ForceAction(this);
        }
    }
}
