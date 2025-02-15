using System.Collections.Generic;
using BossRushJam25.Character.AI.Actions;
using BossRushJam25.GameControllers;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;
using Utils;

namespace BossRushJam25.Character
{
    public class DebugActionsTrigger : MonoBehaviour
    {
        [Header("Dodge")]
        [SerializeField] private float projectileSpeed;

        protected CharacterCore character;
        protected List<Transform> projectiles = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void HandleInputs()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)
                && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, maxDistance: 500, layerMask: ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)
                && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")
                )
            {
                character.ActionPriorityHandler.ForceAction(new MoveAction(character, hit.point, Color.black));
            }

            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                character.ActionPriorityHandler.ForceAction(new TakeCoverAction(character));
            }

            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                GameConfig.Instance.PowerUpsManager.SpawnDefaultPowerUpOnRandomEmptyHex();
            }

            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                // if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                // {
                //     SpawnProjectile(hit.point);
                // }

                SpawnProjectile();
            }

            if(Input.GetKeyDown(KeyCode.Alpha5)
                && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, maxDistance: 500, layerMask: ~0, queryTriggerInteraction: QueryTriggerInteraction.Ignore)
                && HexGridController.Instance.TryGetHex(hit.point, out GridHex hex)
                && hex.ContentsAreDamageable(character.DamageInfo.DamageType.AsFlags())
                )
            {
                character.ActionPriorityHandler.ForceAction(new AttackMeleeAction(character, hex));
            }
        }

        private void SpawnProjectile()
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.transform.position = character.transform.position + Random.insideUnitCircle.ToVector3(EAxis.Y) * 4f;
            projectile.transform.forward = character.transform.position - projectile.transform.position;
            projectile.transform.localScale = Vector3.one * 0.3f;
            projectiles.Add(projectile.transform);
            character.ActionPriorityHandler.ForceAction(new DodgeAction(character, projectile.transform.forward));
        }

        private void MoveProjectiles()
        {
            for (int projectileIndex = projectiles.Count - 1; projectileIndex > -1; projectileIndex--)
            {
                Transform projectile = projectiles[projectileIndex];

                if (projectile.position.sqrMagnitude > 2500f)
                {
                    projectiles.Remove(projectile);
                    Destroy(projectile.gameObject);

                    continue;
                }

                projectile.Translate(Time.deltaTime * projectileSpeed * Vector3.forward);
            }
        }

        private void Update()
        {
            HandleInputs();

            MoveProjectiles();
        }
    }
}
