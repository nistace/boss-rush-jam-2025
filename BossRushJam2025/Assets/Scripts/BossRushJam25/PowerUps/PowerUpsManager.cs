using System.Collections.Generic;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.PowerUps
{
    public class PowerUpsManager : MonoBehaviour
    {
        [SerializeField] private GameObject powerUpPrefab;

        public List<GameObject> powerUps { get; } = new();

        public void Initialize()
        {
            SpawnPowerUp();
        }

        public void SpawnPowerUp()
        {
            GridHex randomHex = HexGridController.Instance.GetRandomGridHex();
            Vector3 randomPosition = randomHex.transform.position + 1f * Vector3.up;
            GameObject powerUp = Instantiate(powerUpPrefab, randomPosition, Quaternion.identity, randomHex.transform);
            powerUps.Add(powerUp);
        }
    }
}
