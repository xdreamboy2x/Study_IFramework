/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace IFramework.Hotfix.AB
{
    abstract class Collecter
    {
        public string searchPath;
        public string bundleName;
        public Collecter() { }
        public Collecter(string path)
        {
            searchPath = path;
        }
        public abstract void Collect();
        //public abstract string GetAssetBundleName(string assetPath);
        public List<string> MeetFiles = new List<string>();
    }

    class ABNameCollecter : Collecter
    {
        public ABNameCollecter() { }
        public ABNameCollecter(string path,string assetBundleName) : base(path)
        {
            bundleName = assetBundleName;
        }
        //public override string GetAssetBundleName(string assetPath)
        //{
        //    return bundleName;
        //}
        public override void Collect()
        {
            var files = AB.Collect.GetNonCollect(this);
            List<string> list = new List<string>();
            foreach (var item in files)
            {
                list.AddRange(AB.Collect.GetDependenciesWithoutShared(item));
            }
            files.AddRange(list);
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = files.ToArray();
            AB.Collect.SetBuild(build, files);
        }
    }
    class ScenesCollecter : Collecter
    {
        public ScenesCollecter() { }
        public ScenesCollecter(string path) : base(path) { }

        //public override string GetAssetBundleName(string assetPath)
        //{
        //    return AB.Collect.GetAbsPathWithoutExtension(assetPath);
        //}

        public override void Collect()
        {
            var files = AB.Collect.GetNonCollect(this);
            for (int i = 0; i < files.Count; i++)
            {
                var item = files[i];
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = AB.Collect.GetAbsPathWithoutExtension(item);
                build.assetNames = new string[] { item };
                AB.Collect.SetBuild(build, build.assetNames);

            }
        }
    }
    class DirNameCollecter : Collecter
    {
        public DirNameCollecter() { }
        public DirNameCollecter(string path) : base(path) { }
        //public override string GetAssetBundleName(string assetPath)
        //{
        //    return AB.Collect.GetAbsPathWithoutExtension(Path.GetDirectoryName(assetPath));
        //}
        public override void Collect()
        {
            List<string> files = AB.Collect.GetNonCollect(this);
            Dictionary<string, List<string>> bundles = new Dictionary<string, List<string>>();
            for (int i = 0; i < files.Count; i++)
            {
                var filePath = files[i];
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Collecting... [{0}/{1}]", i, files.Count), filePath, i * 1f / files.Count)) break;
                var path = Path.GetDirectoryName(filePath);
                if (!bundles.ContainsKey(path))
                {
                    bundles[path] = new List<string>();
                }
                bundles[path].Add(filePath);
                bundles[path].AddRange(AB.Collect.GetDependenciesWithoutShared(filePath));
            }

            int count = 0;
            foreach (var item in bundles)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = AB.Collect.GetAbsPathWithoutExtension(item.Key) + "_" + item.Value.Count;
                build.assetNames = item.Value.ToArray();
                AB.Collect.SetBuild(build, build.assetNames);
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", count, bundles.Count), build.assetBundleName, count * 1f / bundles.Count)) break;
                count++;
            }
        }
    }
    class FileNameCollecter : Collecter
    {
        public FileNameCollecter() { }
        public FileNameCollecter(string path) : base(path) { }

        //public override string GetAssetBundleName(string assetPath)
        //{
        //    return AB.Collect.GetAbsPathWithoutExtension(assetPath);
        //}
        public override void Collect()
        {
            var files = AB.Collect.GetNonCollect(this);
            for (int i = 0; i < files.Count; i++)
            {
                string filePath = files[i];
                if (EditorUtility.DisplayCancelableProgressBar(string.Format("Packing... [{0}/{1}]", i, files.Count), filePath, i * 1f / files.Count)) break;
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = AB.Collect.GetAbsPathWithoutExtension(filePath);
                List<string> assetNames = AB.Collect.GetDependenciesWithoutShared(filePath);
                assetNames.Add(filePath);
                build.assetNames = assetNames.ToArray();
                AB.Collect.SetBuild(build, assetNames);
            }
        }
    }
}
