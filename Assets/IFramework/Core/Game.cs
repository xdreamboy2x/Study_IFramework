/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
	public abstract class Game:MonoBehaviour
	{
        public EnvironmentType envType { get { return Launcher.envType; } }
        public FrameworkEnvironment env { get { return Launcher.env; } }
        public FrameworkModules modules { get { return Launcher.modules; } }



        public abstract void CreateModules();
        public abstract void Startup();
	}
}
