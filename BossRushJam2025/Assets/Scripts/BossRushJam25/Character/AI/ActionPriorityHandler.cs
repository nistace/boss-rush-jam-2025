using System.Collections.Generic;
using System.Text;
using BossRushJam25.Character.AI.Actions;
using UnityEngine;
using Utils;

namespace BossRushJam25.Character.AI
{
    public class ActionPriorityHandler : MonoBehaviour
    {
        [SerializeField] private bool displayDebugGUI;
        [SerializeField] private bool drawPreviews;
        [SerializeField] private int queueSize = 3;
        [SerializeField] private SerializableDictionary<EActionType, AActionData> actionDataMap;

        protected CharacterCore character;
        protected List<AAction> plannedActions = new();

        protected AAction ActivePlannedAction => plannedActions.Count > 0 ? plannedActions[0] : null;
        public SerializableDictionary<EActionType, AActionData> ActionDataMap => actionDataMap;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        public void PlanAction(APlannedAction action)
        {
            if(plannedActions.Count >= queueSize)
            {
                return;
            }

            plannedActions.Add(action);
        }

        public void ForceAction(AAction action)
        {
            ActivePlannedAction?.Reset();
            plannedActions.Insert(0, action);

            if (plannedActions.Count == 4)
            {
                CleanUpAction(plannedActions[3]);
            }
        }

        public void CancelActiveAction()
        {
            ActivePlannedAction?.Cancel();
        }

        public void CancelAllActions()
        {
            CancelActiveAction();
            plannedActions.Clear();
        }

        private void ProcessActivePlannedAction()
        {
            if(ActivePlannedAction == null)
            {
                return;
            }

            switch(ActivePlannedAction.Status)
            {
                case EActionStatus.Pending:
                {
                    ActivePlannedAction.Execute();

                    break;
                }
                case EActionStatus.Finished:
                case EActionStatus.Cancelled:
                {
                    CleanUpAction(ActivePlannedAction);
                    ProcessActivePlannedAction();

                    break;
                }
            }
        }

        private void DrawPreviews()
        {
            if(!drawPreviews)
            {
                return;
            }

            for(int actionIndex = 0; actionIndex < plannedActions.Count; actionIndex++)
            {
                plannedActions[actionIndex].DrawPreview(priorityValue01: (float)actionIndex / queueSize);
            }
        }

        private void CleanUpAction(AAction action)
        {
            plannedActions.Remove(action);
            action.CleanUp();
        }

        private void Update()
        {
            ProcessActivePlannedAction();
            DrawPreviews();
        }

        private void OnGUI()
        {
            if(!displayDebugGUI)
            {
                return;
            }

            GUIStyle pendingActionStyle = new(GUI.skin.label) { fontSize = 25, alignment = TextAnchor.UpperLeft };
            pendingActionStyle.normal.textColor = Color.black;

            if (plannedActions.Count > 0)
            {
                GUIStyle reflexActionStyle = new(pendingActionStyle);
                reflexActionStyle.normal.textColor = Color.blue;
                GUIStyle activeActionStyle = new(reflexActionStyle);
                activeActionStyle.normal.textColor = Color.red;

                GUI.Label(new Rect(10, 10, 400, 30), ActivePlannedAction.ToString(), ActivePlannedAction is AReflexAction ? reflexActionStyle : activeActionStyle);

                StringBuilder builder = new();

                for(int actionIndex = 1; actionIndex < plannedActions.Count; actionIndex++)
                {
                    builder.AppendLine(plannedActions[actionIndex].ToString());
                }

                GUI.Label(new Rect(10, 40, 400, 60), builder.ToString(), pendingActionStyle);
            }
            else
            {
                GUI.Label(new Rect(10, 10, 400, 30), "No action assigned", pendingActionStyle);
            }
        }

        private void OnDrawGizmos()
        {
            foreach(AAction action in plannedActions)
            {
                action.DrawGizmos();
            }
        }
    }
}
