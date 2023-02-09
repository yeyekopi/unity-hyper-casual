// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/12/24 13:50

using System;
using System.IO;
using System.Reflection;
using System.Text;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
    public static class EditorUtils
    {
        public static string projectPath { get; private set; } // Without final slash
        public static string assetsPath { get; private set; } // Without final slash
        public static bool hasPro { get { if (!_hasCheckedForPro) CheckForPro(); return _hasPro; } }
        public static string proVersion { get { if (!_hasCheckedForPro) CheckForPro(); return _proVersion; } }
        // Editor path from Assets (not included) with final slash, in AssetDatabase format (/)
        // With final slash (system based) - might be NULL in case users are not using a parent Demigiant folder
        public static bool isOSXEditor { get; private set; }
        public static string pathSlash { get; private set; } // for full paths
        public static string pathSlashToReplace { get; private set; } // for full paths

        static readonly StringBuilder _Strb = new StringBuilder();
        static bool _hasPro;
        static string _proVersion;
        static bool _hasCheckedForPro;

        static EditorUtils()
        {
            isOSXEditor = Application.platform == RuntimePlatform.OSXEditor;
            bool useWindowsSlashes = Application.platform == RuntimePlatform.WindowsEditor;
            pathSlash = useWindowsSlashes ? "\\" : "/";
            pathSlashToReplace = useWindowsSlashes ? "/" : "\\";

            projectPath = Application.dataPath;
            projectPath = projectPath.Substring(0, projectPath.LastIndexOf("/"));
            projectPath = projectPath.Replace(pathSlashToReplace, pathSlash);

            assetsPath = projectPath + pathSlash + "Assets";
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        public static void DelayedCall(float delay, Action callback)
        {
            new DelayedCall(delay, callback);
        }

        /// <summary>
        /// Checks that the given editor texture use the correct import settings,
        /// and applies them if they're incorrect.
        /// </summary>
        public static void SetEditorTexture(Texture2D texture, FilterMode filterMode = FilterMode.Point, int maxTextureSize = 32)
        {
            if (texture.wrapMode == TextureWrapMode.Clamp) return;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            tImporter.textureType = TextureImporterType.GUI;
            tImporter.npotScale = TextureImporterNPOTScale.None;
            tImporter.filterMode = filterMode;
            tImporter.wrapMode = TextureWrapMode.Clamp;
            tImporter.maxTextureSize = maxTextureSize;
            tImporter.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(path);
        }

        /// <summary>
        /// Returns TRUE if the file/directory at the given path exists.
        /// </summary>
        /// <param name="adbPath">Path, relative to Unity's project folder</param>
        /// <returns></returns>
        public static bool AssetExists(string adbPath)
        {
            string fullPath = ADBPathToFullPath(adbPath);
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }

        /// <summary>
        /// Converts the given project-relative path to a full path,
        /// with backward (\) slashes).
        /// </summary>
        public static string ADBPathToFullPath(string adbPath)
        {
            adbPath = adbPath.Replace(pathSlashToReplace, pathSlash);
            return projectPath + pathSlash + adbPath;
        }

        /// <summary>
        /// Converts the given full path to a path usable with AssetDatabase methods
        /// (relative to Unity's project folder, and with the correct Unity forward (/) slashes).
        /// </summary>
        public static string FullPathToADBPath(string fullPath)
        {
            string adbPath = fullPath.Substring(projectPath.Length + 1);
            return adbPath.Replace("\\", "/");
        }

        /// <summary>
        /// Connects to a <see cref="ScriptableObject"/> asset.
        /// If the asset already exists at the given path, loads it and returns it.
        /// Otherwise, either returns NULL or automatically creates it before loading and returning it
        /// (depending on the given parameters).
        /// </summary>
        /// <typeparam name="T">Asset type</typeparam>
        /// <param name="adbFilePath">File path (relative to Unity's project folder)</param>
        /// <param name="createIfMissing">If TRUE and the requested asset doesn't exist, forces its creation</param>
        public static T ConnectToSourceAsset<T>(string adbFilePath, bool createIfMissing = false) where T : ScriptableObject
        {
            if (!AssetExists(adbFilePath)) {
                if (createIfMissing) CreateScriptableAsset<T>(adbFilePath);
                else return null;
            }
            T source = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
            if (source == null) {
                // Source changed (or editor file was moved from outside of Unity): overwrite it
                CreateScriptableAsset<T>(adbFilePath);
                source = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
            }
            return source;
        }

        /// <summary>
        /// Full path for the given loaded assembly, assembly file included
        /// </summary>
        public static string GetAssemblyFilePath(Assembly assembly)
        {

            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            if (path.Substring(path.Length - 3) == "dll") return path;

//            string codeBase = assembly.CodeBase;
//            UriBuilder uri = new UriBuilder(codeBase);
//            string path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
//            string lastChar = path.Substring(path.Length - 4);
//            if (lastChar == "dll") return path;

            // Invalid path, use Location
            return Path.GetFullPath(assembly.Location);
        }

        /// <summary>
        /// Adds the given global define if it's not already present
        /// </summary>
        public static void AddGlobalDefine(string id)
        {
            bool added = false;
            int totGroupsModified = 0;
            BuildTargetGroup[] targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
            foreach(BuildTargetGroup btg in targetGroups) {
                if (!IsValidBuildTargetGroup(btg)) continue;
                string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
                string[] singleDefs = defs.Split(';');
                if (Array.IndexOf(singleDefs, id) != -1) continue; // Already present
                added = true;
                totGroupsModified++;
                defs += defs.Length > 0 ? ";" + id : id;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, defs);
            }
            if (added) Debug.Log(string.Format("DOTween : added global define \"{0}\" to {1} BuildTargetGroups", id, totGroupsModified));
        }

        /// <summary>
        /// Removes the given global define if it's present
        /// </summary>
        public static void RemoveGlobalDefine(string id)
        {
            bool removed = false;
            int totGroupsModified = 0;
            BuildTargetGroup[] targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
            foreach(BuildTargetGroup btg in targetGroups) {
                if (!IsValidBuildTargetGroup(btg)) continue;
                string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
                string[] singleDefs = defs.Split(';');
                if (Array.IndexOf(singleDefs, id) == -1) continue; // Not present
                removed = true;
                totGroupsModified++;
                _Strb.Length = 0;
                for (int i = 0; i < singleDefs.Length; ++i) {
                    if (singleDefs[i] == id) continue;
                    if (_Strb.Length > 0) _Strb.Append(';');
                    _Strb.Append(singleDefs[i]);
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, _Strb.ToString());
            }
            _Strb.Length = 0;
            if (removed) Debug.Log(string.Format("DOTween : removed global define \"{0}\" from {1} BuildTargetGroups", id, totGroupsModified));
        }

        /// <summary>
        /// Returns TRUE if the given global define is present in all the <see cref="BuildTargetGroup"/>
        /// or only in the given <see cref="BuildTargetGroup"/>, depending on passed parameters.<para/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to check in all of them.</param>
        public static bool HasGlobalDefine(string id, BuildTargetGroup? buildTargetGroup = null)
        {
            BuildTargetGroup[] targetGroups = buildTargetGroup == null
                ? (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))
                : new[] {(BuildTargetGroup)buildTargetGroup};
            foreach(BuildTargetGroup btg in targetGroups) {
                if (!IsValidBuildTargetGroup(btg)) continue;
                string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
                string[] singleDefs = defs.Split(';');
                if (Array.IndexOf(singleDefs, id) != -1) return true;
            }
            return false;
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void CheckForPro()
        {
            _hasCheckedForPro = true;
            try {
                Assembly additionalAssembly = Assembly.Load("DOTweenPro, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                _proVersion = additionalAssembly.GetType("DG.Tweening.DOTweenPro").GetField("Version", BindingFlags.Static | BindingFlags.Public).GetValue(null) as string;
                _hasPro = true;
            } catch {
                // No DOTweenPro present
                _hasPro = false;
                _proVersion = "-";
            }
        }

        static void CreateScriptableAsset<T>(string adbFilePath) where T : ScriptableObject
        {
            T data = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(data, adbFilePath);
        }

        static bool IsValidBuildTargetGroup(BuildTargetGroup group)
        {
            if (group == BuildTargetGroup.Unknown) return false;
            Type moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
//            MethodInfo miIsPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo miGetTargetStringFromBuildTargetGroup = moduleManager.GetMethod(
                "GetTargetStringFromBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic
            );
            MethodInfo miGetPlatformName = typeof(PlayerSettings).GetMethod(
                "GetPlatformName", BindingFlags.Static | BindingFlags.NonPublic
            );
            string targetString = (string)miGetTargetStringFromBuildTargetGroup.Invoke(null, new object[] {group});
            string platformName = (string)miGetPlatformName.Invoke(null, new object[] {group});

            // Group is valid if at least one betweeen targetString and platformName is not empty.
            // This seems to me the safest and more reliant way to check,
            // since ModuleManager.IsPlatformSupportLoaded dosn't work well with BuildTargetGroup (only BuildTarget)
            bool isValid = !string.IsNullOrEmpty(targetString) || !string.IsNullOrEmpty(platformName);

//            Debug.Log((isValid ? "<color=#00ff00>" : "<color=#ff0000>") + group + " > " + targetString + " / " + platformName + " > "  + isValid + "/" + miIsPlatformSupportLoaded.Invoke(null, new object[] {group.ToString()}) + "</color>");
            return isValid;
        }
    }
}