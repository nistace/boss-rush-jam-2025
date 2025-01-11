namespace BossRushJam25.Character.AI
{
    public abstract class AAction
    {
        public CharacterCore Character { get; internal set; }

        public virtual void Execute()
        {

        }

        public virtual void Cancel()
        {

        }
    }
}
