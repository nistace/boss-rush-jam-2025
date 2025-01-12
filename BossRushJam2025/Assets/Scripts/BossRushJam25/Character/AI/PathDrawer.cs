using UnityEngine;
using UnityEngine.AI;

namespace BossRushJam25.Character.AI
{
    public static class PathDrawer
    {
        public static void UpdatePath(LineRenderer pathLine, Vector3 origin, NavMeshPath path)
        {
            Vector3[] positions = new Vector3[path.corners.Length + 1];

            positions[0] = origin;

            for(int cornerIndex = 0; cornerIndex < path.corners.Length; cornerIndex++)
            {
                positions[cornerIndex + 1] = path.corners[cornerIndex];
            }

            pathLine.positionCount = path.corners.Length;
            pathLine.SetPositions(path.corners);
        }
    }
}
