using System;
using UnityEngine;

namespace BossRushJam25.HexGrid.Glyphs {
   [CreateAssetMenu]
   public class GlyphDefinition : ScriptableObject {
      [SerializeField] private GridHexPreset originGlyphPart;
      [SerializeField] private AttachedGlyphPart[] otherGlyphParts;

      [Serializable] private class AttachedGlyphPart {
         [SerializeField] protected GridHexPreset otherGlyphPart;
         [SerializeField] protected Vector3 offsetWithOrigin;
         [SerializeField] protected float rotationWithOrigin;
      }
   }
}