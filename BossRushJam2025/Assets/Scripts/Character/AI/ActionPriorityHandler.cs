using System;
using System.Text;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class ActionPriorityHandler : MonoBehaviour
    {
        [SerializeField] private bool displayDebugGUI;

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

        private void OnGUI()
        {
            if(!displayDebugGUI)
            {
                return;
            }

            StringBuilder builder = new();
            builder.Append(currentAction);

            GUI.Box(new Rect(10, 10, 400, 200), builder.ToString(), new GUIStyle(GUI.skin.box) { fontSize = 25, alignment = TextAnchor.UpperLeft });
        }
    }
}
