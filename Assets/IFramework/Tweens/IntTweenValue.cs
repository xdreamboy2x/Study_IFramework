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
    [Version(5)]
    public class IntTweenValue : TweenValue<int>
    {
        protected override void MoveNext()
        {
            var point = curve.GetPercent(percent);
            int _cur = start.Lerp(end, point.y);
            cur = targetValue.Lerp(_cur,percentDelta);
        }
    }
}
