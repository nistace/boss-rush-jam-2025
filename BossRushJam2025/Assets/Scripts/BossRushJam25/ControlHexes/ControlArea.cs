using System;
using UnityEngine;

namespace BossRushJam25.ControlHexes {
   public class ControlArea : MonoBehaviour {
      [SerializeField] protected ControlHex hex;
      [SerializeField] private MeshRenderer meshRenderer;
      [SerializeField] private Color colorWhenHeroInside;
      [SerializeField] private Color colorWhenHeroOutside;

      public float ControlRadius => hex.Detector.DetectionRadius;

      private void OnTriggerEnter(Collider collider) {
         if (collider.gameObject.layer == LayerMask.NameToLayer("Hero")) {
            meshRenderer.material.SetColor("_Color", colorWhenHeroInside);
         }
      }

      private void OnTriggerExit(Collider collider) {
         if (collider.gameObject.layer == LayerMask.NameToLayer("Hero")) {
            meshRenderer.material.SetColor("_Color", colorWhenHeroOutside);
         }
      }

      private void OnValidate() {
         transform.localScale = new Vector3(ControlRadius * 2, 1f, ControlRadius * 2);
      }

      private void Update() {
         meshRenderer.enabled = hex.Active;
      }
   }
}