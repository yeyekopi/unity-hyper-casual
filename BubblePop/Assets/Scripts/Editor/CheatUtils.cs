using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatUtils {
    // # -> Shift
    // % -> Ctrl
    // & -> Alt
    // _ -> (needed for single key) e.g. _5

    [MenuItem("Tools/Reboot &Del")]
    public static void Reboot() {
        Contexts.sharedInstance.state.Shutdown();
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
        RenderingExtensions.ClearFields();
    }
    //
    // [MenuItem("Toos/DoCodePatcher")]
    // public static void 
}
