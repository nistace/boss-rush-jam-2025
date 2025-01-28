using System;
using System.Collections.Generic;
using BossRushJam25.HexGrid.Glyphs;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class HexGridPreset : ScriptableObject {
      [SerializeField] protected Vector2Int heroSpawnPosition = Vector2Int.right;
      [SerializeField] protected Vector2Int heroControlHexPosition = Vector2Int.zero;
      [SerializeField] protected GridHexPreset heroSpawnHexPreset;
      [SerializeField] protected GridHexPreset heroControlHexPreset;
      [SerializeField] protected Glyph[] glyphs;
      [SerializeField] protected HexGridPresetData[] presets;
      [SerializeField] protected GridHexPreset fillerPreset;

      public Vector2Int HeroSpawnPosition => heroSpawnPosition;
      public Vector2Int HeroControlHexPosition => heroControlHexPosition;
      public GridHexPreset HeroSpawnHexPreset => heroSpawnHexPreset;
      public GridHexPreset HeroControlHexPreset => heroControlHexPreset;
      public IReadOnlyList<Glyph> Glyphs => glyphs;
      public IReadOnlyList<HexGridPresetData> Presets => presets;
      public GridHexPreset FillerPreset => fillerPreset;

      public Dictionary<Vector2Int, GridHexPreset> GetRequiredHexes() => new Dictionary<Vector2Int, GridHexPreset>
         { { heroSpawnPosition, heroSpawnHexPreset }, { heroControlHexPosition, heroControlHexPreset } };

      [Serializable]
      public class HexGridPresetData {
         [SerializeField] protected GridHexPreset gridHexPreset;
         [SerializeField] protected int amount = 1;

         public GridHexPreset GridHexPreset => gridHexPreset;
         public int Amount => amount;
      }
   }
}
