using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BossRushJam25.Character.AI
{
    //TODO: add queue limit
    public class ActionPriorityHandler : MonoBehaviour
    {
        [SerializeField] private bool displayDebugGUI;

        protected CharacterCore character;
        protected AReflexAction activeReflexAction;
        protected List<APlannedAction> plannedActions = new();

        protected APlannedAction activePlannedAction => plannedActions.Count > 0 ? plannedActions[^1] : null;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        public void ExecuteReflexAction(AReflexAction action)
        {
            activeReflexAction = action;
            activeReflexAction.Character = character;
            activeReflexAction.Execute();
        }

        public void PlanAction(APlannedAction action)
        {
            plannedActions.Insert(0, action);
            action.Character = character;
        }

        public void CancelActiveAction()
        {
            if(activePlannedAction != null && activePlannedAction.Status == EActionStatus.Started)
            {
                activePlannedAction.Cancel();
            }
        }

        public void CancelAllActions()
        {
            CancelActiveAction();
            plannedActions.Clear();
        }

        private void ProcessActivePlannedAction()
        {
            if (activePlannedAction == null)
            {
                return;
            }

            switch (activePlannedAction.Status)
            {
                case EActionStatus.Pending:
                {
                    activePlannedAction.Execute();

                    break;
                }
                case EActionStatus.Finished:
                case EActionStatus.Cancelled:
                {
                    plannedActions.Remove(activePlannedAction);
                    ProcessActivePlannedAction();

                    break;
                }
            }
        }

        private void Update()
        {
            ProcessActivePlannedAction();

            if(activeReflexAction != null && (activeReflexAction.Status == EActionStatus.Finished || activeReflexAction.Status == EActionStatus.Cancelled))
            {
                activeReflexAction = null;
            }
        }

        private void OnGUI()
        {
            if(!displayDebugGUI)
            {
                return;
            }

            GUIStyle reflexActionStyle = new(GUI.skin.label) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
            reflexActionStyle.normal.textColor = Color.cyan;

            GUI.Label(new Rect(10, 10, 400, 50), activeReflexAction != null ? activeReflexAction.ToString() : "", reflexActionStyle);

            GUIStyle currentActionStyle = new(GUI.skin.label) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
            currentActionStyle.normal.textColor = Color.yellow;

            if(plannedActions.Count > 0)
            {
                GUI.Label(new Rect(10, 70, 400, 50), activePlannedAction.ToString(), currentActionStyle);

                GUIStyle pendingActionsStyle = new(GUI.skin.label) { fontSize = 25, alignment = TextAnchor.UpperLeft };
                pendingActionsStyle.normal.textColor = Color.white;
                StringBuilder builder = new();

                for(int actionIndex = plannedActions.Count - 2; actionIndex > -1; actionIndex--)
                {
                    builder.AppendLine(plannedActions[actionIndex].ToString());
                }

                GUI.Label(new Rect(10, 130, 400, 150), builder.ToString(), pendingActionsStyle);
            }
            else
            {
                GUI.Label(new Rect(10, 70, 400, 50), "No action assigned", currentActionStyle);
            }
        }
    }
}
