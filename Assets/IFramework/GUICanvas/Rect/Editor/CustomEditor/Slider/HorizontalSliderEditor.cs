﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    [CustomGUINode(typeof(HorizontalSlider))]
    public class HorizontalSliderEditor : SliderNodeEditor
    {
        private HorizontalSlider toolbar { get { return element as HorizontalSlider; } }

        public override void OnSceneGUI(Action children)
        {
            if (!toolbar.active) return;
            BeginGUI();
            toolbar.value = GUI.HorizontalSlider(toolbar.position, toolbar.value, toolbar.startValue, toolbar.endValue, toolbar.slider, toolbar.thumb);
            if (children != null) children();

            EndGUI();
        }

    }
}
