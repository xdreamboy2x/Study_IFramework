/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.Tweens
{
    public class RectTweenValue : TweenValue<Rect>
    {
        protected override void MoveNext()
        {
            var point = curve.GetPercent(percent);

            Rect _cur = start.Lerp(start, end, point.y);
            cur = start.Lerp(  targetValue, _cur, percentDelta);
        }

    }

}
