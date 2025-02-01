using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Combat {
   [RequireComponent(typeof(GridHexContent))]
   public class LaserCondenser : MonoBehaviour, ILaserCaster {
      [SerializeField] protected GridHexContent hexContent;
      [SerializeField] protected Transform shootOrigin;
      [SerializeField] protected float damageMultiplier = 1f;
      [SerializeField] protected DamageTypes inputDamageTypes = (DamageTypes)~0;
      [SerializeField] protected DamageType outputDamageType = DamageType.OverclockedLaser;
      [SerializeField] protected float shootingTime = .5f;

      private float DamageLoadUp { get; set; }
      private float LastShotTime { get; set; } = float.MinValue;
      private Vector2Int coordinatesWhereShotIsBlocked;
      public GridHexContent HexContent => hexContent;
      private bool AffectingAnyHex { get; set; }
      public bool IsShooting => Time.time < LastShotTime + shootingTime;
      public Vector2Int CoordinatesWhereShotIsBlocked => coordinatesWhereShotIsBlocked;

      private void Reset() {
         hexContent = GetComponent<GridHexContent>();
      }

      private void Start() {
         hexContent.OnDamageAbsorbed.AddListener(HandleDamageAbsorbed);
      }

      private void OnDestroy() {
         if (hexContent) hexContent.OnDamageAbsorbed.RemoveListener(HandleDamageAbsorbed);
      }

      private void HandleDamageAbsorbed(DamageType damageType, int damage) {
         if (!inputDamageTypes.Contains(damageType)) return;

         DamageLoadUp += damage * damageMultiplier;
         if (DamageLoadUp > 1) {
            var damageToDeal = Mathf.FloorToInt(DamageLoadUp);
            DamageLoadUp -= damageToDeal;

            LastShotTime = Time.time;
            var coordinates = HexGridController.Instance.WorldToCoordinates(transform.position);
            foreach (var damagedCoordinates in LaserUtils.Shoot(coordinates, HexCoordinates.RotationToDirection(shootOrigin), outputDamageType, true, out coordinatesWhereShotIsBlocked)) {
               if (CombatUtils.GetHeroCoordinates() == damagedCoordinates) {
                  CombatUtils.DamageHero(outputDamageType, damageToDeal);
               }
               if (HexGridController.Instance.TryGetHex(damagedCoordinates, out var hex)) {
                  hex.TryDamageContents(damageToDeal, outputDamageType);
               }
            }

            if (HexGridController.Instance.TryGetHex(coordinatesWhereShotIsBlocked, out var lastHex)) {
               lastHex.TryDamageContents(damageToDeal, outputDamageType);
            }
         }
      }

      private void Update() {
         if (!IsShooting && !AffectingAnyHex) return;
         AffectedHexesManager.HideAllAffectedHexes(this);
         AffectingAnyHex = IsShooting;
         if (IsShooting) {
            var coordinates = HexGridController.Instance.WorldToCoordinates(transform.position);
            foreach (var damagedCoordinates in LaserUtils.Shoot(coordinates, HexCoordinates.RotationToDirection(shootOrigin), outputDamageType, true, out coordinatesWhereShotIsBlocked)) {
               AffectedHexesManager.SetAffectedHex(this, damagedCoordinates, true);
            }
         }
      }
   }
}