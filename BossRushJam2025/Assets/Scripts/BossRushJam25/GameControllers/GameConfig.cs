using System.Collections.Generic;
using BossRushJam25.Character;
using BossRushJam25.HexGrid;
using BossRushJam25.PowerUps;
using UnityEngine;

namespace BossRushJam25.GameControllers {
   public class GameConfig : MonoBehaviour {
      public static GameConfig Instance { get; private set; }

      [SerializeField] protected GameObject spinStrategy;
      [SerializeField] protected PowerUpsManager powerUpsManager;
      [SerializeField] protected CharacterCore heroPrefab;
      [SerializeField] protected CharacterCore bossPrefab;
      [SerializeField] protected LineRenderer pathLinePrefab;
      [SerializeField] protected Gradient actionPreviewsGradient;
      [SerializeField] protected List<GridHexContentType> coverTypes;

      public PowerUpsManager PowerUpsManager => powerUpsManager;
      public GameObject SpinStrategy => spinStrategy;
      public CharacterCore HeroPrefab => heroPrefab;
      public CharacterCore BossPrefab => bossPrefab;
      public LineRenderer PathLinePrefab => pathLinePrefab;
      public Gradient ActionPreviewsGradient => actionPreviewsGradient;
      public List<GridHexContentType> CoverTypes => coverTypes;

      private void Awake() {
         Instance = this;
      }
   }
}