/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.NodeAction;
using System;

namespace IFramework.Tweens
{
    public enum LoopType
    {
        ReStart,
        PingPong
    }
    public enum TweenDirection
    {
        Forward,Back
    }
    public abstract class Tween:RecyclableObject
    {
        private TweenDirection _direction = TweenDirection.Forward;
        public TweenDirection direction { get { return _direction; }protected set { _direction = value; } }

        public event Action onCompelete;
        public float dur;
        public bool autoRecyle = true;
        public LoopType loopType;
        public abstract int loop { get; set; }
        public abstract ValueCurve curve { get; set; }
        public abstract void Run();
        public abstract void ReStart();
        public abstract void Rewind(float dur);
        public abstract void Complete(bool invoke);

        protected void InvokeCompelete()
        {
            if (onCompelete!=null)
            {
                onCompelete.Invoke();
            }
        }
        protected override void OnDataReset()
        {
            onCompelete = null;
        }
    }
    [Version(12)]
    public class Tween<T> : Tween where T : struct
    {
        private TweenValue<T> _tv;
        private RepeatNode _repeat;
        private SequenceNode _seq;

        private T _cur;
        private T _end;
        private T _start;
        private ValueCurve _curve = ValueCurve.linecurve;
        private Action<T> setter;
        private Func<T> getter;
        private int _loop = 1;

        public T end { get { return _end; } set { _end = value; } }
        public T start { get { return _start; } set { _start = value; } }

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
        public override int loop
        {
            get
            {
                if (_repeat != null && !_repeat.recyled)
                    _loop = _repeat.repeat;
                return _loop;
            }
            set
            {
                _loop = value;
                if (_repeat != null && !_repeat.recyled)
                {
                    _repeat.repeat = value;
                }
            }
        }
        public override ValueCurve curve
        {
            get { return _curve; }
            set
            {
                _curve = value;
                if (_tv != null && !_tv.recyled)
                {
                    _tv.curve = value;
                }
            }
        }


        public virtual void Config(T start, T end, float dur, Func<T> getter, Action<T> setter)
        {
            this._start = this.cur = start;
            this._end = end;
            this.dur = dur;
            this.getter = getter;
            this.setter = setter;
            SetDataDirty();
        }


        public override void Run()
        {
            if (recyled) return;
            _seq = this.Sequence(env.envType)
                .Repeat((r) =>
                {
                    _repeat = r.Sequence((s) =>
                    {
                        s.Until(() => {
                            if (recyled) return true;
                            return _tv.compeleted;
                        })
                        .OnCompelete(() => {
                            if (_tv!=null)
                            {
                                _tv.Recyle();
                                _tv = null;
                            }
                        })
                        .OnBegin(() => {
                            if (recyled) return;
                            _tv = TweenValue.Get<T>(env.envType);
                            _tv.curve = curve;
                            switch (loopType)
                            {
                                case LoopType.ReStart:
                                    _tv.Config(start, end, dur, getter,(value) => { cur = value; }, null);
                                    break;
                                case LoopType.PingPong:
                                    if (direction== TweenDirection.Forward)
                                    {
                                        _tv.Config(start, end, dur, getter, (value) => { cur = value; }, null);
                                        direction =  TweenDirection.Back;
                                    }
                                    else
                                    {
                                        _tv.Config(end, start, dur, getter, (value) => { cur = value; }, null);
                                        direction =  TweenDirection.Forward;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            _tv.Run();
                        });
                    });
                }, loop)
                .OnCompelete(() =>
                {
                    InvokeCompelete();
                    TryRecyleSelf();
                })
                .Run();
        }

        public override void ReStart()
        {
            if (recyled) return;
            direction = TweenDirection.Forward;
            RecycleInner();
            Run();
        }
        public override void Rewind(float dur)
        {
            if (recyled) return;
            direction = TweenDirection.Forward;

            RecycleInner();

            _tv = TweenValue.Get<T>(env.envType);
            _tv.curve = curve;
            _tv.Config(cur, start, dur, getter,
                (value) => { cur = value; },
                () => {
                    TryRecyleSelf();
                });
            _tv.Run();
        }
        public override void Complete(bool invoke)
        {
            if (recyled) return;
            direction = TweenDirection.Forward;

            if (invoke )
            {
                InvokeCompelete();
            }
            TryRecyleSelf();
        }


        protected override void OnDataReset()
        {
            base.OnDataReset();
            direction = TweenDirection.Forward;
            RecycleInner();
            _cur = _start = _end = default(T);
            dur = 0;
            _loop = 1;
            autoRecyle = true;
            _curve = ValueCurve.linecurve;
            loopType = LoopType.ReStart;
            setter = null;
            getter = null;
        }

        private void TryRecyleSelf()
        {
            RecycleInner();
            if (autoRecyle)
            {
                Recyle();
            }
        }
        private void RecycleInner()
        {
            if (_tv != null && !_tv.recyled)
            {
                _tv.Recyle();
                _tv = null;
            }
            if (_seq != null && !_seq.recyled)
            {
                _seq.Recyle();
                _seq = null;
            }
            if (_repeat != null && !_repeat.recyled)
            {
                _repeat.Recyle();
                _repeat = null;
            }
        }
    }
}
