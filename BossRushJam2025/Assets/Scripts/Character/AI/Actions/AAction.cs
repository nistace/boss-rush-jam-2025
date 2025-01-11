namespace BossRushJam25.Character.AI
{
    public abstract class AAction
    {
        protected EActionStatus status;

        public CharacterCore Character { get; internal set; }
        public virtual EActionStatus Status => status;

        public AAction()
        {
            status = EActionStatus.Pending;
        }

        public virtual void Execute()
        {
            status = EActionStatus.Started;
        }

        public virtual void Cancel()
        {
            status = EActionStatus.Cancelled;
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
