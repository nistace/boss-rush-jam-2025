using UnityEngine;
using UnityEngine.AI;

public class DebugHeroDestinationAssigner : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent navMeshAgent;

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                navMeshAgent.destination = hit.point;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(navMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(navMeshAgent.destination, 0.3f);
        }
    }
}
