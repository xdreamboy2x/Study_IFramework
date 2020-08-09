/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using IFramework.Utility;

namespace IFramework
{
    class EditorUtil
	{
        [MenuItem("IFramework/Tool/Copy Path")]
        public static void CopyAssetPath()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            GUIUtility.systemCopyBuffer = path;
        }
        [MenuItem("IFramework/Editor/Quit")]
        public static void Quit()
        {
          //  Environment.Exit(0);
            EditorApplication.Exit(0);
        }
        [MenuItem("IFramework/Editor/ReOpen")]
        public static void ReOpen()
        {
            EditorApplication.OpenProject(Application.dataPath.CombinePath("../"));
        }

        public static EditorBuildSettingsScene[] ScenesInBuildSetting()
        {
            return EditorBuildSettings.scenes;
        }
        public static string[] GetScenesInBuildSetting()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            if (target == BuildTarget.Android)
            {
                return  name + PlayerSettings.Android.bundleVersionCode + ".apk";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return  name + PlayerSettings.Android.bundleVersionCode + ".exe";
            }
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
            {
                return  name + ".Game";
            }
            if (target == BuildTarget.iOS)
            {
                return "iOS";
            }
            return null;
            //if (target == BuildTarget.WebGL)
            //{
            //    return "/web";
            //}

        }
        public static GameObject CreatePrefab(GameObject source, string savePath)
        {
            GameObject goClone =GameObject.Instantiate(source);
            GameObject prefab;
#if UNITY_2018_1_OR_NEWER
            prefab= PrefabUtility.SaveAsPrefabAssetAndConnect(goClone, savePath, InteractionMode.AutomatedAction);
          
#else
            prefab = PrefabUtility.CreatePrefab(savePath, goClone);
            prefab = PrefabUtility.ReplacePrefab(goClone, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
            AssetDatabase.ImportAsset(savePath);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            GameObject.DestroyImmediate(goClone);
            return prefab;
        }
    }

}
