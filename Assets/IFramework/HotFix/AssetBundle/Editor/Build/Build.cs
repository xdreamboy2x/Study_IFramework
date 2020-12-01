/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using IFramework.Utility;
using System.Text;

namespace IFramework.Hotfix.AB
{
    class Build
    {
        public static void BuildManifest(string path, List<AssetBundleBuild> builds)
        {
            if (File.Exists(path)) File.Delete(path);
            List<BundleGroup> contents = new List<BundleGroup>();
            foreach (var item in builds)
            {
                contents.Add(new BundleGroup(item.assetBundleName, item.assetNames));
            }
            string txt = Xml.ToXmlString<List<BundleGroup>>(contents);
            File.WriteAllText(path, txt);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        private static long GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (File.Exists(sFullName))
                lSize = new FileInfo(sFullName).Length;
            return lSize;
        }

        private static string BytesToString(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X4"));
            }
            return sb.ToString();
        }
        public static void BuildAssetBundles(List<AssetBundleBuild> builds, BuildTarget buildTarget)
        {
            if (!Directory.Exists(ABTool.assetsOutPutPath))
                Directory.CreateDirectory(ABTool.assetsOutPutPath);
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            if (builds == null || builds.Count == 0)
              BuildPipeline.BuildAssetBundles(ABTool.assetsOutPutPath, options, buildTarget);
            else
              BuildPipeline.BuildAssetBundles(ABTool.assetsOutPutPath, builds.ToArray(), options, buildTarget);
            var list = IO.GetSubFiles(ABTool.assetsOutPutPath, true);
            List<VersionConfig.BundleVersion> _versions = new List<VersionConfig.BundleVersion>();
            for (int i = 0; i < list.Count; i++)
            {
                var _path = list[i].ToRegularPath();
                EditorUtility.DisplayProgressBar("Build Version", _path, (float)i / list.Count);
                _versions.Add(new VersionConfig.BundleVersion()
                {
                    path = _path,
                    size = GetFileSize(_path),
                    crc32 = BytesToString(Verifier.GetCrc32(_path)),
                    bundle = _path.Replace(ABTool.assetsOutPutPath.ToAbsPath(), "").Remove(0,1)
                    
                });
            }
            EditorUtility.ClearProgressBar();
            string path = EditorEnv.frameworkPath.CombinePath(string.Format("Hotfix/Version_{0}.asset",ABTool.platformName));
            if (!File.Exists(path)) EditorTools.ScriptableObjectTool.Create<VersionConfig>(path);
            var versions = EditorTools.ScriptableObjectTool.Load<VersionConfig>(path);

            versions.SetVersion(_versions);
            EditorTools.ScriptableObjectTool.Update(versions);

        }



        public static string GetAssetBundleManifestFilePath()
        {
            var path = Path.Combine(ABTool.assetsOutPutPath, ABTool.platformName);
            return Path.Combine(path, ABTool.platformName) + ".manifest";
        }


        public static void CopyBundleFilesTo(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var destination = Path.Combine(path, ABTool.assetsDir);
            if (Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(path);
            FileUtil.CopyFileOrDirectory(ABTool.assetsDir, destination);
            AssetDatabase.Refresh();
        }
        public static void DeleteBundleFiles()
        {
            if (Directory.Exists(ABTool.assetsOutPutPath))
            {
                Directory.Delete(ABTool.assetsOutPutPath, true);
            }
        }
    }
}
