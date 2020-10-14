/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-11
 *Description:    Description
 *History:        2019-12-11--
*********************************************************************************/
using IFramework;
using IFramework.Injection;
using UnityEngine;

namespace IFramework_Demo
{

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
            Framework.env0.container.RegisterInstance<INNN>(new NNN());
            Framework.env0.container.Inject(this); 
            nnn.ToDo();
        }

    }
}
