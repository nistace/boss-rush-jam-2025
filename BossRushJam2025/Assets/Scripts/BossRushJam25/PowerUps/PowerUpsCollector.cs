using UnityEngine;

namespace BossRushJam25.Character
{
    public class PowerUpsCollector : MonoBehaviour
    {
        private Collider[] results = new Collider[10];
        protected CharacterCore character;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void FixedUpdate()
        {
            if(Physics.OverlapSphereNonAlloc(character.transform.position, 1f, results, 1 << LayerMask.NameToLayer("PowerUp")) > 0)
            {
                Destroy(results[0].gameObject);
                character.NavMeshAgent.speed += 0.5f;
            }
        }
    }
}
