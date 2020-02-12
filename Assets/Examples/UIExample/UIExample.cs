/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using UnityEngine;
namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]
    internal class UIExample: MonoBehaviour
    {
        UIModule mou;
        private void Start()
        {
            
            mou = Framework.env1.modules.CreateModule<UIModule>();
            mou.AddLoader((type, path,pt,name,arg) =>
            {
                GameObject go = Resources.Load<GameObject>(path);
                return go.GetComponent<UIPanel>();
            });
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                mou.Get(typeof(Panel1), "Canvas", UIPanelLayer.Background, "Panel1", new UIEventArgs(), false);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                mou.Get(typeof(Panel2), "Canvas1", UIPanelLayer.Guide, "Panel2", new UIEventArgs(), true);
            }
        }
       
    }
}
