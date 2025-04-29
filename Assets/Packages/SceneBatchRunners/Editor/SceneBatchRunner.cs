using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace SceneBatchRunners {
public abstract class SceneBatchRunner : EditorWindow
{
    #region Field

    [SerializeField]
    private List<Object>       _scenes;
    private SerializedObject   _serializedObject;
    private SerializedProperty _scenesProperty;

    private ReorderableList _reorderableList;

    #endregion Field

    #region Property

    protected const string MenuPathBase = "Custom/" + nameof(SceneBatchRunners) + "/";

    #endregion Property

    #region Method

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        _serializedObject = new SerializedObject(this);
        _scenesProperty   = _serializedObject.FindProperty(nameof(_scenes));
        _reorderableList  = new ReorderableList(_serializedObject, _scenesProperty);

        _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = _scenesProperty.GetArrayElementAtIndex(index);

            element.objectReferenceValue = EditorGUI.ObjectField
                (rect, element.objectReferenceValue, typeof(SceneAsset), false);
        };

        _reorderableList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Scenes");

            const string clearMark = "\u229d";

            var buttonSize = GUI.skin.label.CalcSize(new GUIContent(clearMark));
            var buttonRect = new Rect(rect.width - buttonSize.x, rect.y, buttonSize.x, rect.height);

            if (GUI.Button(buttonRect, new GUIContent(clearMark, "Clear List"), GUI.skin.label))
            {
                _scenes.Clear();
            }
        };
    }

    private void OnGUI()
    {
        _serializedObject.Update();
        _reorderableList.DoLayoutList();
        _serializedObject.ApplyModifiedProperties();

        HandleDragAndDrop();

        if (GUILayout.Button(nameof(Run)))
        {
            foreach (var scene in _scenes)
            {
                Debug.Log($"Scene '{scene.name}' opened.");

                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
                Run(scene as SceneAsset);

                Debug.Log($"Scene '{scene.name}' closed.");
            }
        }
    }

    private void HandleDragAndDrop()
    {
        if (Event.current.type == EventType.DragUpdated
         && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            if (DragAndDrop.objectReferences.All(obj => obj is SceneAsset))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        if (Event.current.type == EventType.DragPerform
         && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            if (DragAndDrop.objectReferences.All(obj => obj is SceneAsset))
            {
                foreach (var obj in DragAndDrop.objectReferences.Cast<SceneAsset>())
                {
                    _scenes.Add(obj);
                }

                DragAndDrop.AcceptDrag();
                Event.current.Use();
            }
        }
    }

    protected abstract void Run(SceneAsset scene);
    
    // NOTE:
    // Setting the dirty flag and saving the scene should be handled in the derived class.
    // EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    // EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

    #endregion Method
}}