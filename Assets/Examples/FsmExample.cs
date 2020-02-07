/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.Modules.Fsm;
using UnityEngine;
namespace IFramework_Demo
{
    public class State:IFsmState
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
    public class FsmExample : MonoBehaviour { 


        private void Awake()
        
        {
            Framework.Init();
            Framework.modules.Fsm = Framework.modules.CreateModule<FsmModule>();
            State1 s1 = new State1();
            State2 s2 = new State2();
            Framework.modules.Fsm.SubscribeState(s1);
            Framework.modules.Fsm.EnterState = s1;
            Framework.modules.Fsm.SubscribeState(s2); 
            //f.ExitState = s2;
         var val= Framework.modules.Fsm.CreateConditionValue<bool>("bool", true);

            var t1= Framework.modules.Fsm.CreateTransition(s1, s2);
            var t2 = Framework.modules.Fsm.CreateTransition(s2, s1);

            t1.BindCondition(Framework.modules.Fsm.CreateCondition<bool>("bool", false, ConditionCompareType.EqualsWithCompare));
            t2.BindCondition(Framework.modules.Fsm.CreateCondition<bool>(val, true, ConditionCompareType.EqualsWithCompare));

            Framework.modules.Fsm.Start();
        }
        private void Update()
        {
            Framework.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Framework.modules.Fsm.SetBool("bool", false);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Framework.modules.Fsm.SetBool("bool", true);
            }
        }
        private void OnDisable()
        {
            Framework.Dispose();

        }
    }
}
