/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.UI
{
    public enum UILayer
    {
        BGBG,               //非常BG
        Background,         //BG
        AnimationUnderPage, //背景动画
        Common,             //普通
        AnimationOnPage,    //上层动画
        PopUp,              //弹框
        Guide,              //引导
        Toast,              //对话框
        Top,                //Top
        TopTop,             //非常Top
    }
    public abstract class UIPanel : MonoBehaviour
    {
        public UILayer layer { get; set; }
    }
}
