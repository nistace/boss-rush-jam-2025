using System.Collections.Generic;
using BossRushJam25.Character;
using BossRushJam25.Character.Bosses;
using BossRushJam25.HexGrid;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.GameControllers {
   public class GameConfig : MonoBehaviour {
      public static GameConfig Instance { get; private set; }

      [SerializeField] protected GameObject spinStrategy;
      [SerializeField] protected PowerUpsManager powerUpsManager;
      [SerializeField] protected CharacterCore heroPrefab;
      [SerializeField] protected BossCore bossPrefab;
      [SerializeField] protected LineRenderer pathLinePrefab;
      [SerializeField] protected List<GridHexContentType> coverTypes;

      public PowerUpsManager PowerUpsManager => powerUpsManager;
      public GameObject SpinStrategy => spinStrategy;
      public CharacterCore HeroPrefab => heroPrefab;
      public BossCore BossPrefab => bossPrefab;
      public LineRenderer PathLinePrefab => pathLinePrefab;
      public List<GridHexContentType> CoverTypes => coverTypes;

      private void Awake() {
         Instance = this;
      }
   }
}