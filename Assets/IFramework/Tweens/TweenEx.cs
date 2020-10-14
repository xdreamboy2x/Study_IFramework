/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
namespace IFramework.Tweens
{
    [Version(8)]
    public static class TweenEx
    {
        public static Tween<T> AllocateTween<T>(EnvironmentType env) where T : struct
        {
            return Tween<T>.Allocate<Tween<T>>(env);
        }

        public static Tween<T> DoGoto<T>(T start, T end, float dur, Func<T> getter, Action<T> setter, EnvironmentType env = EnvironmentType.Ev1) where T : struct
        {
            var tween = AllocateTween<T>(env);
            tween.Config(start, end, dur, getter, setter);
            tween.Run();
            return tween;
        }


        public static Tween SetRecyle(this Tween tween, bool rec)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.autoRecyle = rec;
            return tween;
        }
        public static Tween OnCompelete(this Tween tween, Action onCompelete)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.onCompelete += onCompelete;
            return tween;
        }
        public static Tween SetLoop(this Tween tween, int loop, LoopType loopType)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.loop = loop;
            tween.loopType = loopType;
            return tween;
        }
        public static Tween SetCurve(this Tween tween, ValueCurve curve)
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.curve = curve;
            return tween;
        }


        public static Tween<Vector3> DoMove(this Transform target, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.position, end, dur,
                    () => { return target.position; },
                    (value) => {
                        target.position = value;
                    }
                );
        }
        public static Tween<float> DoMoveX(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.position.x, end, dur, () => { return target.position.x; }, (value) => {
                target.position = new Vector3(value, target.position.y, target.position.z);
            });
        }
        public static Tween<float> DoMoveY(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.position.y, end, dur, () => { return target.position.y; },
                             (value) => {
                                 target.position = new Vector3(target.position.x, value, target.position.z);
                             });
        }
        public static Tween<float> DoMoveZ(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.position.z, end, dur, () => { return target.position.z; }, (value) => {
                target.position = new Vector3(target.position.x, target.position.y, value);
            });
        }

        public static Tween<Vector3> DoLocalMove(this Transform target, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localPosition, end, dur, () => { return target.localPosition; }, (value) => {
                target.localPosition = value;
            });
        }
        public static Tween<float> DoLocalMoveX(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localPosition.x, end, dur, () => { return target.localPosition.x; }, (value) => {
                target.localPosition = new Vector3(value, target.localPosition.y, target.localPosition.z);
            });
        }
        public static Tween<float> DoLocalMoveY(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localPosition.y, end, dur, () => { return target.localPosition.y; }, (value) => {
                target.localPosition = new Vector3(target.localPosition.x, value, target.localPosition.z);
            });
        }
        public static Tween<float> DoLocalMoveZ(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localPosition.z, end, dur, () => { return target.localPosition.z; }, (value) => {
                target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, value);
            });
        }


        public static Tween<Vector3> DoScale(this Transform target, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localScale, end, dur, () => { return target.localScale; }, (value) => {
                target.localScale = value;
            });
        }
        public static Tween<float> DoScaleX(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localScale.x, end, dur, () => { return target.localScale.x; }, (value) => {
                target.localScale = new Vector3(value, target.localScale.y, target.localScale.z);
            });
        }
        public static Tween<float> DoScaleY(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localScale.y, end, dur, () => { return target.localScale.y; }, (value) => {
                target.localScale = new Vector3(target.localScale.x, value, target.localScale.z);
            });
        }
        public static Tween<float> DoScaleZ(this Transform target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localScale.z, end, dur, () => { return target.localScale.z; }, (value) => {
                target.localScale = new Vector3(target.localScale.x, target.localScale.y, value);
            });
        }


        public static Tween<Quaternion> DoRota(this Transform target, Quaternion end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.rotation, end, dur, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            });
        }
        public static Tween<Vector3> DoRota(this Transform target, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.rotation.eulerAngles, end, dur, () => { return target.rotation.eulerAngles; }, (value) => {
                target.rotation = Quaternion.Euler(value);
            });
        }
        public static Tween<Quaternion> DoRotaFast(this Transform target, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.rotation, Quaternion.Euler(end), dur, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            });
        }

        public static Tween<Quaternion> DoLocalRota(this Transform target, Quaternion end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localRotation, end, dur, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            });
        }
        public static Tween<Quaternion> DoLocalRota(this Transform target, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.localRotation, Quaternion.Euler(end), dur, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            });
        }


        public static Tween<Color> DoColor(this Material target, Color end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.color, end, dur, () => { return target.color; }, (value) => {
                target.color = value;
            });
        }
        public static Tween<Color> DoColor(this Graphic target, Color end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.color, end, dur, () => { return target.color; }, (value) => {
                target.color = value;
            });
        }
        public static Tween<Color> DoColor(this Light target, Color end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.color, end, dur, () => { return target.color; }, (value) => {
                target.color = value;
            });
        }
        public static Tween<Color> DoColor(this Camera target, Color end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.backgroundColor, end, dur, () => { return target.backgroundColor; }, (value) => {
                target.backgroundColor = value;
            });
        }


        public static Tween<float> DoAlpha(this Material target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.color.a, end, dur, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            });
        }
        public static Tween<float> DoAlpha(this Graphic target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.color.a, end, dur, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            });
        }
        public static Tween<float> DoAlpha(this Light target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.color.a, end, dur, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            });
        }
        public static Tween<float> DoAlpha(this Camera target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.backgroundColor.a, end, dur, () => { return target.backgroundColor.a; }, (value) => {
                target.backgroundColor = new Color(target.backgroundColor.r, target.backgroundColor.g, target.backgroundColor.b, value);
            });
        }
        public static Tween<float> DoAlpha(this CanvasGroup target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.alpha, end, dur, () => { return target.alpha; }, (value) => {
                target.alpha = value;
            });
        }



        public static Tween<int> DoText(this Text target, int start, int end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(start, end, dur, () => {
                int value;
                if (int.TryParse(target.text, out value))
                    return value;
                return 0;
            }, (value) => {
                target.text = value.ToString();
            });
        }
        public static Tween<int> DoText(this Text target, string end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.text.Length, end.Length, dur, () =>{ return target.text.Length;}, (value) => {
                target.text = end.Substring(0, value);
            });
        }
        public static Tween<float> DoText(this Text target, float start, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(start, end, dur, () => {
                float value;
                if (float.TryParse(target.text, out value))
                    return value;
                return 0;
            }, (value) => {
                target.text = value.ToString();
            });
        }



        public static Tween<float> DoFillAmount(this Image target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.fillAmount, end, dur, () => { return target.fillAmount;}, (value) => {
                target.fillAmount = value;
            });
        }
        public static Tween<float> DoNormalizedPositionX(this ScrollRect target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.normalizedPosition.x, end, dur, () => { return target.normalizedPosition.x; }, (value) => {
                target.normalizedPosition = new Vector2(value, target.normalizedPosition.y);
            });
        }
        public static Tween<float> DoNormalizedPositionY(this ScrollRect target, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.normalizedPosition.y, end, dur, () => { return target.normalizedPosition.y; }, (value) => {
                target.normalizedPosition = new Vector2(target.normalizedPosition.x, value);
            });
        }




        public static Tween<bool> DoActive(this GameObject target, bool end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.activeSelf, end, dur, () => { return target.activeSelf; }, (value) => {
                target.SetActive(value);
            });
        }
        public static Tween<bool> DoEnable(this Behaviour target, bool end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.enabled, end, dur, () => { return target.enabled; }, (value) => {
                target.enabled = value;
            });
        }
        public static Tween<bool> DoToggle(this Toggle target, bool end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return DoGoto(target.isOn, end, dur, () => { return target.isOn; }, (value) => {
                target.isOn = value;
            });
        }


    }

}
