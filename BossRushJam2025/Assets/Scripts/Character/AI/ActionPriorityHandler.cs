using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class ActionPriorityHandler : MonoBehaviour
    {
        [SerializeField] private bool displayDebugGUI;

        protected CharacterCore character;
        protected AAction currentAction;
        protected Queue<AAction> pendingActions = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        public void ExecuteNextAction()
        {
            if(pendingActions.TryDequeue(out AAction nextAction))
            {
                currentAction = nextAction;
                currentAction.Execute();
            }
            else
            {
                currentAction = null;
            }
        }

        public void AddActionToQueue(AAction action)
        {
            action.Character = character;
            pendingActions.Enqueue(action);
        }

        public void CancelCurrentAction()
        {
            if(currentAction != null)
            {
                currentAction.Cancel();
                currentAction = null;
            }
        }

        public void CancelAllActions()
        {
            CancelCurrentAction();
            pendingActions.Clear();
        }

        private void Update()
        {
            if(currentAction == null
                || currentAction.Status == EActionStatus.Finished
                || currentAction.Status == EActionStatus.Cancelled
                )
            {
                ExecuteNextAction();
            }
        }

        private void OnGUI()
        {
            if(!displayDebugGUI)
            {
                return;
            }

            GUIStyle currentActionStyle = new(GUI.skin.box) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
            currentActionStyle.normal.textColor = Color.yellow;

            GUI.Box(new Rect(10, 10, 400, 50), currentAction != null ? currentAction.ToString() : "No action assigned", currentActionStyle);

            if(pendingActions.Count > 0 )
            {
                GUIStyle pendingActionsStyle = new(GUI.skin.box) { fontSize = 25, alignment = TextAnchor.UpperLeft };
                pendingActionsStyle.normal.textColor = Color.white;
                StringBuilder builder = new();

                foreach(AAction action in pendingActions)
                {
                    builder.AppendLine(action.ToString());
                }

                GUI.Box(new Rect(10, 70, 400, 150), builder.ToString(), pendingActionsStyle);
            }
        }
    }
}
