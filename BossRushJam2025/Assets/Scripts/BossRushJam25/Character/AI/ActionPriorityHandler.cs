using System.Collections.Generic;
using System.Linq;
using System.Text;
using BossRushJam25.BossFights;
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
        [SerializeField] private int maxViableActionsCount = 3;
        [SerializeField] private SerializableDictionary<EActionType, AActionData> actionDataMap;

        [Header("Action triggers")]
        [SerializeField] private List<AActionTrigger> actionTriggers;

        protected CharacterCore character;
        protected List<AAction> viableActions = new();
        protected List<AAction> previouslyViableActions = new();
        protected float evaluationTickTimer;

        protected AAction ActiveAction => viableActions.Count > 0 ? viableActions[0] : null;
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

        public void ForceAction(AAction action)
        {
            action.IsForced = true;
            viableActions.Add(action);
            RefreshPriorities();
        }

        public void RemoveAction(AAction action)
        {
            if(action.Status == EActionStatus.Started)
            {
                action.Cancel();
            }

            action.CleanUp();
            viableActions.Remove(action);
        }

        public void RemoveAllActions()
        {
            for(int actionIndex = viableActions.Count - 1;  actionIndex > -1; actionIndex--)
            {
                RemoveAction(viableActions[actionIndex]);
            }
        }

        private void ProcessActiveAction()
        {
            if(ActiveAction == null)
            {
                return;
            }

            switch(ActiveAction.Status)
            {
                case EActionStatus.NotStarted:
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
                    ProcessActiveAction();

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

            for(int actionIndex = 0; actionIndex < viableActions.Count; actionIndex++)
            {
                viableActions[actionIndex].DrawPreview(priorityValue01: (float)actionIndex / (maxViableActionsCount - 1));
            }
        }

        private void CheckEvaluationTick()
        {
            if(evaluationTickTimer > evaluationTickPeriod)
            {
                EvaluateActions();
                evaluationTickTimer -= evaluationTickPeriod;
            }

            evaluationTickTimer += Time.deltaTime;
        }

        private void EvaluateActions()
        {
            previouslyViableActions.Clear();
            previouslyViableActions.AddRange(viableActions);
            viableActions.Clear();

            foreach(AActionTrigger actionTrigger in actionTriggers)
            {
                if(actionTrigger.IsActive)
                {
                    if(!actionTrigger.TryGet(out AAction newAction))
                    {
                        continue;
                    }

                    AAction alreadyExistingAction = previouslyViableActions.Where(action => action.Equals(newAction)).FirstOrDefault();

                    if(alreadyExistingAction != null)
                    {
                        viableActions.Add(alreadyExistingAction);
                        previouslyViableActions.Remove(alreadyExistingAction);
                    }
                    else
                    {
                        viableActions.Add(newAction);
                    }
                }
            }

            foreach (AAction action in previouslyViableActions)
            {
                if(action.IsForced)
                {
                    viableActions.Add(action);
                }
                else
                {
                    RemoveAction(action);
                }
            }

            RefreshPriorities();
        }

        private void RefreshPriorities()
        {
            foreach(AAction action in viableActions)
            {
                action.ComputePriority();
            }

            viableActions.Sort();

            for(int actionIndex = viableActions.Count - 1; actionIndex > 0; actionIndex--)
            {
                AAction action = viableActions[actionIndex];

                if(actionIndex >= maxViableActionsCount)
                {
                    RemoveAction(action);

                    continue;
                }

                if(action.Status == EActionStatus.Started)
                {
                    action.Cancel();
                }
            }
        }

        private void Update() {
            if (!BossFightInfo.IsPlaying) return;
            
            ProcessActiveAction();
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

            if(viableActions.Count > 0)
            {
                GUIStyle activeActionStyle = new(pendingActionStyle);
                activeActionStyle.normal.textColor = Color.red;

                GUI.Label(new Rect(10, 10, 500, 30), ActiveAction.ToString(), activeActionStyle);

                StringBuilder builder = new();

                for(int actionIndex = 1; actionIndex < viableActions.Count; actionIndex++)
                {
                    builder.AppendLine(viableActions[actionIndex].ToString());
                }

                GUI.Label(new Rect(10, 40, 500, 60), builder.ToString(), pendingActionStyle);
            }
            else
            {
                GUI.Label(new Rect(10, 10, 500, 30), "No viable action", pendingActionStyle);
            }
        }

        private void OnDrawGizmos() {
            if (!BossFightInfo.IsPlaying) return;
            foreach(AAction action in viableActions)
            {
                action.DrawGizmos();
            }
        }
    }
}
