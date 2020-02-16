/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-11
 *Description:    Description
 *History:        2019-12-11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;
using UnityEngine;

namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]

    public class InjectExample:UnityEngine.MonoBehaviour
	{
	    public interface INNN
        {
            void ToDo();
        }
        public class NNN : INNN
        {
            public void ToDo()
            {
                Log.L("13213213");
            }
        }
        [Inject]
        static INNN nnn=null;
        private void Awake()
        {
            Framework.env1.container.RegisterInstance<INNN>(new NNN());
            Framework.env1.container.Inject(this); 
            nnn.ToDo();
        }

    }
}
