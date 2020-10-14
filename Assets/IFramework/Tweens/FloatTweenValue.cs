/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework.Tweens
{
    public class FloatTweenValue : TweenValue<float>
    {
        protected override void MoveNext()
        {
            var point = curve.GetPercent(percent);
            float _cur = start.Lerp(end, point.y);
            cur = targetValue.Lerp(_cur,percentDelta);
        }
    }

}
