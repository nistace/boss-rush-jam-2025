using BossRushJam25.BossFights;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Combat {
   [RequireComponent(typeof(GridHexContent))]
   public class Turret : MonoBehaviour, ILaserCaster {
      [SerializeField] protected GridHexContent hexContent;
      [SerializeField] protected bool active;
      [SerializeField] protected DamageInfo damageInfo = new DamageInfo();
      [SerializeField] protected float cooldown = 4;
      [SerializeField] protected Transform rotatingPart;
      [SerializeField] protected float rotationSpeed = 60;

      private Vector2Int coordinatesWhereShotIsBlocked;
      public GridHexContent HexContent => hexContent;
      private DamageRunner DamageRunner { get; set; }
      private float CurrentCooldown { get; set; }
      private HexCoordinates.EDirection TargetDirection { get; set; } = HexCoordinates.EDirection.Left;
      public bool IsShooting => DamageRunner.HasStarted;
      public Vector2Int CoordinatesWhereShotIsBlocked => coordinatesWhereShotIsBlocked;

      public bool Active {
         get => active;
         set => active = value;
      }

      private void Start() {
         DamageRunner = new DamageRunner(damageInfo);
         DamageRunner.Reset();
         CurrentCooldown = cooldown;
      }

      private void Update() {
         if (!BossFightInfo.IsPlaying) return;
         if (!active) return;

         if (DamageRunner.HasStarted) {
            DamageRunner.Continue(Time.deltaTime);

            if (DamageRunner.DamageDealtThisFrame > 0) {
               foreach (var damagedCoordinates in LaserUtils.Shoot(HexGridController.Instance.WorldToCoordinates(transform.position), TargetDirection, true, out coordinatesWhereShotIsBlocked)) {
                  if (CombatUtils.GetHeroCoordinates() == damagedCoordinates) {
                     CombatUtils.DamageHero(damageInfo.DamageType, DamageRunner.DamageDealtThisFrame);
                  }
                  if (HexGridController.Instance.TryGetHex(damagedCoordinates, out var hex)) {
                     hex.TryDamageContents(DamageRunner.DamageDealtThisFrame, DamageType.Laser);
                  }
               }

               if (DamageRunner.Done()) {
                  DamageRunner.Reset();
                  CurrentCooldown = cooldown;
               }
            }
         }
         else {
            CurrentCooldown -= Time.deltaTime;

            if (CurrentCooldown > 0) {
               TargetDirection = HexCoordinates.RotationToDirection(Vector3.SignedAngle(Vector3.forward, BossFightInfo.Hero.HexLink.LinkedHex.transform.position - rotatingPart.position, Vector3.up));
            }

            var targetYawDegrees = TargetDirection.ToYawDegrees();

            rotatingPart.rotation = Quaternion.Euler(0, Mathf.MoveTowards(Vector3.SignedAngle(Vector3.forward, rotatingPart.forward, Vector3.up), targetYawDegrees, Time.deltaTime * rotationSpeed), 0);

            if (CurrentCooldown < 0 && Mathf.Abs(targetYawDegrees - Vector3.SignedAngle(Vector3.forward, rotatingPart.forward, Vector3.up)) < 2) {
               DamageRunner.Continue(Time.deltaTime);
            }
         }
      }
   }
}