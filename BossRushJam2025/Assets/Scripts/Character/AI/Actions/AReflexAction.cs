namespace BossRushJam25.Character.AI
{
    public abstract class AReflexAction : AAction
    {
        public override void Execute()
        {
            base.Execute();

            Character.ActionPriorityHandler.CancelAllActions();
        }
    }
}
