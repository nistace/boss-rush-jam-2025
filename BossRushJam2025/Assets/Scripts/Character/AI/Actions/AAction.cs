namespace BossRushJam25.Character.AI
{
    public abstract class AAction
    {
        public CharacterCore Character { get; internal set; }
        public abstract EActionStatus Status { get; }

        public virtual void Execute()
        {

        }

        public virtual void Cancel()
        {

        }
    }

    public enum EActionStatus
    {
        Pending = 0,
        Started = 1,
        Finished = 2,
        Cancelled = 3,
    }
}
