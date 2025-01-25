using System;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   [Serializable]
   public class Glyph {
      [SerializeField] protected GlyphDefinition definition;
      [SerializeField] protected float castingTime = 3;
      [SerializeField] protected PowerUp powerUpToSpawn;

      public GlyphDefinition Definition => definition;
      public float CastingTime => castingTime;
      public PowerUp PowerUpToSpawn => powerUpToSpawn;
   }
}