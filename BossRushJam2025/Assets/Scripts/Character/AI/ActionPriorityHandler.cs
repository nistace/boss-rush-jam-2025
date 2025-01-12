using System.Collections.Generic;
using System.Text;
using BossRushJam25.GameControllers;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.AI
{
    public class ActionPriorityHandler : MonoBehaviour
    {
        [SerializeField] private bool displayDebugGUI;
        [SerializeField] private int queueSize = 3;

        protected CharacterCore character;
        protected List<AAction> plannedActions = new();

        protected AAction ActivePlannedAction => plannedActions.Count > 0 ? plannedActions[0] : null;

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
                plannedActions.RemoveAt(3);
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
                    plannedActions.Remove(ActivePlannedAction);
                    ProcessActivePlannedAction();

                    break;
                }
            }
        }

        private void TryPlanNewAction()
        {
            if (plannedActions.Count > queueSize)
            {
                return;
            }

            //TODO: use enum
            int randomIndex = Random.Range(0,2);

            APlannedAction action = randomIndex switch
            {
                0 => new MoveAction(character, HexGridController.Instance.GetRandomPositionOnNavMesh()),
                1 => new CollectPowerUpAction(character, GameConfig.Instance.PowerUpsManager.powerUps[0]),
                _ => throw new System.NotImplementedException()
            };

            PlanAction(action);
        }

        private void Update()
        {
            ProcessActivePlannedAction();
            TryPlanNewAction();
        }

        private void OnGUI()
        {
            if(!displayDebugGUI)
            {
                return;
            }

            GUIStyle pendingActionStyle = new(GUI.skin.label) { fontSize = 25, alignment = TextAnchor.UpperLeft };
            pendingActionStyle.normal.textColor = Color.white;

            if (plannedActions.Count > 0)
            {
                GUIStyle reflexActionStyle = new(GUI.skin.label) { fontSize = 25, alignment = TextAnchor.MiddleLeft };
                reflexActionStyle.normal.textColor = Color.cyan;
                GUIStyle activeActionStyle = new(reflexActionStyle);
                activeActionStyle.normal.textColor = Color.yellow;

                GUI.Label(new Rect(10, 10, 400, 50), ActivePlannedAction.ToString(), ActivePlannedAction is AReflexAction ? reflexActionStyle : activeActionStyle);

                StringBuilder builder = new();

                for(int actionIndex = 1; actionIndex < plannedActions.Count; actionIndex++)
                {
                    builder.AppendLine(plannedActions[actionIndex].ToString());
                }

                GUI.Label(new Rect(10, 70, 400, 150), builder.ToString(), pendingActionStyle);
            }
            else
            {
                GUI.Label(new Rect(10, 10, 400, 50), "No action assigned", pendingActionStyle);
            }
        }
    }
}
