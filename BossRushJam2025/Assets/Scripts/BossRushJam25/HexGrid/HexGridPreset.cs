using System;
using System.Collections.Generic;
using BossRushJam25.HexGrid.Glyphs;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class HexGridPreset : ScriptableObject {
      [SerializeField] protected GlyphDefinition[] glyphs;
      [SerializeField] protected HexGridPresetData[] presets;
      [SerializeField] protected GridHexPreset fillerPreset;

      public IReadOnlyList<GlyphDefinition> Glyphs => glyphs;
      public IReadOnlyList<HexGridPresetData> Presets => presets;
      public GridHexPreset FillerPreset => fillerPreset;

      [Serializable]
      public class HexGridPresetData {
         [SerializeField] protected GridHexPreset gridHexPreset;
         [SerializeField] protected int amount = 1;

         public GridHexPreset GridHexPreset => gridHexPreset;
         public int Amount => amount;
      }
   }
}