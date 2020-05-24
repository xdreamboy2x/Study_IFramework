/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFramework;
using IFramework.UI;

namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]
	public class UIExample : MonoBehaviour
	{
        UIModule module;
        private void Start()
        {
            module = Framework.env1.modules.CreateModule<UIModule>();
            module.AddLoader(Load);
            //module.SetGroups(new Groups(UIMap_MVVM.map));
        }

        private UIPanel Load(Type type, string path, string name, UIPanelLayer layer)
        {
            GameObject go = Resources.Load<GameObject>(path);
            return go.GetComponent<UIPanel>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                module.Get<Panel01>("Panel01", "Panel01");
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                module.Get<Panel02>("Panel02",  "Panel02");
            }
        }
        
    }
}
