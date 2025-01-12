using BossRushJam25.Character;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.GameControllers {
   public class GameConfig : MonoBehaviour {
      public static GameConfig Instance { get; private set; }

      [SerializeField] protected GameObject spinStrategy;
      [SerializeField] protected PowerUpsManager powerUpsManager;
      [SerializeField] protected CharacterCore heroPrefab;
      [SerializeField] protected CharacterCore bossPrefab;

      public PowerUpsManager PowerUpsManager => powerUpsManager;
      public GameObject SpinStrategy => spinStrategy;
      public CharacterCore HeroPrefab => heroPrefab;
      public CharacterCore BossPrefab => bossPrefab;

      private void Awake() {
         Instance = this;
      }
   }
}