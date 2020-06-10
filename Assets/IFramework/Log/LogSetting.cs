/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-17
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
    public class LogSetting : ScriptableObject
    {
        public List<LogEliminateItem> Infos = new List<LogEliminateItem>();
        public int lev_L;
        public int lev_W;
        public int lev_E;
        public bool enable = true;
        public bool enable_L = true;
        public bool enable_W = true;
        public bool enable_E = true;
    }
    [System.Serializable]
    public class LogEliminateItem
    {
        public string name;
        public string path;
        public TextAsset text;
    }
}
