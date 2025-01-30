using BossRushJam25.Character.AI.Actions.ActionTriggers;
using UnityEngine;

public class ControlArea : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GoToControlHexWhenTooFar goToControlHexTrigger;
    [SerializeField] private Color colorWhenHeroInside;
    [SerializeField] private Color colorWhenHeroOutside;

    private void Awake()
    {
        transform.localScale = new Vector3(goToControlHexTrigger.MaxDistanceWithControlHex * 2, 1f, goToControlHexTrigger.MaxDistanceWithControlHex * 2);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Hero"))
        {
            meshRenderer.material.SetColor("_Color", colorWhenHeroInside);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Hero"))
        {
            meshRenderer.material.SetColor("_Color", colorWhenHeroOutside);
        }
    }
}
