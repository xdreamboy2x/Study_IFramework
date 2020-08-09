/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.114
 *UnityVersion:   2018.4.24f1
 *Date:           2020-08-11
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework
{
	public static class GameObjectExtension
	{
        public static GameObject ToggleActive(this GameObject obj)
        {
            obj.SetActive(!obj.activeInHierarchy);
            return obj;
        }
        public static GameObject RemoveComponent(this GameObject obj,Type component)
        {
            var com= obj.GetComponent(component);
            UnityEngine.Object.Destroy(com);
            return obj;
        }
        public static GameObject RemoveComponent<T>(this GameObject obj)where T:Component
        {
            var com = obj.GetComponent<T>();
            UnityEngine.Object.Destroy(com);
            return obj;
        }
    }
}
