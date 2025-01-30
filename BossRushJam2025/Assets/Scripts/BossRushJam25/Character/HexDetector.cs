using System.Collections.Generic;
using System.Linq;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character
{
    public class HexDetector : MonoBehaviour
    {
        [SerializeField] protected float detectionRadius = 10f;
        [SerializeField] protected float collectionRadius = 1f;
        [SerializeField] protected SphereCollider detectionCollider;
        [SerializeField] protected List<GridHexType> hexTypes;
        [SerializeField] protected List<GridHexContentType> contentTypes;

        protected List<GridHex> detectedHexes = new();

        public GridHex NearestHex { get; private set; }

        private void CheckNearestBatteryHex()
        {
            float nearestHexDistance = float.MaxValue;
            GridHex newNearestHex = null;

            foreach(GridHex hex in detectedHexes)
            {
                float sqrDistance = (hex.transform.position - transform.position).sqrMagnitude;

                if(sqrDistance < nearestHexDistance)
                {
                    nearestHexDistance = sqrDistance;
                    newNearestHex = hex;
                }
            }

            if(newNearestHex != NearestHex)
            {
                NearestHex = newNearestHex;
            }
        }

        private void CheckHexDestroyedContents()
        {
            if(contentTypes.Count == 0)
            {
                return;
            }

            for(int hexIndex = detectedHexes.Count - 1; hexIndex > -1; hexIndex--)
            {
                GridHex hex = detectedHexes[hexIndex];

                if(!HexMatchesFilters(hex))
                {
                    detectedHexes.Remove(hex);
                }
            }
        }

        private bool HexMatchesFilters(GridHex hex)
        {
            return (hexTypes.Count == 0 || hexTypes.Contains(hex.Type))
                    && (contentTypes.Count == 0 || hex.HexContents.Any(content => contentTypes.Contains(content.Type)));
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GridHex hex = collider.GetComponentInParent<GridHex>();

                if(HexMatchesFilters(hex))
                {
                    detectedHexes.Add(hex);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GridHex hex = collider.GetComponentInParent<GridHex>();

                if(HexMatchesFilters(hex))
                {
                    detectedHexes.Remove(hex);
                }
            }
        }

        private void Update()
        {
            CheckHexDestroyedContents();
            CheckNearestBatteryHex();
        }

        private void OnValidate()
        {
            if(detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius;
            }
        }
    }
}
