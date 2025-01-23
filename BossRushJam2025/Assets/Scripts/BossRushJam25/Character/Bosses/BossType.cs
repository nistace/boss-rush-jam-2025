using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses {
   [CreateAssetMenu]
   public class BossType : ScriptableObject {
      [SerializeField] protected bool isInvincible;
      [SerializeField] protected int maxHealth;
      [SerializeField] protected DamageTypes vulnerabilities;
      [SerializeField] protected HexGridPreset hexGridPreset;

      public bool IsInvincible => isInvincible;
      public int MaxHealth => maxHealth;
      public DamageTypes Vulnerabilities => isInvincible ? DamageTypes.Nothing : vulnerabilities;
      public HexGridPreset HexGridPreset => hexGridPreset;
   }
}