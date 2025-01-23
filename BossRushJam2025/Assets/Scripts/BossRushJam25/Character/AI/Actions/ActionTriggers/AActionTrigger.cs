using System;
using UnityEngine;

namespace BossRushJam25.Character.AI.Actions.ActionTriggers
{
    public abstract class AActionTrigger : ScriptableObject, IComparable<AActionTrigger>
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

        public abstract void Assess();

        public int CompareTo(AActionTrigger other)
        {
            return -priority.CompareTo(other.priority);
        }
    }
}
