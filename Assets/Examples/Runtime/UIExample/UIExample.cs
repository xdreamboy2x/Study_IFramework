/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework_Demo
{

    [RequireComponent(typeof(APP))]
    internal class UIExample: MonoBehaviour
    {
        UIModule mou;
        private static Dictionary<Type, Tuple<Type, Type, Type, Type, Type>> map =
           new Dictionary<Type, Tuple<Type, Type, Type, Type, Type>>()
           {
                { typeof(Panel1),Tuple.Create(typeof(UIEnity),typeof(p1Sensor),typeof(p1Policy),typeof(p1Excutor),typeof(p1View))},
                { typeof(Panel2),Tuple.Create(typeof(UIEnity),typeof(p2Sensor),typeof(p2Policy),typeof(p2Excutor),typeof(p2View))},
           };
        private void Start()
        {
            
            mou = Framework.env1.modules.CreateModule<UIModule>();
            mou.SetMap(map);
            mou.AddLoader((type, path,pt,name) =>
            {
                GameObject go = Resources.Load<GameObject>(path);
                return go.GetComponent<UIPanel>();
            });
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                mou.Get(typeof(Panel1), "Panel1", new UIEventArgs(), "Canvas", UIPanelLayer.Background);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                mou.Get(typeof(Panel2), "Panel2", new UIEventArgs(), "Canvas1", UIPanelLayer.Guide);
            }
        }
       
    }
}
