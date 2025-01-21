using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses {
   [CreateAssetMenu]
   public class BossType : ScriptableObject {
      [SerializeField] protected int maxHealth;
      [SerializeField] protected HexGridPreset hexGridPreset;

      public int MaxHealth => maxHealth;
      public HexGridPreset HexGridPreset => hexGridPreset;
   }
}