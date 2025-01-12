using BossRushJam25.Character;
using BossRushJam25.Character.Bosses;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.GameControllers {
   public class GameConfig : MonoBehaviour {
      public static GameConfig Instance { get; private set; }

      [SerializeField] protected GameObject spinStrategy;
      [SerializeField] protected PowerUpsManager powerUpsManager;
      [SerializeField] protected CharacterCore heroPrefab;
      [SerializeField] protected CharacterCore bossPrefab;
      [SerializeField] protected BossPatternManager bossPatternManagerPrefab;
      [SerializeField] protected LineRenderer pathLinePrefab;
      [SerializeField] protected Gradient actionPreviewsGradient;

      public PowerUpsManager PowerUpsManager => powerUpsManager;
      public GameObject SpinStrategy => spinStrategy;
      public CharacterCore HeroPrefab => heroPrefab;
      public CharacterCore BossPrefab => bossPrefab;
      public BossPatternManager BossPatternManagerPrefab => bossPatternManagerPrefab;
      public LineRenderer PathLinePrefab => pathLinePrefab;
      public Gradient ActionPreviewsGradient => actionPreviewsGradient;

      private void Awake() {
         Instance = this;
      }
   }
}