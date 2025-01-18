using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   [CreateAssetMenu]
   public class GlyphDefinition : ScriptableObject {
      [SerializeField] private GridHexPreset originGlyphPart;
      [SerializeField] private AttachedGlyphPart[] otherGlyphParts;

      public GridHexPreset OriginGlyphPart => originGlyphPart;

      public IReadOnlyList<AttachedGlyphPart> OtherGlyphParts => otherGlyphParts;

      [Serializable] public class AttachedGlyphPart {
         [SerializeField] protected GridHexPreset otherGlyphPart;
         [SerializeField] protected Vector3 offsetWithOrigin;
         [SerializeField] protected float rotationWithOrigin;

         public GridHexPreset OtherGlyphPart => otherGlyphPart;
         public Vector3 OffsetWithOrigin => offsetWithOrigin;
         public float RotationWithOrigin => rotationWithOrigin;
      }
   }
}