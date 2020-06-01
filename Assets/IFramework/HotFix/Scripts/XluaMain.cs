/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.319
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-04
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEngine;
using System;
using XLua;

namespace IFramework.Lua
{
	public class XluaMain
	{
		private LuaTable scriptEnv;
		public XluaMain(TextAsset main)
		{
			XLuaEnv.AddLoader(new AssetBundleLoader());
			XLuaEnv.onDispose += LuaDispose;
			
			scriptEnv = XLuaEnv.GetTable(main, "Main");
			var  awake= scriptEnv.Get<Action>("Awake");
			awake.Invoke();
			awake = null;
		}

		private void LuaDispose()
		{
			var dis = scriptEnv.Get<Action>("OnDispose");
			dis.Invoke();
			dis = null;
		}
	}
}
