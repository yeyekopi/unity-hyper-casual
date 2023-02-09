// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/12/24 13:37

using System.IO;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor.UI
{
    public class DOTweenUtilityWindow : EditorWindow
    {
        [MenuItem("Tools/Demigiant/" + _Title)]
        static void ShowWindow() { Open(); }
		
        const string _Title = "DOTween Utility Panel";
        static readonly Vector2 _WinSize = new Vector2(370,600);
        public const string Id = "DOTweenVersion";
        public const string IdPro = "DOTweenProVersion";
        static readonly float _HalfBtSize = _WinSize.x * 0.5f - 6;

        DOTweenSettings _src;
        string _innerTitle;

        int _selectedTab;
        string[] _tabLabels = new[] { "Setup", "Preferences" };

        // If force is FALSE opens the window only if DOTween's version has changed
        // (set to FALSE by OnPostprocessAllAssets).<para/>
        // NOTE: this is also called via Reflection by UpgradeWindow
        public static void Open()
        {
            DOTweenUtilityWindow window = EditorWindow.GetWindow<DOTweenUtilityWindow>(true, _Title, true);
            window.minSize = _WinSize;
            window.maxSize = _WinSize;
            window.ShowUtility();
            EditorPrefs.SetString(Id, DOTween.Version);
            EditorPrefs.SetString(IdPro, EditorUtils.proVersion);
        }

        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        void OnHierarchyChange() { Repaint(); }

        void OnEnable()
        {
#if COMPATIBLE
            _innerTitle = "DOTween v" + DOTween.Version + " [Compatibility build]";
#else
            _innerTitle = "DOTween v" + DOTween.Version + (TweenManager.isDebugBuild ? " [Debug build]" : " [Release build]");
#endif
            if (EditorUtils.hasPro) _innerTitle += "\nDOTweenPro v" + EditorUtils.proVersion;
            else _innerTitle += "\nDOTweenPro not installed";

        }

        void OnDestroy()
        {
            if (_src != null) {
                EditorUtility.SetDirty(_src);
            }
        }

        void OnGUI()
        {
            EditorGUIUtils.SetGUIStyles();
            Connect();
            if (Application.isPlaying) {
                GUILayout.Space(40);
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                GUILayout.Label("DOTween Utility Panel\nis disabled while in Play Mode", EditorGUIUtils.wrapCenterLabelStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(40);
                GUILayout.EndHorizontal();
            } else {
                Rect areaRect = new Rect(0, 0, 160, 30);
                _selectedTab = GUI.Toolbar(areaRect, _selectedTab, _tabLabels);

                switch (_selectedTab) {
                case 1:
                    float labelW = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 160;
                    DrawPreferencesGUI();
                    EditorGUIUtility.labelWidth = labelW;
                    break;
                default:
                    DrawSetupGUI();
                    break;
                }
            }

            if (GUI.changed) EditorUtility.SetDirty(_src);
        }

        // ===================================================================================
        // GUI METHODS -----------------------------------------------------------------------

        void DrawSetupGUI()
        {
            GUILayout.Space(40);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Website", EditorGUIUtils.btBigStyle, GUILayout.Width(_HalfBtSize))) Application.OpenURL("http://dotween.demigiant.com/index.php");
            if (GUILayout.Button("Get Started", EditorGUIUtils.btBigStyle, GUILayout.Width(_HalfBtSize))) Application.OpenURL("http://dotween.demigiant.com/getstarted.php");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Documentation", EditorGUIUtils.btBigStyle, GUILayout.Width(_HalfBtSize))) Application.OpenURL("http://dotween.demigiant.com/documentation.php");
            if (GUILayout.Button("Support", EditorGUIUtils.btBigStyle, GUILayout.Width(_HalfBtSize))) Application.OpenURL("http://dotween.demigiant.com/support.php");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Changelog", EditorGUIUtils.btBigStyle, GUILayout.Width(_HalfBtSize))) Application.OpenURL("http://dotween.demigiant.com/download.php");
            if (GUILayout.Button("Check Updates", EditorGUIUtils.btBigStyle, GUILayout.Width(_HalfBtSize))) Application.OpenURL("http://dotween.demigiant.com/download.php?v=" + DOTween.Version);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        void DrawPreferencesGUI()
        {
            GUILayout.Space(40);
            if (GUILayout.Button("Reset", EditorGUIUtils.btBigStyle)) {
                // Reset to original defaults
                _src.useSafeMode = true;
                _src.safeModeOptions.nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
                _src.showUnityEditorReport = false;
                _src.timeScale = 1;
                _src.useSmoothDeltaTime = false;
                _src.maxSmoothUnscaledTime = 0.15f;
                _src.rewindCallbackMode = RewindCallbackMode.FireIfPositionChanged;
                _src.logBehaviour = LogBehaviour.ErrorsOnly;
                _src.drawGizmos = true;
                _src.defaultRecyclable = false;
                _src.defaultAutoPlay = AutoPlay.All;
                _src.defaultUpdateType = UpdateType.Normal;
                _src.defaultTimeScaleIndependent = false;
                _src.defaultEaseType = Ease.OutQuad;
                _src.defaultEaseOvershootOrAmplitude = 1.70158f;
                _src.defaultEasePeriod = 0;
                _src.defaultAutoKill = true;
                _src.defaultLoopType = LoopType.Restart;
                EditorUtility.SetDirty(_src);
            }
            GUILayout.Space(8);
            _src.useSafeMode = EditorGUILayout.Toggle("Safe Mode", _src.useSafeMode);
            if (_src.useSafeMode) {
                _src.safeModeOptions.nestedTweenFailureBehaviour = (NestedTweenFailureBehaviour)EditorGUILayout.EnumPopup(
                    new GUIContent("└ On Nested Tween Failure", "Behaviour in case a tween inside a Sequence fails"),
                    _src.safeModeOptions.nestedTweenFailureBehaviour
                );
            }
            _src.timeScale = EditorGUILayout.FloatField("DOTween's TimeScale", _src.timeScale);
            _src.useSmoothDeltaTime = EditorGUILayout.Toggle("Smooth DeltaTime", _src.useSmoothDeltaTime);
            _src.maxSmoothUnscaledTime = EditorGUILayout.Slider("Max SmoothUnscaledTime", _src.maxSmoothUnscaledTime, 0.01f, 1f);
            _src.rewindCallbackMode = (RewindCallbackMode)EditorGUILayout.EnumPopup("OnRewind Callback Mode", _src.rewindCallbackMode);
            GUILayout.Space(-5);
            GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth + 4);
                EditorGUILayout.HelpBox(
                    _src.rewindCallbackMode == RewindCallbackMode.FireIfPositionChanged
                        ? "When calling Rewind or PlayBackwards/SmoothRewind, OnRewind callbacks will be fired only if the tween isn't already rewinded"
                        : _src.rewindCallbackMode == RewindCallbackMode.FireAlwaysWithRewind
                            ? "When calling Rewind, OnRewind callbacks will always be fired, even if the tween is already rewinded."
                            : "When calling Rewind or PlayBackwards/SmoothRewind, OnRewind callbacks will always be fired, even if the tween is already rewinded",
                    MessageType.None
                );
            GUILayout.EndHorizontal();
            _src.showUnityEditorReport = EditorGUILayout.Toggle("Editor Report", _src.showUnityEditorReport);
            _src.logBehaviour = (LogBehaviour)EditorGUILayout.EnumPopup("Log Behaviour", _src.logBehaviour);
            _src.drawGizmos = EditorGUILayout.Toggle("Draw Path Gizmos", _src.drawGizmos);
            
            GUILayout.Space(8);
            GUILayout.Label("DEFAULTS ▼");
            _src.defaultRecyclable = EditorGUILayout.Toggle("Recycle Tweens", _src.defaultRecyclable);
            _src.defaultAutoPlay = (AutoPlay)EditorGUILayout.EnumPopup("AutoPlay", _src.defaultAutoPlay);
            _src.defaultUpdateType = (UpdateType)EditorGUILayout.EnumPopup("Update Type", _src.defaultUpdateType);
            _src.defaultTimeScaleIndependent = EditorGUILayout.Toggle("TimeScale Independent", _src.defaultTimeScaleIndependent);
            _src.defaultEaseType = (Ease)EditorGUILayout.EnumPopup("Ease", _src.defaultEaseType);
            _src.defaultEaseOvershootOrAmplitude = EditorGUILayout.FloatField("Ease Overshoot", _src.defaultEaseOvershootOrAmplitude);
            _src.defaultEasePeriod = EditorGUILayout.FloatField("Ease Period", _src.defaultEasePeriod);
            _src.defaultAutoKill = EditorGUILayout.Toggle("AutoKill", _src.defaultAutoKill);
            _src.defaultLoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", _src.defaultLoopType);
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        public static DOTweenSettings GetDOTweenSettings()
        {
            return ConnectToSource(null, false, false);
        }

        static DOTweenSettings ConnectToSource(DOTweenSettings src, bool createIfMissing, bool fullSetup)
        {
            LocationData assetsLD = new LocationData(EditorUtils.assetsPath + EditorUtils.pathSlash + "Resources");

            if (src == null) {
                // Load eventual existing settings
                src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(assetsLD.adbFilePath, false);
            }
            if (src == null) {
                // Settings don't exist.
                if (!createIfMissing) return null; // Stop here
                // Create it in external folder
                if (!Directory.Exists(assetsLD.dir)) AssetDatabase.CreateFolder(assetsLD.adbParentDir, "Resources");
                src = EditorUtils.ConnectToSourceAsset<DOTweenSettings>(assetsLD.adbFilePath, true);
            }

            return src;
        }

        void Connect(bool forceReconnect = false)
        {
            if (_src != null && !forceReconnect) return;
            _src = ConnectToSource(_src, true, true);
        }


        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ||| INTERNAL CLASSES ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        struct LocationData
        {
            public string dir; // without final slash
            public string filePath;
            public string adbFilePath;
            public string adbParentDir; // without final slash

            public LocationData(string srcDir) : this()
            {
                dir = srcDir;
                filePath = dir + EditorUtils.pathSlash + DOTweenSettings.AssetFullFilename;
                adbFilePath = EditorUtils.FullPathToADBPath(filePath);
                adbParentDir = EditorUtils.FullPathToADBPath(dir.Substring(0, dir.LastIndexOf(EditorUtils.pathSlash)));
            }
        }
    }
}