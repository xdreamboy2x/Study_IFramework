/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.Modules.Coroutine;
using System;

namespace IFramework
{
    class EditorEnv
    {
        private const string RelativeCorePath = "0.1Core";
        private const string RelativeCoreEditorPath = "0.1Core/Editor";
        private const string RelativeUtilPath = "UTil";
        private const string RelativeEditorPath = "UTil/Editor";
        private static string _Fpath;
        private const EnvironmentType envType = EnvironmentType.Ev0;

        public static string FrameworkName { get { return Framework.FrameworkName; } }
        public static string Author { get { return Framework.Author; } }
        public static string Version { get { return Framework.Version; } }
        public static string Description { get { return Framework.Description; } }
        public static string FrameworkPath { get {
                if (string.IsNullOrEmpty(_Fpath))
                {
                    string[] assetPaths = AssetDatabase.GetAllAssetPaths();
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        if (assetPaths[i].Contains(RelativeCorePath))
                        {
                            string tempPath = assetPaths[i];
                            int index = tempPath.IndexOf(RelativeCorePath);
                            _Fpath= tempPath.Substring(0, index);
                            break;
                        }
                    }
                }
                return _Fpath;
            }
        }
        public static string EditorPath { get { return FrameworkPath.CombinePath(RelativeEditorPath); } }
        public static string UtilPath { get { return FrameworkPath.CombinePath(RelativeUtilPath); } }
        public static string CorePath { get { return FrameworkPath.CombinePath(RelativeCorePath); } }
        public static string CoreEditorPath { get { return  FrameworkPath.CombinePath(RelativeCoreEditorPath); } }


        public static FrameworkEnvironment Env { get { return Framework.env0; } }


        public static event EditorApplication.CallbackFunction update { add { EditorApplication.update += value; } remove { EditorApplication.update -= value; } }
        public static event EditorApplication.CallbackFunction delayCall { add { EditorApplication.delayCall += value; } remove { EditorApplication.delayCall -= value; } }
        public static event EditorApplication.CallbackFunction searchChanged { add { EditorApplication.searchChanged += value; } remove { EditorApplication.searchChanged -= value; } }
        public static event EditorApplication.CallbackFunction modifierKeysChanged { add { EditorApplication.modifierKeysChanged += value; } remove { EditorApplication.modifierKeysChanged -= value; } }
        public static event EditorApplication.SerializedPropertyCallbackFunction contextualPropertyMenu { add { EditorApplication.contextualPropertyMenu += value; } remove { EditorApplication.contextualPropertyMenu -= value; } }

        public static event Func<bool> wantsToQuit { add { EditorApplication.wantsToQuit += value; } remove { EditorApplication.wantsToQuit -= value; } }
        public static event Action quitting { add { EditorApplication.quitting += value; } remove { EditorApplication.quitting -= value; } }

        public static event Action<PlayModeStateChange> playModeStateChanged { add { EditorApplication.playModeStateChanged += value; } remove { EditorApplication.playModeStateChanged -= value; } }
        public static event Action<PauseState> pauseStateChanged { add { EditorApplication.pauseStateChanged += value; } remove { EditorApplication.pauseStateChanged -= value; } }


        public static event EditorApplication.ProjectWindowItemCallback projectWindowItemOnGUI { add { EditorApplication.projectWindowItemOnGUI += value; } remove { EditorApplication.projectWindowItemOnGUI -= value; } }
        public static event EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI { add { EditorApplication.hierarchyWindowItemOnGUI += value; } remove { EditorApplication.hierarchyWindowItemOnGUI -= value; } }
        public static event Action projectChanged { add { EditorApplication.projectChanged += value; } remove { EditorApplication.projectChanged -= value; } }
        public static event Action hierarchyChanged { add { EditorApplication.hierarchyChanged += value; } remove { EditorApplication.hierarchyChanged -= value; } }


        [InitializeOnLoadMethod]
        public static void EditorEnvInit()
        {
            UnityEngine.Debug.Log("FrameworkPath   right?   "+FrameworkPath);
            Framework.InitEnv("IFramework_Editor", envType).InitWithAttribute();

            quitting += Framework.env0.Dispose;
            update += Framework.env0.Update;
            Framework.env0.modules.Coroutine = Framework.env0.modules.CreateModule<CoroutineModule>();

#if UNITY_2018_1_OR_NEWER
            PlayerSettings.allowUnsafeCode = true;
#else
          string  path = Application.dataPath.CombinePath("mcs.rsp");
            string content = "-unsafe";
            if (File.Exists(path) && path.ReadText(System.Text.Encoding.Default) == content) return;
                path.WriteText(content, System.Text.Encoding.Default); 
            AssetDatabase.Refresh();
            EditorUtil.ReOpen2();
#endif
        }
    }

}
