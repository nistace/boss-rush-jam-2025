using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BossRushJam25.HexGrid.Glyphs.Editor {
   public class GlyphEditorWindow : EditorWindow {
      private Transform playgroundParent;

      private ObjectField hexGridField;
      private ObjectField glyphDefinitionField;
      private Button generateButton;
      private Button saveButton;
      private Label errorLabel;
      private Button fixPositionsButton;
      private Button clearPlaygroundButton;

      private HexGridController hexGrid;

      private void OnEnable() {
         if (!playgroundParent) playgroundParent = new GameObject($"{nameof(GlyphEditorWindow)}.playground").transform;
      }

      private void OnDisable() {
         if (playgroundParent) DestroyImmediate(playgroundParent.gameObject);
      }

      [MenuItem("BRJ25/" + nameof(GlyphEditorWindow))]
      public static void ShowExample() {
         var window = GetWindow<GlyphEditorWindow>();
         window.titleContent = new GUIContent("MyEditorWindow");
      }

      public void CreateGUI() {
         hexGridField = new ObjectField { objectType = typeof(HexGridController), allowSceneObjects = true };
         hexGridField.RegisterValueChangedCallback(HandleSelectedHexGridChanged);
         hexGridField.value = FindObjectsByType<HexGridController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault();
         rootVisualElement.Add(hexGridField);

         glyphDefinitionField = new ObjectField { objectType = typeof(GlyphDefinition) };
         glyphDefinitionField.RegisterValueChangedCallback(HandleSelectedGlyphDefinitionChanged);
         rootVisualElement.Add(glyphDefinitionField);

         fixPositionsButton = new Button { name = "button", text = "Fix Hex Positions" };
         fixPositionsButton.clicked += FixHexPositions;
         rootVisualElement.Add(fixPositionsButton);

         generateButton = new Button { name = "button", text = "Generate" };
         generateButton.clicked += HandleGenerateClicked;
         rootVisualElement.Add(generateButton);

         saveButton = new Button { name = "button", text = "Save to SO" };
         saveButton.clicked += HandleSaveToSoClicked;
         rootVisualElement.Add(saveButton);

         clearPlaygroundButton = new Button { name = "button", text = "Clear Playground" };
         clearPlaygroundButton.clicked += ClearPlayground;
         rootVisualElement.Add(clearPlaygroundButton);

         errorLabel = new Label { name = "errorLabel", text = "Error", style = { color = Color.red } };
         rootVisualElement.Add(errorLabel);

         RefreshButtons();
      }

      private void ClearPlayground() {
         errorLabel.text = string.Empty;
         while (playgroundParent.childCount > 0) {
            DestroyImmediate(playgroundParent.GetChild(0).gameObject);
         }
      }

      private void FixHexPositions() {
         if (!hexGrid) return;
         hexGrid.RefreshInnerRadius();
         foreach (var hex in playgroundParent.GetComponentsInChildren<GridHex>()) {
            hex.transform.position = hexGrid.CoordinatesToWorldPosition(hexGrid.WorldToCoordinates(hex.transform.position));
            hex.transform.rotation = Quaternion.Euler(0, 60 * Mathf.RoundToInt(hex.transform.rotation.eulerAngles.y / 60), 0);
         }
      }

      private void HandleSelectedGlyphDefinitionChanged(ChangeEvent<Object> evt) => RefreshButtons();

      private void HandleSelectedHexGridChanged(ChangeEvent<Object> evt) {
         hexGrid = evt.newValue as HexGridController;
         RefreshButtons();
      }

      private void RefreshButtons() {
         errorLabel.text = string.Empty;
         fixPositionsButton.visible = hexGrid && playgroundParent && glyphDefinitionField != null && glyphDefinitionField.value;
         generateButton.visible = hexGrid && playgroundParent && glyphDefinitionField != null && glyphDefinitionField.value;
         saveButton.visible = hexGrid && playgroundParent && glyphDefinitionField != null && glyphDefinitionField.value;
      }

      private void HandleGenerateClicked() {
         errorLabel.text = string.Empty;
         ClearPlayground();
         var so = new SerializedObject(glyphDefinitionField.value);
         GenerateOneHex(so.FindProperty("originGlyphPart"), Vector3.zero, 0);
         for (var i = 0; i < so.FindProperty("otherGlyphParts").arraySize; ++i) {
            var otherGlyphPartProperty = so.FindProperty("otherGlyphParts").GetArrayElementAtIndex(i);
            GenerateOneHex(otherGlyphPartProperty.FindPropertyRelative("otherGlyphPart"),
               otherGlyphPartProperty.FindPropertyRelative("offsetWithOrigin").vector3Value,
               otherGlyphPartProperty.FindPropertyRelative("rotationWithOrigin").floatValue);
         }
      }

      private void GenerateOneHex(SerializedProperty gridHexPresetProperty, Vector3 offset, float rotation) {
         if (gridHexPresetProperty.FindPropertyRelative("hexPrefab").objectReferenceValue) {
            var hex = (GridHex)PrefabUtility.InstantiatePrefab(gridHexPresetProperty.FindPropertyRelative("hexPrefab").objectReferenceValue);
            if (gridHexPresetProperty.FindPropertyRelative("hexPrefab").objectReferenceValue) {
               var hexContent = (GridHexContent)PrefabUtility.InstantiatePrefab(gridHexPresetProperty.FindPropertyRelative("contentPrefab").objectReferenceValue);
               hexContent.transform.SetParent(hex.transform, false);
            }
            hex.transform.SetParent(playgroundParent);
            hex.transform.position = offset;
            hex.transform.rotation = Quaternion.Euler(0, rotation, 0);
         }
      }

      private void HandleSaveToSoClicked() {
         FixHexPositions();
         errorLabel.text = string.Empty;
         var hexes = playgroundParent.GetComponentsInChildren<GridHex>();

         if (hexes.Length < 2) {
            errorLabel.text += "There cannot be a glyph with less than 2 hexes.";
            return;
         }

         var origin = hexes.OrderBy(t => t.transform.position.sqrMagnitude).FirstOrDefault();
         var so = new SerializedObject(glyphDefinitionField.value);
         so.Update();
         SaveGlyphHexPreset(so.FindProperty("originGlyphPart"), origin);
         so.FindProperty("otherGlyphParts").arraySize = hexes.Length - 1;
         foreach (var otherHex in hexes.Except(new[] { origin }).Select((t, i) => (hex: t, index: i))) {
            var arrayElement = so.FindProperty("otherGlyphParts").GetArrayElementAtIndex(otherHex.index);
            arrayElement.FindPropertyRelative("offsetWithOrigin").vector3Value = otherHex.hex.transform.position - origin.transform.position;
            arrayElement.FindPropertyRelative("rotationWithOrigin").floatValue = Vector3.SignedAngle(origin.transform.forward, otherHex.hex.transform.forward, Vector3.up);
            SaveGlyphHexPreset(arrayElement.FindPropertyRelative("otherGlyphPart"), otherHex.hex);
         }

         so.ApplyModifiedProperties();
      }

      private void SaveGlyphHexPreset(SerializedProperty property, GridHex gridHex) {
         if (!gridHex) {
            errorLabel.text += "GridHex not defined.\n";
            return;
         }
         if (!PrefabUtility.IsAnyPrefabInstanceRoot(gridHex.gameObject)) {
            errorLabel.text += $"GridHex {gridHex.name} is not an instance of a prefab.\n";
            return;
         }

         property.FindPropertyRelative("hexPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gridHex));
         property.FindPropertyRelative("contentPrefab").objectReferenceValue = null;

         var content = gridHex.GetComponentInChildren<GridHexContent>();
         if (!content) {
            errorLabel.text += $"GridHex {gridHex.name} has no content. That's weird for a glyph.\n";
            return;
         }
         if (!PrefabUtility.IsAnyPrefabInstanceRoot(gridHex.gameObject)) {
            errorLabel.text += $"GridHex {gridHex.name} has a content " + content.name + ", but this content is not an instance of a prefab.\n";
            return;
         }

         property.FindPropertyRelative("contentPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(content));
      }
   }
}