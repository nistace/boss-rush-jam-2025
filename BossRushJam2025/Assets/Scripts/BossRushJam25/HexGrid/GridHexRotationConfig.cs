using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class GridHexRotationConfig : ScriptableObject {
      [SerializeField] protected float delayBeforeRotation = .1f;
      [SerializeField] protected AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
      [SerializeField] protected float translationSpeed = .2f;
      [SerializeField] protected float delayAfterRotation = .1f;
      [SerializeField] protected float singleHexRotationSpeed = 4;

      public float DelayBeforeRotation => delayBeforeRotation;
      public AnimationCurve AccelerationCurve => accelerationCurve;
      public float TranslationSpeed => translationSpeed;
      public float DelayAfterRotation => delayAfterRotation;
      public float SingleHexRotationSpeed => singleHexRotationSpeed;

      public float GetTranslationSpeedDelta(float deltaTime, bool withAcceleration, float time) {
         if (!withAcceleration) return deltaTime * translationSpeed;
         return accelerationCurve.Evaluate(time * translationSpeed) * deltaTime * translationSpeed;
      }
   }
}