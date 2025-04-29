using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneBatchRunners {
public class SampleSceneBatchRunner : SceneBatchRunner
{
    [MenuItem(MenuPathBase + nameof(SampleSceneBatchRunner))]
    public static void ShowWindow()
    {
        GetWindow<SampleSceneBatchRunner>(nameof(SampleSceneBatchRunner));
    }

    protected override void Run(SceneAsset scene)
    {
        Debug.Log("RUN : " + scene.name);

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
}}