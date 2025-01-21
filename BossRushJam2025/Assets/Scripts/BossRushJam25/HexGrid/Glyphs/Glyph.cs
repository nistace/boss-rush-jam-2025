using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BossRushJam25.HexGrid.Glyphs {
   [Serializable]
   public class Glyph {
      [SerializeField] protected GlyphDefinition definition;
      [SerializeField] protected float castingTime = 3;
      [SerializeField] protected GameObject objectToSpawn;

      public GlyphDefinition Definition => definition;
      public float CastingTime => castingTime;
      public GameObject ObjectToSpawn => objectToSpawn;

      public void Spawn(Transform origin) {
         Object.Instantiate(objectToSpawn, origin.transform.TransformPoint(definition.SpawnOffsetWithOrigin), Quaternion.identity);
      }
   }
}