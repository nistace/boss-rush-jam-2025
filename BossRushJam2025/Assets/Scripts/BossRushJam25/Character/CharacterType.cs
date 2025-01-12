using UnityEngine;

namespace BossRushJam25.Character {
   [CreateAssetMenu]
   public class CharacterType : ScriptableObject {
      [SerializeField] protected Vector2Int spawnPosition;
      [SerializeField] protected int maxHealth = 10;

      public Vector2Int SpawnPosition => spawnPosition;
      public int MaxHealth => maxHealth;
   }
}