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

namespace IFramework.Tweens
{
    [FrameworkVersion(8)]
    public static class TweenExtension
    {
        private static Tween<T> AllocateTween<T>(EnvironmentType env) where T : struct
        {
            return Tween<T>.Allocate<Tween<T>>(env);
        }
        private static Tween<T> ConfigAndRun<T>(this Tween<T> tween, T start, T end, float dur, Action<T> getter) where T:struct
        {
            tween.Config(start, end, dur, getter);
            tween.Run();
            return tween;
        }


        public static Tween<T> SetRecyle<T>(this Tween<T> tween, bool rec) where T : struct
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.autoRecyle = rec;
            return tween;
        }
        public static Tween<T> OnCompelete<T>(this Tween<T> tween, Action onCompelete) where T : struct
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.onCompelete += onCompelete;
            return tween;
        }
        public static Tween<T> SetLoop<T>(this Tween<T> tween, int loop, LoopType loopType) where T : struct
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.loop = loop;
            tween.loopType = loopType;
            return tween;
        }
        public static Tween<T> SetCurve<T>(this Tween<T> tween, ValueCurve curve) where T : struct
        {
            if (tween.recyled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.curve = curve;
            return tween;
        }


        public static Tween<Vector3> DoMove(this Transform trans, Vector3 end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.position, end, dur, (v3) => {
                trans.position = v3;
            });
        }
        public static Tween<Vector3> DoMoveX(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.position;
            endv3.x = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.position, endv3, dur, (v3) => {
                trans.position = v3;
            });
        }
        public static Tween<Vector3> DoMoveY(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.position;
            endv3.y = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.position, endv3, dur, (v3) => {
                trans.position = v3;
            });
        }
        public static Tween<Vector3> DoMoveZ(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.position;
            endv3.z = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.position, endv3, dur, (v3) => {
                trans.position = v3;
            });
        }

        public static Tween<Vector3> DoLocalMove(this Transform trans,Vector3 end,float dur,EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localPosition, end, dur, (v3) => {
                trans.localPosition = v3;
            });
        }
        public static Tween<Vector3> DoLocalMoveX(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.localPosition;
            endv3.x = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localPosition, endv3, dur, (v3) => {
                trans.localPosition = v3;
            });
        }
        public static Tween<Vector3> DoLocalMoveY(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.localPosition;
            endv3.y = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localPosition, endv3, dur, (v3) => {
                trans.localPosition = v3;
            });
        }
        public static Tween<Vector3> DoLocalMoveZ(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.localPosition;
            endv3.z = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localPosition, endv3, dur, (v3) => {
                trans.localPosition = v3;
            });
        }


        public static Tween<Vector3> DoScale(this Transform trans, Vector3 end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localScale, end, dur, (v3) => {
                trans.localScale = v3;
            });
        }
        public static Tween<Vector3> DoScaleX(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.localScale;
            endv3.x = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localScale, endv3, dur, (v3) => {
                trans.localScale = v3;
            });
        }
        public static Tween<Vector3> DoScaleY(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.localScale;
            endv3.y = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localScale, endv3, dur, (v3) => {
                trans.localScale = v3;
            });
        }
        public static Tween<Vector3> DoScaleZ(this Transform trans, float end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            Vector3 endv3 = trans.localScale;
            endv3.z = end;
            return AllocateTween<Vector3>(env).ConfigAndRun(trans.localScale, endv3, dur, (v3) => {
                trans.localScale = v3;
            });
        }


        public static Tween<Quaternion> DoRota(this Transform trans, Quaternion end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Quaternion>(env).ConfigAndRun(trans.rotation, end, dur, (v3) => {
                trans.rotation = v3;
            });
        }
        public static Tween<Quaternion> DoRota(this Transform trans, Vector3 end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Quaternion>(env).ConfigAndRun(trans.rotation, Quaternion.Euler(end), dur, (v3) => {
                trans.rotation = v3;
            });
        }
        public static Tween<Quaternion> DoLocalRota(this Transform trans, Quaternion end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Quaternion>(env).ConfigAndRun(trans.localRotation, end, dur, (v3) => {
                trans.localRotation = v3;
            });
        }
        public static Tween<Quaternion> DoLocalRota(this Transform trans, Vector3 end, float dur, EnvironmentType env = EnvironmentType.Ev1)
        {
            return AllocateTween<Quaternion>(env).ConfigAndRun(trans.localRotation, Quaternion.Euler(end), dur, (v3) => {
                trans.localRotation = v3;
            });
        }


        public static Tween<Color> DoColor(this Material mt,Color end,float dur,EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Color>(env).ConfigAndRun(mt.color, end, dur, (v3) => {
                mt.color = v3;
            });
        }
        public static Tween<Color> DoColor(this Light light, Color end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Color>(env).ConfigAndRun(light.color, end, dur, (v3) => {
                light.color = v3;
            });
        }
        public static Tween<Color> DoColor(this Camera camera, Color end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            return AllocateTween<Color>(env).ConfigAndRun(camera.backgroundColor, end, dur, (v3) => {
                camera.backgroundColor = v3;
            });
        }
        public static Tween<Color> DoAlpha(this Material mt, float end, float dur, EnvironmentType env= EnvironmentType.Ev1)
        {
            Color endColor = mt.color;
            endColor.a = end;
            return AllocateTween<Color>(env).ConfigAndRun(mt.color, endColor, dur, (color) => {
                mt.color = color;
            });
        }
       

    }

}
