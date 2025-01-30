using System;

namespace BossRushJam25.Character.AI.Actions
{
    public abstract class AAction : IComparable<AAction>
    {
        protected EActionStatus status;
        protected CharacterCore character;
        protected int basePriority;

        protected abstract EActionType Type { get; }
        public virtual EActionStatus Status => status;
        public int Priority { get; protected set; }
        public bool IsForced { get; set; }

        public AAction(CharacterCore character, int basePriority)
        {
            this.character = character;
            this.basePriority = basePriority;
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
            Priority = basePriority;

            if(IsForced)
            {
                Priority += 10000;
            }
        }

        public override string ToString()
        {
            return IsForced ? "Forced " : Priority.ToString() + "P ";
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
