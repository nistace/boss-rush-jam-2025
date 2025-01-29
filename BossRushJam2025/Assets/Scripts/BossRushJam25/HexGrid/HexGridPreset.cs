using System;
using System.Collections.Generic;
using BossRushJam25.HexGrid.Glyphs;
using UnityEngine;
using Utils;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class HexGridPreset : ScriptableObject {
      [SerializeField] protected SerializableDictionary<GridHexPreset, Vector2Int> requiredHexPresets;
      [SerializeField] protected Glyph[] glyphs;
      [SerializeField] protected HexGridPresetData[] presets;
      [SerializeField] protected GridHexPreset fillerPreset;

      public IReadOnlyList<Glyph> Glyphs => glyphs;
      public IReadOnlyList<HexGridPresetData> Presets => presets;
      public GridHexPreset FillerPreset => fillerPreset;

      public IReadOnlyDictionary<GridHexPreset, Vector2Int> GetRequiredHexes() => requiredHexPresets;

      [Serializable]
      public class HexGridPresetData {
         [SerializeField] protected GridHexPreset gridHexPreset;
         [SerializeField] protected int amount = 1;

         public GridHexPreset GridHexPreset => gridHexPreset;
         public int Amount => amount;
      }
   }
}
