using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character.AI
{
    public static class PathDrawer
    {
        public static void UpdatePath(LineRenderer pathLine, Vector3 origin, NavMeshPath path)
        {
            Vector3[] positions = new Vector3[path.corners.Length + 1];

            for(int positionIndex = 0; positionIndex < path.corners.Length; positionIndex++)
            {
                positions[positionIndex] = path.corners[path.corners.Length - 1 - positionIndex];
            }

            positions[^1] = origin;

            pathLine.positionCount = path.corners.Length;
            pathLine.SetPositions(positions);
        }
    }
}
