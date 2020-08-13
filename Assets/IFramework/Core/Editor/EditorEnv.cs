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
using UnityEditor.Compilation;
using System.IO;

namespace IFramework
{
    class EditorEnv
    {
        private const string _relativeCorePath = "Core";
        private static string _fpath;


        public const EnvironmentType envType = EnvironmentType.Ev0;
        public static FrameworkEnvironment env { get { return Framework.env0; } }
        public static FrameworkModules moudules { get { return env.modules; } }



        public static string frameworkName { get { return Framework.FrameworkName; } }
        public static string author { get { return Framework.Author; } }
        public static string version { get { return Framework.Version; } }
        public static string description { get { return Framework.Description; } }
        public static string frameworkPath
        {
            get
            {
                if (string.IsNullOrEmpty(_fpath))
                {
                    string[] assetPaths = AssetDatabase.GetAllAssetPaths();
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        if (assetPaths[i].Contains(_relativeCorePath))
                        {
                            string tempPath = assetPaths[i];
                            int index = tempPath.IndexOf(_relativeCorePath);
                            _fpath = tempPath.Substring(0, index);
                            break;
                        }
                    }
                }
                return _fpath;
            }
        }
        public static string corePath { get { return frameworkPath.CombinePath(_relativeCorePath); } }
        public static string memoryPath
        {
            get
            {
                string path = "Assets/../" + frameworkName+"EditorMemory";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string formatScriptsPath
        {
            get
            {
                string path = Path.Combine(memoryPath,"Scripts");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }



        public static event EditorApplication.CallbackFunction update { add { EditorApplication.update += value; } remove { EditorApplication.update -= value; } }
        public static event EditorApplication.CallbackFunction delayCall { add { EditorApplication.delayCall += value; } remove { EditorApplication.delayCall -= value; } }
        public static event EditorApplication.HierarchyWindowItemCallback hierarchyWindowItemOnGUI { add { EditorApplication.hierarchyWindowItemOnGUI += value; } remove { EditorApplication.hierarchyWindowItemOnGUI -= value; } }
        public static event Action<string> assemblyCompilationStarted { add { CompilationPipeline.assemblyCompilationStarted += value; } remove { CompilationPipeline.assemblyCompilationStarted -= value; } }

        [InitializeOnLoadMethod]
        static void EditorEnvInit()
        {
            UnityEngine.Debug.Log("FrameworkPath   right?   " + frameworkPath);
            Framework.InitEnv("IFramework_Editor", envType).InitWithAttribute();
            assemblyCompilationStarted += (str) => {
                Framework.env0.Dispose();
                UnityEngine.Debug.Log("EditorEnv Dispose"); 
            };
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
