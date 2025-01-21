using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BossRushJam25.HexGrid.Glyphs.Editor {
   public class GlyphEditorWindow : EditorWindow {
      private Transform playgroundParent;
      private Transform spawnPosition;

      private ObjectField hexGridField;
      private ObjectField glyphDefinitionField;
      private Button generateButton;
      private Button saveButton;
      private Label errorLabel;
      private Button fixPositionsButton;
      private Button clearPlaygroundButton;

      private HexGridController hexGrid;

      private void OnEnable() {
         if (!playgroundParent) playgroundParent = FindObjectsByType<Transform>(FindObjectsSortMode.None).FirstOrDefault(t => t.name == $"{nameof(GlyphEditorWindow)}.playground");
         if (!playgroundParent) playgroundParent = new GameObject($"{nameof(GlyphEditorWindow)}.playground").transform;
         if (!spawnPosition) spawnPosition = playgroundParent.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == $"{nameof(GlyphEditorWindow)}.spawnPosition");
         if (!spawnPosition) spawnPosition = new GameObject($"{nameof(GlyphEditorWindow)}.spawnPosition").transform;
         spawnPosition.SetParent(playgroundParent);
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
         hexGrid = FindObjectsByType<HexGridController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault();
         hexGridField.value = hexGrid;
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
         spawnPosition.SetSiblingIndex(0);
         while (playgroundParent.childCount > 1) {
            DestroyImmediate(playgroundParent.GetChild(1).gameObject);
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
         fixPositionsButton.visible = playgroundParent;
         generateButton.visible = hexGrid && playgroundParent && glyphDefinitionField != null && glyphDefinitionField.value;
         saveButton.visible = hexGrid && playgroundParent && glyphDefinitionField != null && glyphDefinitionField.value;
      }

      private void HandleGenerateClicked() {
         errorLabel.text = string.Empty;
         ClearPlayground();
         var so = new SerializedObject(glyphDefinitionField.value);
         spawnPosition.position = so.FindProperty("spawnOffsetWithOrigin").vector3Value;
         GenerateOneHex(so.FindProperty("originGlyphChunk"), Vector3.zero, 0);
         for (var i = 0; i < so.FindProperty("otherGlyphParts").arraySize; ++i) {
            var otherGlyphPartProperty = so.FindProperty("otherGlyphParts").GetArrayElementAtIndex(i);
            GenerateOneHex(otherGlyphPartProperty.FindPropertyRelative("glyphChunk"),
               otherGlyphPartProperty.FindPropertyRelative("offsetWithOrigin").vector3Value,
               otherGlyphPartProperty.FindPropertyRelative("rotationWithOrigin").floatValue);
         }
      }

      private void GenerateOneHex(SerializedProperty glyphChunkProperty, Vector3 offset, float rotation) {
         if (glyphChunkProperty.objectReferenceValue) {
            var hex = (GlyphChunk)PrefabUtility.InstantiatePrefab(glyphChunkProperty.objectReferenceValue);
            hex.transform.SetParent(playgroundParent);
            hex.transform.position = offset;
            hex.transform.rotation = Quaternion.Euler(0, rotation, 0);
         }
      }

      private void HandleSaveToSoClicked() {
         FixHexPositions();
         errorLabel.text = string.Empty;
         var glyphChunks = playgroundParent.GetComponentsInChildren<GlyphChunk>();

         if (glyphChunks.Length < 2) {
            errorLabel.text += "There cannot be a glyph with less than 2 hexes.";
            return;
         }

         var origin = glyphChunks.OrderBy(t => t.transform.position.sqrMagnitude).FirstOrDefault();
         var so = new SerializedObject(glyphDefinitionField.value);
         so.Update();
         SaveGlyphHexPreset(so.FindProperty("originGlyphChunk"), origin);
         so.FindProperty("spawnOffsetWithOrigin").vector3Value = origin.transform.InverseTransformPoint(spawnPosition.position);
         so.FindProperty("otherGlyphParts").arraySize = glyphChunks.Length - 1;
         foreach (var otherChunk in glyphChunks.Except(new[] { origin }).Select((t, i) => (chunk: t, index: i))) {
            var arrayElement = so.FindProperty("otherGlyphParts").GetArrayElementAtIndex(otherChunk.index);
            arrayElement.FindPropertyRelative("offsetWithOrigin").vector3Value = origin.transform.InverseTransformPoint(otherChunk.chunk.transform.position);
            arrayElement.FindPropertyRelative("rotationWithOrigin").floatValue = Vector3.SignedAngle(origin.transform.forward, otherChunk.chunk.transform.forward, Vector3.up);
            SaveGlyphHexPreset(arrayElement.FindPropertyRelative("glyphChunk"), otherChunk.chunk);
         }

         so.ApplyModifiedProperties();
      }

      private void SaveGlyphHexPreset(SerializedProperty property, GlyphChunk glyphChunk) {
         if (!glyphChunk) {
            errorLabel.text += "GlyphChunk not defined.\n";
            return;
         }
         if (!PrefabUtility.IsAnyPrefabInstanceRoot(glyphChunk.gameObject)) {
            errorLabel.text += $"GlyphChunk {glyphChunk.name} is not an instance of a prefab.\n";
            return;
         }

         property.objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(glyphChunk));
      }

      private void Update() {
         if (EditorApplication.isPlaying) {
            DestroyImmediate(playgroundParent.gameObject);
            Close();
         }
      }
   }
}