/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEditor;
using System.IO;
using System;
using System.Text;
using UnityEngine;

namespace IFramework
{
    class  FormatIFrameworkScript
    {
        const string key = "FormatIFrameworkScript";

        private class FormatIFrameWorkScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            public static void OnWillCreateAsset(string metaPath)
            {
                if (!EditorPrefs.GetBool(key, false)) return;                   
                string filePath = metaPath.Replace(".meta", "");
                if (!filePath.EndsWith(".cs")) return;
                string realPath = filePath.ToAbsPath();
                string txt = File.ReadAllText(realPath);
                if (!txt.Contains("#FAuthor#")) return;
                txt = txt.Replace("#FAuthor#", EditorEnv.author)
                         .Replace("#FNameSpace#", EditorEnv.frameworkName)
                         .Replace("#FDescription#", EditorEnv.description)
                         .Replace("#FSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                         .Replace("#FVERSION#", EditorEnv.version)
                         .Replace("#FUNITYVERSION#", Application.unityVersion)
                         .Replace("#FDATE#", DateTime.Now.ToString("yyyy-MM-dd"));
                File.WriteAllText(realPath, txt, Encoding.UTF8);
                EditorPrefs.SetBool(key, false);
            }
        }
        private static string newScriptName = "newScript.cs";
        private static string originScriptPath = EditorEnv.formatScriptsPath.CombinePath("AuthorCharpScript.txt");

        [MenuItem("Assets/Create/IFramework/AuthorScript",priority =-1000)]
        public static void Create()
        {
            CreateOriginIfNull();
            CopyAsset.Copy(newScriptName, originScriptPath);
            EditorPrefs.SetBool(key, true);
        }

        private static void CreateOriginIfNull()
        {
            if (File.Exists(originScriptPath)) return;
            using (FileStream fs = new FileStream(originScriptPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Lock(0, fs.Length);
                    sw.WriteLine("/*********************************************************************************");
                    sw.WriteLine(" *Author:         #FAuthor#");
                    sw.WriteLine(" *Version:        #FVERSION#");
                    sw.WriteLine(" *UnityVersion:   #FUNITYVERSION#");
                    sw.WriteLine(" *Date:           #FDATE#");
                    sw.WriteLine(" *Description:    #FDescription#");
                    sw.WriteLine(" *History:        2018.11--");
                    sw.WriteLine("*********************************************************************************/");
                    sw.WriteLine("namespace #FNameSpace#");
                    sw.WriteLine("{");
                    sw.WriteLine("\tpublic class #FSCRIPTNAME#");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t");
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    fs.Unlock(0, fs.Length);
                    sw.Flush();
                    fs.Flush();
                }
            }
            AssetDatabase.Refresh();
        }
    }

}