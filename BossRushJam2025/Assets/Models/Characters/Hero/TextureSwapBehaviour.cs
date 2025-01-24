using BossRushJam25.BossFights;
using UnityEngine;

namespace BossRushJam25.Character.Heroes
{
    public class TextureSwapBehaviour : StateMachineBehaviour
    {
        [SerializeField] protected Texture2D texture;
        [SerializeField] protected int columnCount;
        [SerializeField] protected int rowCount;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Material material = BossFightInfo.Hero.GetComponentInChildren<MeshRenderer>().material;
            material.SetTexture("_MainTex", texture);
            material.SetInt("_ColumnCount", columnCount);
            material.SetInt("_RowCount", rowCount);
        }
    }
}
