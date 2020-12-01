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
    public class QuaternionTweenValue : TweenValue<Quaternion>
    {
        protected override void MoveNext()
        {
            var point = curve.GetPercent(percent);
            Quaternion _cur = Quaternion.Lerp(start, end, point.y);
            cur = Quaternion.Lerp(  targetValue, _cur, percentDelta);
        }
    }

}
