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
using IFramework.NodeAction;
using System;
using UnityEngine;

namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]

    public class CoroutimeActionExample: MonoBehaviour
    {
        private void Start()
        {
            APP.env.modules.Coroutine = APP.env.modules.CreateModule<CoroutineModule>();

           this.Sequence( APP.envType)
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
                .Run();
        }
       
    }
}
