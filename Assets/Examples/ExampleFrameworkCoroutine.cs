/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-11
 *Description:    Description
 *History:        2019-12-11--
*********************************************************************************/
using System.Collections;
using IFramework;
using IFramework.Moudles.Coroutine;

namespace IFramework_Demo
{

    public class ExampleFrameworkCoroutine : UnityEngine.MonoBehaviour
    {
        void Start()
        {
            Framework.Init();
            Framework.moudles.Coroutine = CoroutineMoudle.CreatInstance<CoroutineMoudle>();
            Framework.moudles.Coroutine.StartCoroutine(wait2());
        }
        IEnumerator wait()
        {
            //Log.L(Framework.DeltaTime);
            //Log.L(Framework.TimeSinceInit);
            yield return new WaitForSeconds(2);

        }
        IEnumerator wait1()
        {
            Log.L("wait1 Go");
            yield return wait();
            Log.L("wait1 end");

        }
        IEnumerator wait2()
        {
            Log.L("wait2 Go");
            yield return wait1();
            Log.L("wait2 end");
            Log.L("wait2 Go");
            yield return wait();
            Log.L("wait2 end");
        }
        // Update is called once per frame
        void Update()
        {


            Framework.Update();
        }
        private void OnDestroy()
        {

            Framework.Dispose();
        }
    }
}
