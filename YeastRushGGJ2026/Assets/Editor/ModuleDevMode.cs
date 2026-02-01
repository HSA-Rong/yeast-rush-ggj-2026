#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quick scene workspace loader:
/// - Developer (Sample): Persistent + Main + Sample (active: Sample)
/// - Tester (Gameplay):  Persistent + Main + Gameplay (active: Gameplay)
/// No save-protection. Purpose: open required scenes fast.
/// </summary>

public static class ModuleDevMode
{
    // Core
    private const string PATH_PERSISTENT = "Assets/_Core/Scenes/Persistent.unity";
    private const string PATH_MAIN = "Assets/Scenes/MainScene.unity";

    // Workspaces
    private const string PATH_SAMPLE = "Assets/Scenes/SampleScene.unity";
    private const string PATH_GAMEPLAY = "Assets/_Gameplay/Scenes/GameplayScene.unity";

    // Optional: remember last workspace
    private const string PREF_WORKSPACE = "GoYeast.SceneWorkspace.Last";

    private const string MENU_ROOT = "GoYeast/";

    private enum Workspace
    {
        DeveloperSample = 0,
        TesterGameplay = 1,
    }

    [MenuItem(MENU_ROOT + "Developer (Sample)", priority = 10)]
    public static void OpenDeveloperSample()
    {
        if (!ConfirmSaveIfDirty()) return;
        OpenWorkspace(Workspace.DeveloperSample);
    }

    [MenuItem(MENU_ROOT + "Tester (Gameplay)", priority = 11)]
    public static void OpenTesterGameplay()
    {
        if (!ConfirmSaveIfDirty()) return;
        OpenWorkspace(Workspace.TesterGameplay);
    }

    // Optional: checkmarks (shows which one was used last)
    [MenuItem(MENU_ROOT + "Developer (Sample)", true)]
    private static bool ValidateDeveloperSample()
    {
        Menu.SetChecked(MENU_ROOT + "Developer (Sample)", GetLast() == Workspace.DeveloperSample);
        return true;
    }

    [MenuItem(MENU_ROOT + "Tester (Gameplay)", true)]
    private static bool ValidateTesterGameplay()
    {
        Menu.SetChecked(MENU_ROOT + "Tester (Gameplay)", GetLast() == Workspace.TesterGameplay);
        return true;
    }

    private static void OpenWorkspace(Workspace ws)
    {
        // Always load Persistent as Single, then add others.
        var persistent = OpenSceneOrWarn(PATH_PERSISTENT, OpenSceneMode.Single);
        if (!persistent.IsValid()) return;

        var main = OpenSceneOrWarn(PATH_MAIN, OpenSceneMode.Additive);
        if (!main.IsValid()) return;

        string thirdPath = ws == Workspace.DeveloperSample ? PATH_SAMPLE : PATH_GAMEPLAY;
        var third = OpenSceneOrWarn(thirdPath, OpenSceneMode.Additive);
        if (!third.IsValid()) return;

        EditorSceneManager.SetActiveScene(third);
        SetLast(ws);

        EditorUtility.DisplayDialog(
            "Scene Workspace Loaded",
            ws == Workspace.DeveloperSample
                ? "Opened: Persistent + MainScene + SampleScene (Active: SampleScene)"
                : "Opened: Persistent + MainScene + Gameplay (Active: Gameplay)",
            "OK");
    }

    private static Scene OpenSceneOrWarn(string path, OpenSceneMode mode)
    {
        var full = Path.GetFullPath(path);
        if (!File.Exists(full))
        {
            EditorUtility.DisplayDialog(
                "Scene Not Found",
                $"Missing scene file:\n{path}\n\nPlease check the path or add the scene to the project.",
                "OK");
            return default;
        }
        return EditorSceneManager.OpenScene(path, mode);
    }

    private static bool ConfirmSaveIfDirty()
    {
        // Unity will show the "Do you want to save?" dialog for modified scenes.
        return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }

    private static Workspace GetLast()
    {
        return (Workspace)EditorPrefs.GetInt(PREF_WORKSPACE, (int)Workspace.DeveloperSample);
    }

    private static void SetLast(Workspace ws)
    {
        EditorPrefs.SetInt(PREF_WORKSPACE, (int)ws);
    }
}
#endif