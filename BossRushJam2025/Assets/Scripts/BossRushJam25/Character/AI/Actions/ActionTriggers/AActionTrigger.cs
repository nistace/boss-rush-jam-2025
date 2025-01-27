using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    public abstract class AActionTrigger : ScriptableObject
    {
        [SerializeField] protected bool isActive;
        //TODO: the priority could be affected by internal modifiers
        [SerializeField] protected int priority;

        protected CharacterCore character;

        public bool IsActive => isActive;

        public virtual void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        public abstract bool TryGet(out AAction action);
    }
}
