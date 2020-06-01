/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.319
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using IFramework.AB;
using IFramework.Lua;
using UnityEngine;
namespace IFramework
{
	public partial class Game
	{
		public const EnvironmentType envType = EnvironmentType.Ev1;
		public static FrameworkEnvironment env{ get { return Framework.GetEnv(envType); } }
		public static Game instance { get; private set; }
		public static FrameworkModules modules { get { return env.modules; } }
	}
	public partial class Game:MonoBehaviour
	{
		private void Awake()
		{
			instance = this;
			Framework.InitEnv("Game_RT", envType).InitWithAttribute();
			InitFrameworkModules();
			InitGame();
		}
		private void Update()
		{
			Framework.env1.Update();
		}
		private void OnDisable()
		{

			Framework.env1.Dispose();
		}
		
	}

	public partial class Game
	{
		private void InitFrameworkModules()
		{
			
		}

		private void InitGame()
		{
			InitHotFix();
		}
		public  static string genPath
		{
			get { return Application.dataPath.CombinePath("IFramework/HotFix/Scripts").ToRegularPath(); }
		}
		private void InitHotFix()
		{
			ABAssets.Init();
			var _asset=	 ABAssets.Load<TextAsset>(Application.dataPath.CombinePath("IFramework/HotFix/Scripts/Main.Lua")
				.ToRegularPath().ToAssetsPath());
			XluaMain _main=new XluaMain(_asset.asset as TextAsset);
		}
	}
}
