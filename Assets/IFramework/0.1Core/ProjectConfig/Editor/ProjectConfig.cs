/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEditor;
using System.Collections.Generic;
using System.Linq;
namespace IFramework
{
    public partial class ProjectConfig
    {
        public static string NameSpace { get { return info.NameSpace; } }
        public static string UserName { get { return info.UserName; } }
        public static string Version { get { return info.Version; } }
        public static string Description { get { return info.Description; } }
        private static ProjectConfigInfo __info;
        private static ProjectConfigInfo info
        {
            get
            {
                if (__info == null) LoadProjectInfo();
                return __info;
            }
        }
        public static string ProjectConfigInfoPath = EditorEnv.corePath.CombinePath("ProjectConfig/Editor/ProjectConfig.asset").ToRegularPath();
        private static void LoadProjectInfo()
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}",typeof(ProjectConfigInfo)), new string[] { @"Assets" });
            List<ProjectConfigInfo> stos = guids.ToList()
                .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<ProjectConfigInfo>(AssetDatabase.GUIDToAssetPath(guid)); });
            if (stos.Count == 0 || !AssetDatabase.GetAssetPath(stos[0]).Equals(ProjectConfigInfoPath)) CreateNewSto(stos);
            else
            {
                for (int i = 1; i < stos.Count; i++)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(stos[i]));
                __info = stos[0];
            }
        }
        private static void CreateNewSto(List<ProjectConfigInfo> stos)
        {
            if (stos.Count > 0)
                stos.ReverseForEach((sto) => { AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sto)); });
            __info = ScriptableObj.Create<ProjectConfigInfo>(ProjectConfigInfoPath);
        }
    }
}
