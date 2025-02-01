using BossRushJam25.Health;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class GridHexContentType : ScriptableObject {
      [SerializeField] protected int maxToSpawn = 1;
      [SerializeField] protected int rotationStepsInHex = 1;
      [SerializeField] protected bool preventPowerUpSpawning = true;
      [SerializeField] protected bool locksHexInPlace;
      [SerializeField] protected bool isInvincible;
      [SerializeField] protected int maxHealth = 10;
      [SerializeField] protected bool canDisplayHealthBar = true;
      [SerializeField] protected DamageTypes vulnerabilities = 0;
      [SerializeField] protected DamageTypes absorbedDamages = 0;
      [SerializeField] protected DamageTypes obstacleForDamageTypes = (DamageTypes)~0;

      public int MaxToSpawn => maxToSpawn;
      public int RotationStepsInHex => rotationStepsInHex;
      public bool LocksHexInPlace => locksHexInPlace;
      public bool PreventPowerUpSpawning => preventPowerUpSpawning;
      public bool CanDisplayHealthBar => canDisplayHealthBar;
      protected DamageTypes Vulnerabilities => isInvincible ? DamageTypes.Nothing : vulnerabilities;
      public HealthSystem NewHealthSystem => new(maxHealth, Vulnerabilities);
      public DamageTypes ObstacleForDamageTypes => obstacleForDamageTypes;
      public DamageTypes AbsorbedDamages => absorbedDamages;
   }
}