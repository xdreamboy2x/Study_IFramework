/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.259
 *UnityVersion:   2018.4.17f1
 *Date:           2020-05-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules.Resources;
using UnityEngine;
using IFramework;
namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]
	public class ReouourcesTest:MonoBehaviour
	{
        private void Start()
        {
            APP.env.modules.Resources = APP.env.modules.CreateModule<ResourceModule>();
            var res= APP.env.modules.Resources.Load<TextAsset,ResourcesLoader<TextAsset>>("RS","txt","txt", null, null);
            APP.env.modules.Resources.trick = 10;
            Log.L(res.value.text);
            var res1 = APP.env.modules.Resources.Load<TextAsset, ResourcesLoader<TextAsset>>("RS", "txt", "txt", null, null);

            res.Release();
            res.Release();

        }
    }
}
