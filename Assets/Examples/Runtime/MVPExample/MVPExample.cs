/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-17
 *Description:    Description
 *History:        2020-02-17--
*********************************************************************************/
using IFramework;
using IFramework.Modules.MVP;
using IFramework.Modules.ECS;
using UnityEngine;
using UnityEngine.UI;
namespace IFramework_Demo
{
    public class MVPExample : MonoBehaviour
    {
        private MVPModule mod;
        private struct DataComponent : IComponent{
            public float sliderVal;
        }
        private class SliderValueChangeArg : IEventArgs{
            public float value;
        }
        private class FreshViewArg : IEventArgs { }

        private class MyMVPEnity : MVPEnity   
        {
            public Slider slider;
            public Text txt; 
            public void FreshText(float value)
            {
                txt.text = value.ToString();
            }
        }
        private class MySensorSystem : SensorSystem
        {
            public MySensorSystem() : base() { }

            protected override void OnSetEnity(MVPEnity value)
            {
                (value as MyMVPEnity).slider.onValueChanged.AddListener((val) =>
                {
                    SendSensor(0, new SliderValueChangeArg() { value = val });
                });
            }
        }
        private class MyPolicySystem : PolicySystem
        {
            public MyPolicySystem() : base() { }

            protected override void OnSensor(int code, IEventArgs args, object[] param)
            {
                SliderValueChangeArg change = args as SliderValueChangeArg;
                if (change.value < 0.5f) return;
                SendPolicy(0, args);
            }
        }
        private class MyPolicyExecutorSystem : PolicyExecutorSystem
        {
            public MyPolicyExecutorSystem() : base() { }

            protected override void OnPolicy(int code, IEventArgs args, object[] param)
            {
                SliderValueChangeArg change = args as SliderValueChangeArg;
                var data = enity.GetComponent<DataComponent>();
                data.sliderVal = change.value;
                enity.ReFreshComponent<DataComponent>(data);
                SendPolicyExecutor(0, new FreshViewArg());
            }
        }
        private class MyViewSystem : ViewSystem
        {
            public MyViewSystem() : base() { }

            protected override void OnPolicyPolicyExecutor(int code, IEventArgs args, object[] param)
            {
                var data = enity.GetComponent<DataComponent>();
                (enity as MyMVPEnity).FreshText(data.sliderVal);
            }
        }

        public Slider slider;
        public Text txt;
        private void Awake()
        {
            mod = MVPModule.CreatInstance<MVPModule>("Test");
            mod.enity = new MyMVPEnity() { slider = slider, txt = txt };
            mod.policy = new MyPolicySystem();
            mod.policyExecutor = new MyPolicyExecutorSystem();
            mod.sensor = new MySensorSystem();
            mod.view = new MyViewSystem();
            mod.enity.AddComponent<DataComponent>();

        }
        private void Update()
        {
            mod.Update();
        }
        private void OnDisable()
        {
            mod.Dispose();
        }
    }
}
