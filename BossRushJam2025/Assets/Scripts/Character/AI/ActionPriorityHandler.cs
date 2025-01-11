using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class ActionPriorityHandler : MonoBehaviour
    {
        protected CharacterCore character;
        protected AAction currentAction;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
            //ChooseNextAction();
        }

        public void ChooseNextAction()
        {
            currentAction = new MoveAction(Vector3.one * 5)
            {
                Character = character
            };
        }

        public void AddNextAction(AAction action)
        {
            currentAction = action;
            currentAction.Character = character;
            currentAction.Execute();
        }

        public void CancelCurrentAction()
        {
            if(currentAction != null)
            {
                currentAction.Cancel();
            }
        }

        public void CancelAllActions()
        {
            //TODO: empty queue
            CancelCurrentAction();
        }
    }
}
