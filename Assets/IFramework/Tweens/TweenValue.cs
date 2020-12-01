/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using IFramework.NodeAction;
using UnityEngine;

namespace IFramework.Tweens
{
    [Version(22)]
    public abstract class TweenValue : RecyclableObject
    {
        private ValueCurve _curve = ValueCurve.linecurve;
        private float _dur;
        private SequenceNode _node;
        private float _curdur;
        protected float delta { get { return Time.deltaTime; } }

        protected event Action onCompelete;

        public ValueCurve curve { get { return _curve; } set { _curve = value; } }
        public bool compeleted { get; private set; }
        public float dur { get { return _dur; } set { _dur = value; } }
        protected float percent { get { return (_curdur / dur).Clamp01(); } }
        protected float percentDelta { get { return delta + (1 - delta) * percent; } }

        public void Run()
        {
            _node = this.Sequence(this.env)
                         .While(() => {
                             return percent < 1;
                         },
                         () => {
                             if (recyled) return;
                             _curdur += Time.deltaTime;
                             MoveNext();
                         })
                         .OnCompelete(() =>
                         {
                             _curdur = 0;
                             OnComplete();
                             if (onCompelete != null)
                                 onCompelete();
                             compeleted = true;
                         })
                         .Run();
        }
        protected abstract void MoveNext();
        protected abstract void OnComplete();


        protected override void OnDataReset()
        {

            if (_node != null && !_node.recyled)
            {
                _node.Recyle();
                _node = null;
            }
            onCompelete = null;
            _curdur = _dur = 0f;

            _curve = ValueCurve.linecurve;
            compeleted = false;
        }

        public static TweenValue<T> Get<T>(EnvironmentType envType) where T : struct
        {
            Type type;
            if (map.TryGetValue(typeof(T), out type))
                return Allocate(type, envType) as TweenValue<T>;
            throw new Exception(string.Format("Do not Have TweenValue<{0}>  with Type {0}", typeof(T)));
        }
        private static Dictionary<Type, Type> map = new Dictionary<Type, Type>()
        {
            {typeof(bool),typeof(BoolTweenValue) },
            {typeof(int),typeof(IntTweenValue) },
            {typeof(float),typeof(FloatTweenValue) },
            {typeof(Vector2),typeof(Vector2TweenValue) },
            {typeof(Vector3),typeof(Vector3TweenValue) },
            {typeof(Vector4),typeof(Vector4TweenValue) },
            {typeof(Color),typeof(ColorTweenValue) },
            {typeof(Rect),typeof(RectTweenValue) },
            {typeof(Quaternion),typeof(QuaternionTweenValue) },
        };
    }

    [Version(4)]
    public abstract class TweenValue<T> : TweenValue where T : struct
    {
        private T _cur;
        private T _end;
        private T _start;


        public T cur
        {
            get { return _cur; }
            set
            {
                _cur = value;
                if (setter != null)
                {
                    setter(value);
                }
            }
        }
        public T end
        {
            get { return _end; }
            set
            {
                _end = value;
            }
        }
        public T start
        {
            get { return _start; }
            set
            {
                _start = value;
            }
        }

        private Action<T> setter;
        private Func<T> getter;

        protected T targetValue { get { return getter.Invoke(); } }
        public virtual void Config(T start, T end, float dur, Func<T> getter, Action<T> setter, Action onCompelete)
        {
            this._start = this._cur = start;
            this._end = end;
            this.dur = dur;
            this.onCompelete += onCompelete;
            this.setter = setter;
            this.getter = getter;
            SetDataDirty();
        }
        protected override void OnComplete()
        {
            // cur = end;
        }

        protected override void OnDataReset()
        {
            base.OnDataReset();
            _cur = _start = _end = default(T);
            setter = null;
            getter = null;
        }
    }
}
