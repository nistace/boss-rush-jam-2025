using System.Collections.Generic;
using BossRushJam25.Character.AI;
using UnityEngine;
using Utils;

namespace BossRushJam25.Character
{
    public class DebugDodgeSimulation : MonoBehaviour
    {
        [SerializeField] private float projectileSpeed;

        protected CharacterCore character;
        protected List<Transform> projectiles = new();

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void Update()
        {
            HandleInput();
            MoveProjectiles();
        }

        private void HandleInput()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 500) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                // {
                //     SpawnProjectile(hit.point);
                // }

                SpawnProjectile();
            }
        }

        private void SpawnProjectile()
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.transform.position = character.transform.position + Random.insideUnitCircle.ToVector3(EAxis.Y) * 4f;
            projectile.transform.forward = character.transform.position - projectile.transform.position;
            projectile.transform.localScale = Vector3.one * 0.3f;
            projectiles.Add(projectile.transform);
            new DodgeAction(character, projectile.transform.forward).Assign();
        }

        private void MoveProjectiles()
        {
            for(int projectileIndex = projectiles.Count - 1; projectileIndex > -1; projectileIndex--)
            {
                Transform projectile = projectiles[projectileIndex];

                if(projectile.position.sqrMagnitude > 2500f)
                {
                    projectiles.Remove(projectile);
                    Destroy(projectile.gameObject);

                    continue;
                }

                projectile.Translate(Time.deltaTime * projectileSpeed * Vector3.forward);
            }
        }
    }
}
