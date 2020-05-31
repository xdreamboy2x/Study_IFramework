/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.319
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using UnityEngine;
namespace IFramework
{
	public class Game:MonoBehaviour
	{
		public const EnvironmentType envType = EnvironmentType.Ev1;
		public static FrameworkEnvironment env{ get { return Framework.GetEnv(envType); } }
		public static Game instance { get; private set; }
		public static FrameworkModules modules { get { return env.modules; } }
		
		private void Awake()
		{
			instance = this;
			Framework.InitEnv("Game_RT", envType).InitWithAttribute();
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
}
