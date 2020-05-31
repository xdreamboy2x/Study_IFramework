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
    [RequireComponent(typeof(Game))]
	public class ReouourcesTest:MonoBehaviour
	{
        private void Start()
        {
            Game.env.modules.Resources = Game.env.modules.CreateModule<ResourceModule>();
            var res= Game.env.modules.Resources.Load<TextAsset,ResourcesLoader<TextAsset>>("RS","txt","txt", null, null);
            Game.env.modules.Resources.trick = 10;
            Log.L(res.value.text);
            var res1 = Game.env.modules.Resources.Load<TextAsset, ResourcesLoader<TextAsset>>("RS", "txt", "txt", null, null);

            res.Release();
            res.Release();

        }
    }
}
