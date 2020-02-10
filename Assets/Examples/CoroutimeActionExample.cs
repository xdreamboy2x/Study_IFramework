/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Modules.Coroutine;
using IFramework.Modules.NodeAction;
using System;

namespace IFramework_Demo
{

    public class CoroutimeActionExample:UnityEngine.MonoBehaviour
	{
        private void Start()
        {
            Framework.Init();
            Framework.modules.Coroutine = Framework.modules.CreateModule<CoroutineModule>();

           this.Sequence()
                .Repeat((r) => {
                    r.Sequence((s) =>
                    {
                        s.TimeSpan(new TimeSpan(0, 0, 5))
                         .Event(() => { Log.L("GG"); })
                         .OnCompelete(() => { Log.L(1231); });
                    }, false)
                    ;
                },2)
                .TimeSpan(new TimeSpan(0, 0, 5))
                .OnCompelete((ss) => { /*ss.Reset();*/ })
                .OnDispose((ss) => { Log.L("dispose"); })
                .OnRecyle(() => { Log.L(123132); })
                .Run(Framework.modules.Coroutine as CoroutineModule);
        }
        private void Update()
        {
            Framework.Update();
        }
        private void OnDisable()
        {
            Framework.Dispose();
        }
    }
}
