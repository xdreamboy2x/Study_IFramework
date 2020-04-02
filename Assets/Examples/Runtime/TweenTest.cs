/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-04
 *Description:    Description
 *History:        2020-04-04--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFramework;
using IFramework.Modules.Coroutine;
using IFramework.Tweens;
using IFramework.Modules.NodeAction;
//using DG.Tweening;

namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]
	public class TweenTest : MonoBehaviour
	{
        FloatTweenValue tv;
        public Transform cube;
        Tween<Vector3> tc;
        private void Start()
        { 
          Framework.env1.modules.Coroutine = Framework.env1.modules.CreateModule<CoroutineModule>();
          tc=  cube.DoMove(cube.transform.position+Vector3.one*5, 2, EnvironmentType.Ev1)
                .SetLoop(-1, LoopType.PingPong)
                .SetCurve(ValueCurve.scurve)
                .SetRecyle(false);
            cube.DoScale(Vector3.one * 2, 0.2f)
                .SetLoop(-1, LoopType.PingPong)
                .SetRecyle(false);
            cube.DoRota(Vector3.one * 2, 0.5f)
                .SetLoop(-1, LoopType.PingPong)
                .SetRecyle(false);
            cube.GetComponent<Renderer>().material.DoColor(Color.cyan, 0.6f)
                 .SetLoop(-1, LoopType.PingPong)
                 .SetRecyle(false);
        
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tc.Rewind(1);
            }
            if (Input.GetKey( KeyCode.A))
            {
                tc.ReStart();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                tc.Complete(false);
            }
        }

    }
}
