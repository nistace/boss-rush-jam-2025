using System.Collections.Generic;
using System.Text;
using BossRushJam25.Character.AI.Actions;
using BossRushJam25.Character.AI.Actions.ActionData;
using BossRushJam25.Character.AI.Actions.ActionTriggers;
using UnityEngine;
using Utils;

namespace BossRushJam25.Character.AI
{
    public class ActionPriorityHandler : MonoBehaviour
    {
        [SerializeField] private bool displayDebugGUI;
        [SerializeField] private bool drawPreviews;
        [SerializeField] private float evaluationTickPeriod = 0.2f;
        [SerializeField] private int queueSize = 3;
        [SerializeField] private SerializableDictionary<EActionType, AActionData> actionDataMap;

        [Header("Action triggers")]
        [SerializeField] private List<AActionTrigger> actionTriggers;

        protected CharacterCore character;
        protected List<AAction> plannedActions = new();
        protected float evaluationTickTimer;

        protected AAction ActiveAction => plannedActions.Count > 0 ? plannedActions[0] : null;
        public SerializableDictionary<EActionType, AActionData> ActionDataMap => actionDataMap;

        public void Initialize(CharacterCore character)
        {
            this.character = character;

            foreach (AActionTrigger actionTrigger in actionTriggers)
            {
                actionTrigger.Initialize(character);
            }

            evaluationTickTimer = evaluationTickPeriod;
        }

        public void PlanAction(AAction action)
        {
            if(plannedActions.Count >= queueSize)
            {
                return;
            }

            plannedActions.Add(action);
        }

        public void ForceAction(AAction action)
        {
            ActiveAction?.Cancel();

            if (plannedActions.Count >= queueSize)
            {
                RemoveAction(plannedActions[^1]);
            }

            plannedActions.Insert(0, action);
        }

        public void RemoveAction(AAction action)
        {
            if(action == ActiveAction && action.Status == EActionStatus.Started)
            {
                action.Cancel();
            }

            action.CleanUp();
            plannedActions.Remove(action);
        }

        public void RemoveAllActions()
        {
            for(int actionIndex = plannedActions.Count - 1;  actionIndex > -1; actionIndex--)
            {
                RemoveAction(plannedActions[actionIndex]);
            }
        }

        private void ProcessActivePlannedAction()
        {
            if(ActiveAction == null)
            {
                return;
            }

            switch(ActiveAction.Status)
            {
                case EActionStatus.Pending:
                {
                    ActiveAction.Execute();

                    break;
                }
                case EActionStatus.Started:
                {
                    ActiveAction.Update();

                    break;
                }
                case EActionStatus.Finished:
                {
                    RemoveAction(ActiveAction);
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

        private void CheckEvaluationTick()
        {
            if(evaluationTickTimer > evaluationTickPeriod)
            {
                EvaluateActionsProbability();
                evaluationTickTimer -= evaluationTickPeriod;
            }

            evaluationTickTimer += Time.deltaTime;
        }

        private void EvaluateActionsProbability()
        {
            actionTriggers.Sort();

            bool newActionAssigned = false;

            foreach(AActionTrigger actionTrigger in actionTriggers)
            {
                if(actionTrigger.IsActive)
                {
                    AAction newAction = actionTrigger.Assess();

                    if(newAction != null)
                    {
                        //TODO: ne pas annuler l’action en cours si c’est la même
                        if(!newActionAssigned)
                        {
                            RemoveAllActions();
                        }

                        newAction.Assign();
                        newActionAssigned = true;
                    }
                }
            }
        }

        private void Update()
        {
            ProcessActivePlannedAction();
            DrawPreviews();
            CheckEvaluationTick();
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

                GUI.Label(new Rect(10, 10, 400, 30), ActiveAction.ToString(), ActiveAction is AReflexAction ? reflexActionStyle : activeActionStyle);

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
