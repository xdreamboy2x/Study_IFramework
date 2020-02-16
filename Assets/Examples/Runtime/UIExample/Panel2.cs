/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;
using IFramework;

namespace IFramework_Demo
{
    public class p2Sensor : pExpSensor { }
    public class pExpSensor : UISensor
    {
        protected override void OnClear()
        {
            Debug.Log("Clear  " + GetType());
            (this.enity as UIEnity).Destory(); 
        }
        protected override void OnLoad()
        {
            Debug.Log("OnLoad  " + GetType());
            (this.enity as UIEnity).panel.gameObject.SetActive(true);
        }
        protected override void OnPop(UIEventArgs arg)
        {
            Debug.Log("OnPop  " + GetType());
            (this.enity as UIEnity).panel.gameObject.SetActive(false);

        }
        protected override void OnPress(UIEventArgs arg)
        {
            Debug.Log("OnPress  " + GetType());
            (this.enity as UIEnity).panel.gameObject.SetActive(false);

        }
        protected override void OnTop(UIEventArgs arg)
        {
            Debug.Log("OnTop  " + GetType());
            (this.enity as UIEnity).panel.gameObject.SetActive(true);

        }
    }
    public class p2Policy:UIPolicy
    {

    }
    public class p2Excutor:UIPolicyExecutor
    {

    }
    public class p2View:UIView
    {

    }
    public class Panel2 :  UIPanel
    {
       
    }
    public class p1Sensor : pExpSensor
    {
       
    }
    public class p1Policy : UIPolicy
    {

    }
    public class p1Excutor : UIPolicyExecutor
    {

    }
    public class p1View : UIView
    {

    }
}
