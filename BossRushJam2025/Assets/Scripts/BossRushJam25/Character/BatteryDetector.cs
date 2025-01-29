using System.Collections.Generic;
using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character
{
    public class BatteryDetector : MonoBehaviour
    {
        [SerializeField] protected float detectionRadius = 10f;
        [SerializeField] protected float collectionRadius = 1f;
        [SerializeField] protected SphereCollider detectionCollider;

        protected CharacterCore character;
        protected List<GridHex> detectedBatteryHexes = new();

        public GridHex NearestBatteryHex { get; private set; }

        public UnityEvent OnNearestBatteryHexChanged { get; } = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void CheckNearestBatteryHex()
        {
            float nearestBatteryHexDistance = float.MaxValue;
            GridHex newNearestBatteryHex = null;

            foreach(GridHex batteryHex in detectedBatteryHexes)
            {
                float sqrDistance = (batteryHex.transform.position - transform.position).sqrMagnitude;

                if(sqrDistance < nearestBatteryHexDistance)
                {
                    nearestBatteryHexDistance = sqrDistance;
                    newNearestBatteryHex = batteryHex;
                }
            }

            if(newNearestBatteryHex != NearestBatteryHex)
            {
                NearestBatteryHex = newNearestBatteryHex;
                OnNearestBatteryHexChanged.Invoke();
            }
        }

        private void CheckHexDestroyedContents()
        {
            for(int hexIndex = detectedBatteryHexes.Count - 1; hexIndex > -1; hexIndex--)
            {
                GridHex hex = detectedBatteryHexes[hexIndex];

                if(!hex.ContentsAreDamageable(character.Type.DamageInfo.DamageType))
                {
                    detectedBatteryHexes.Remove(hex);
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GridHex hex = collider.GetComponentInParent<GridHex>();

                if(hex.ContentsAreDamageable(character.Type.DamageInfo.DamageType))
                {
                    detectedBatteryHexes.Add(hex);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GridHex hex = collider.GetComponentInParent<GridHex>();

                if(hex.ContentsAreDamageable(character.Type.DamageInfo.DamageType))
                {
                    detectedBatteryHexes.Remove(hex);
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
            if (detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius;
            }
        }
    }
}
