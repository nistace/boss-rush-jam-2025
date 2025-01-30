using UnityEngine;
using BossRushJam25.PowerUps;
using BossRushJam25.Character;

namespace BossRushJam25.ControlHex
{
    public class PowerUpsCollector : MonoBehaviour
    {
        private CharacterCore character;

        public void Initialize(CharacterCore character)
        {
            this.character = character;
        }

        private void Collect(PowerUp powerUp)
        {
            if (powerUp.Type.HealAmount > 0)
            {
                character.Health.Heal(powerUp.Type.HealAmount);
            }
            if (powerUp.Type.DamageUpAmount > 0)
            {
                character.ChangeDamageInfo(character.DamageInfo.WithIncreasedDamage(powerUp.Type.DamageUpAmount));
            }
            if (powerUp.Type.DamageSpeedUpAmount > float.Epsilon)
            {
                character.ChangeDamageInfo(character.DamageInfo.WithIncreasedSpeed(powerUp.Type.DamageSpeedUpAmount));
            }

            powerUp.Destroy();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("PowerUp")
                && collider.TryGetComponent<PowerUp>(out PowerUp powerUp)
                )
            {
                Collect(powerUp);
            }
        }
    }
}
