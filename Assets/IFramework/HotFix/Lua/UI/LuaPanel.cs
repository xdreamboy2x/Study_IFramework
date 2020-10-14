/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.114
 *UnityVersion:   2018.4.24f1
 *Date:           2020-07-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.Hotfix.Lua
{
    [System.Serializable]
    public class PanelField
    {
        public string Key = "";
        public GameObject value;
    }
	public class LuaPanel:UIPanel
	{
        public List<PanelField> fields = new List<PanelField>();
    }
}
