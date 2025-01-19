using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   [CreateAssetMenu]
   public class GlyphDefinition : ScriptableObject {
      [SerializeField] private GlyphChunk originGlyphChunk;
      [SerializeField] private Vector3 spawnOffsetWithOrigin;
      [SerializeField] private AttachedGlyphPart[] otherGlyphParts;

      public GlyphChunk OriginGlyphChunk => originGlyphChunk;
      public Vector3 SpawnOffsetWithOrigin => spawnOffsetWithOrigin;

      public IReadOnlyList<AttachedGlyphPart> OtherGlyphParts => otherGlyphParts;

      [Serializable] public class AttachedGlyphPart {
         [SerializeField] protected GlyphChunk glyphChunk;
         [SerializeField] protected Vector3 offsetWithOrigin;
         [SerializeField] protected float rotationWithOrigin;

         public GlyphChunk GlyphChunk => glyphChunk;
         public Vector3 OffsetWithOrigin => offsetWithOrigin;
         public float RotationWithOrigin => rotationWithOrigin;
      }
   }
}