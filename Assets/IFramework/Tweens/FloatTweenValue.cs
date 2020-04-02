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
            if (start < end)
            {
                float f = curve.GetYWithX(percent);
                cur = (end - start) * f + start;
            }
            else
            {
                float f = curve.GetYWithX(percent);
                cur = (start - end) * f + end;
            }
        }
    }

}
