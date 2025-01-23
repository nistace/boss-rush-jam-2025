using BossRushJam25.Health;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class GridHexContentType : ScriptableObject {
      [SerializeField] protected int maxToSpawn = 1;
      [SerializeField] protected int rotationStepsInHex = 1;
      [SerializeField] protected bool isInvincible;
      [SerializeField] protected int maxHealth = 10;
      [SerializeField] protected DamageTypes vulnerabilities = 0;

      public int MaxToSpawn => maxToSpawn;
      public int RotationStepsInHex => rotationStepsInHex;
      protected DamageTypes Vulnerabilities => isInvincible ? DamageTypes.Nothing : vulnerabilities;
      public HealthSystem NewHealthSystem => new(maxHealth, Vulnerabilities);
   }
}