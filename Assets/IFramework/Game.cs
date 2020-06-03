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
using IFramework.Modules;
using IFramework.Modules.Coroutine;
using IFramework.Modules.Message;
using IFramework.UI;
using UnityEngine;
namespace IFramework
{
	public partial class Game
	{
		public class UnityModules
		{
			public UIModule UI;
		}
		public const EnvironmentType envType = EnvironmentType.Ev1;
		public static FrameworkEnvironment env{ get { return Framework.GetEnv(envType); } }
		public static Game instance { get; private set; }
		public static FrameworkModules modules { get { return env.modules; } }
		public static UnityModules unityModules=new UnityModules();
	}
	public partial class Game:MonoBehaviour
	{
		private void Awake()
		{
			instance = this;
			Framework.InitEnv("Game_RT", envType).InitWithAttribute();
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
			modules.Coroutine = modules.CreateModule<CoroutineModule>();
			modules.Loom = modules.CreateModule<LoomModule>();
			modules.Message = modules.CreateModule<MessageModule>();
			unityModules.UI = modules.CreateModule<UIModule>();
		}

		private void InitGame()
		{
			InitFrameworkModules();

			CheckUpdate();
			
			ABAssets.Init();
			
			StartUpLua();
		}

		private void CheckUpdate()
		{
			
		}
		
		private void StartUpLua()
		{
			new XluaMain();
		}
	}
}
