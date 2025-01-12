using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.PowerUps
{
    public class PowerUpsManager : MonoBehaviour
    {
        [SerializeField] private GameObject powerUpPrefab;
        public void Initialize()
        {
            SpawnPowerUp();
        }

        private void SpawnPowerUp()
        {
            GridHex randomHex = HexGridController.Instance.GetRandomGridHex();
            Vector3 randomPosition = randomHex.transform.position + 1f * Vector3.up;
            Instantiate(powerUpPrefab, randomPosition, Quaternion.identity, randomHex.transform);
        }
    }
}
