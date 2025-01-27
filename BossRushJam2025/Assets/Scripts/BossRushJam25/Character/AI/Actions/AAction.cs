using System;

namespace BossRushJam25.Character.AI.Actions
{
    public abstract class AAction : IComparable<AAction>
    {
        protected EActionStatus status;

        protected CharacterCore character;
        protected abstract EActionType Type { get; }
        public virtual EActionStatus Status => status;
        public int Priority { get; protected set; }

        public AAction(CharacterCore character, int basePriority)
        {
            this.character = character;
            Priority = basePriority;
        }

        public virtual void Execute()
        {
            status = EActionStatus.Started;
        }

        public virtual void Update()
        {

        }

        public virtual void Cancel()
        {
            status = EActionStatus.NotStarted;
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

        public virtual void ComputePriority()
        {

        }

        public override string ToString()
        {
            return Priority.ToString() + " - ";
        }

        public virtual int CompareTo(AAction other)
        {
            return -Priority.CompareTo(other.Priority);
        }
    }

    public enum EActionStatus
    {
        NotStarted = 0,
        Started = 1,
        Finished = 2,
    }

    public enum EActionType
    {
        Move = 0,
        TakeCover = 1,
        CollectPowerUp = 2,
        Dodge = 3,
        AttackMelee = 4,
    }
}
