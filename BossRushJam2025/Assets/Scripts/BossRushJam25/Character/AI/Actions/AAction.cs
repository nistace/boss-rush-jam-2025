namespace BossRushJam25.Character.AI.Actions
{
    public abstract class AAction
    {
        protected EActionStatus status;

        public CharacterCore Character { get; protected set; }
        public virtual EActionStatus Status => status;

        public AAction(CharacterCore character)
        {
            Character = character;
        }

        public virtual void Assign()
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

        public virtual void CleanUp()
        {

        }

        public void Reset()
        {
            Cancel();
            CleanUp();
            status = EActionStatus.Pending;
        }

        public virtual void DrawPreview(float priorityValue01)
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
