/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace IFramework.Hotfix.AB
{
    public class ABTool
	{
        //public const string configPath = "Assets/Manifest.Xml";
        public const string configPath = "Assets/IFramework/Hotfix/Manifest_AssetBundle.Xml";

        public const string assetsDir = "AssetBundles";
        public const string helpABName = "help";
        public static string assetsOutPutPath = assetsDir.CombinePath(platformName);
        public static string platformName
        {
            get
            {
#if UNITY_EDITOR
                return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
            }
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            if (platform == RuntimePlatform.Android) return "Android";
            if (platform == RuntimePlatform.IPhonePlayer) return "iOS";
            if (platform == RuntimePlatform.tvOS) return "tvOS";
            if (platform == RuntimePlatform.WebGLPlayer) return "WebGL";
            if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor) return "Windows";
            if (platform == RuntimePlatform.OSXPlayer || platform == RuntimePlatform.OSXEditor) return "OSX";
            return null;
        }
#if UNITY_EDITOR
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            if (target == BuildTarget.Android) return "Android";
            if (target == BuildTarget.tvOS) return "tvOS";
            if (target == BuildTarget.iOS) return "iOS";
            if (target == BuildTarget.WebGL) return "WebGL";
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64) return "Windows";
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target ==  BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
                return "OSX";
            return null;
        }
        public static bool testmode
        {
            get
            {
                return EditorPrefs.GetBool("ActiveBundleMode", true);
            }
            set
            {
                EditorPrefs.SetBool("ActiveBundleMode", value);
            }
        }
#endif
    }
}
