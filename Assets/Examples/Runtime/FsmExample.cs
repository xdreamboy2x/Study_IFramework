/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Modules;
using IFramework.Modules.Fsm;
using UnityEngine;
namespace IFramework_Demo
{
    public class State : IState
    {
        public void OnEnter()
        {
            Log.L(GetType() + "  Enter");
        }
        public void OnExit()
        {
            Log.L(GetType() + "  OnExit");

        }
        public void Update()
        {
            Log.L(GetType() + "  Update");
        }
    }
    public class State1 : State
    {
    }
    public class State2 : State
    {
    }
    public class FsmExample : MonoBehaviour
    {

        FsmModule fsm;
        private void Start()

        {
            fsm = FrameworkModule.CreatInstance<FsmModule>("","");
            State1 s1 = new State1();
            State2 s2 = new State2();
            fsm.SubscribeState(s1);
            fsm.enterState = s1;
            fsm.SubscribeState(s2);
            // fsm.exitState = s2;
            var val = fsm.CreateConditionValue<bool>("bool", true);

            var t1 = fsm.CreateTransition(s1, s2);
            var t2 = fsm.CreateTransition(s2, s1);

            t1.BindCondition(fsm.CreateCondition<bool>("bool", false, CompareType.Equals));
            t2.BindCondition(fsm.CreateCondition<bool>(val, true, CompareType.Equals));

            fsm.Start();
        }
        private void Update()
        {
            fsm.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                fsm.SetBool("bool", false);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                fsm.SetBool("bool", true);
            }
        }

    }
}
