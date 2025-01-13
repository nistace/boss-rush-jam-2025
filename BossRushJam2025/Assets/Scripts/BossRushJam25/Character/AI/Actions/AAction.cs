namespace BossRushJam25.Character.AI.Actions
{
    public abstract class AAction
    {
        protected EActionStatus status;

        public CharacterCore Character { get; protected set; }
        protected abstract EActionType Type { get; }
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
            status = EActionStatus.Pending;
        }

        public virtual void CleanUp()
        {

        }

        public virtual void DrawPreview(float priorityValue01)
        {

        }

        public virtual void DrawGizmos()
        {

        }
    }

    public enum EActionStatus
    {
        Pending = 0,
        Started = 1,
        Finished = 2,
    }

    public enum EActionType
    {
        Move = 0,
        TakeCover = 1,
        CollectPowerUp = 2,
        Dodge = 3,
    }
}
