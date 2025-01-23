using BossRushJam25.Health;
using UnityEngine;

namespace BossRushJam25.Character {
   [CreateAssetMenu]
   public class CharacterType : ScriptableObject {
      [SerializeField] protected bool isInvincible;
      [SerializeField] protected int maxHealth = 10;
      [SerializeField] protected DamageTypes vulnerabilities;
      [SerializeField] protected Vector2Int spawnPosition;

      public bool IsInvincible => isInvincible;
      public int MaxHealth => maxHealth;
      public DamageTypes Vulnerabilities => isInvincible ? DamageTypes.Nothing : vulnerabilities;
      public Vector2Int SpawnPosition => spawnPosition;
   }
}