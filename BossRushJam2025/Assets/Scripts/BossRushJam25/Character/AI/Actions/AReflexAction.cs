namespace BossRushJam25.Character.AI.Actions
{
    public abstract class AReflexAction : AAction
    {
        public AReflexAction(CharacterCore character) : base(character)
        {

        }

        public override void Assign()
        {
            base.Assign();

            character.ActionPriorityHandler.ForceAction(this);
        }
    }
}
