#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Toggles Developer Mode via an in-game menu and manages the developer workflow scenes.
/// When enabled, ensures required scenes (Persistent, Environment, and Programmable) are loaded.
/// When disabled, returns to the normal runtime setup.
/// </summary>

public static class ModuleDevMode
{
    const string PATH_PERSISTENT = "Assets/_Core/Scenes/Persistent.unity";
    const string PATH_MAIN = "Assets/Scenes/MainScene.unity";
    const string PATH_SAMPLE = "Assets/Scenes/SampleScene.unity";

    const string PREF_KEY = "ModuleDevMode_Enabled";

    static readonly string[] PROTECTED_SCENES = new[]
        {
            PATH_PERSISTENT,
            PATH_MAIN,
        };

    [MenuItem("GoYeast/Enable Dev Mode", priority = 10)]
    public static void Enable()
    {
        if (!ConfirmSaveIfDirty()) return;

        EditorPrefs.SetBool(PREF_KEY, true);

        OpenScenesForDev();

        // Try to activate the protected scene as read-only
        SetReadonlyFlag(true);

        EditorUtility.DisplayDialog("Module Development Mode",
    "Enabled:\n- Automatically open Persistent / MainScene / SampleScene\n- Set SampleScene as the active scene\n- Disable saving for Persistent / MainScene",
    "OK");
    }

    [MenuItem("GoYeast/Disable Dev Mode", priority = 11)]
    public static void Disable()
    {
        if (!ConfirmSaveIfDirty()) return;

        EditorPrefs.DeleteKey(PREF_KEY);
        SetReadonlyFlag(false);

        EditorUtility.DisplayDialog("Personal Scene Development Mode",
    "Disabled: Saving all scenes is now allowed (still recommended not to modify core scenes directly).",
    "OK");
    }

    [MenuItem("GoYeast/Enable Dev Mode", true)]
    static bool ValidateEnable() => !EditorPrefs.GetBool(PREF_KEY, false);

    [MenuItem("GoYeast/Disable Dev Mode", true)]
    static bool ValidateDisable() => EditorPrefs.GetBool(PREF_KEY, false);

    static void OpenScenesForDev()
    {
        var persistent = EditorSceneManager.OpenScene(PATH_PERSISTENT, OpenSceneMode.Single);
        var houseMain = EditorSceneManager.OpenScene(PATH_MAIN, OpenSceneMode.Additive);

        Scene sample;
        if (File.Exists(PATH_SAMPLE))
            sample = EditorSceneManager.OpenScene(PATH_SAMPLE, OpenSceneMode.Additive);
        else
            sample = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);

        EditorSceneManager.SetActiveScene(sample);
    }

    static bool ConfirmSaveIfDirty()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return true;
        return false;
    }

    static void SetReadonlyFlag(bool readOnly)
    {
        foreach (var p in PROTECTED_SCENES)
        {
            var full = Path.GetFullPath(p);
            if (!File.Exists(full)) continue;
            var attr = File.GetAttributes(full);
            if (readOnly) attr |= FileAttributes.ReadOnly;
            else attr &= ~FileAttributes.ReadOnly;
            File.SetAttributes(full, attr);
        }
    }

    /// <summary>
    /// When in "Module Development Mode" block saving for protected scenes.
    /// </summary>
    public class ProtectCoreScenesSaver : AssetModificationProcessor
    {
        static readonly HashSet<string> Protected = new HashSet<string>(new[]
        {
            "Assets/_Core/Scenes/Persistent.unity",
            "Assets/Scenes/MainScene.unity",
        });

        static bool DevModeOn => EditorPrefs.GetBool("ModuleDevMode_Enabled", false);

        public static string[] OnWillSaveAssets(string[] paths)
        {
            if (!DevModeOn || paths == null || paths.Length == 0) return paths;

            var list = new List<string>(paths.Length);
            bool blockedAny = false;

            foreach (var p in paths)
            {
                if (p.EndsWith(".unity"))
                {
                    var norm = p.Replace('\\', '/');

                    if (Protected.Contains(norm))
                    {
                        blockedAny = true;
                        Debug.LogWarning($"Blocked saving of protected scene: {norm} (Personal Scene Development Mode enabled)");
                        continue;
                    }
                }
                list.Add(p);
            }

            if (blockedAny)
            {
                EditorUtility.DisplayDialog(
    "Saving Core Scene Blocked",
    "You are currently in 'Personal Scene Development Mode', where modifying or saving Persistent or MainScene is not allowed.\n" +
    "Please make your changes in your personal or module scenes instead.",
    "OK");
            }

            return list.ToArray();
        }
    }
}
#endif