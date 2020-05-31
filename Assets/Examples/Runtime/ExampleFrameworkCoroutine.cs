/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-11
 *Description:    Description
 *History:        2019-12-11--
*********************************************************************************/
using System.Collections;
using IFramework;
using IFramework.Modules.Coroutine;
using UnityEngine;

namespace IFramework_Demo
{
    [RequireComponent(typeof(Game))]

    public class ExampleFrameworkCoroutine : UnityEngine.MonoBehaviour
    {
        void Start()
        {
            Game.env.modules.Coroutine = Game.env.modules.CreateModule<CoroutineModule>();
            Game.env.modules.Coroutine.StartCoroutine(wait2());
        }
        IEnumerator wait()
        {
            yield return new IFramework.Modules.Coroutine.WaitForSeconds(2);
        }
        IEnumerator wait1()
        {
            Log.L("wait1 Go");
            yield return wait();
            Log.L("wait1 end");

        }
        IEnumerator wait2()
        {
            Log.L("wait2 Go");
            yield return wait1();
            Log.L("wait2 end");
            Log.L("wait2 Go");
            yield return wait();
            Log.L("wait2 end");
        }
      
    }
}
