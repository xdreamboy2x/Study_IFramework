/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.146
 *UnityVersion:   2018.4.17f1
 *Date:           2020-04-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using IFramework.Modules.NodeAction;
using UnityEngine;

namespace IFramework.Tweens
{

    public enum LoopType
    {
        ReStart,
        PingPong
    }
    public class Tween<T> : RecyclableObject where T : struct
    {
        private TweenValue<T> _tv;
        private RepeatNode _repeat;
        private SequenceNode _seq;

        private T _cur;
        private T _end;
        private T _start;
        private float _dur;
        private LoopType _loopType;
        private bool _autoRecyle = true;
        private ValueCurve _curve = ValueCurve.linecurve;
        public event Action onCompelete;
        private Action<T> getter;
        private int _loop = 1;



        public T end { get { return _end; } set { _end = value; } }
        public T start { get { return _start; } set { _start = value; } }
        public float dur { get { return _dur; } set { _dur = value; } }
        public bool autoRecyle { get { return _autoRecyle; } set { _autoRecyle = value; } }
        public LoopType loopType { get { return _loopType; } set { _loopType = value; } }

        public T cur
        {
            get { return _cur; }
            set
            {
                _cur = value;
                if (getter != null)
                {
                    getter(value);
                }
            }
        }
        public int loop
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
        public ValueCurve curve
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


        public virtual void Config(T start, T end, float dur, Action<T> getter)
        {
            this._start = this.cur = start;
            this._end = end;
            this._dur = dur;
            this.getter = getter;
            SetDataDirty();
        }
        public void Run()
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
                                    _tv.Config(start, end, dur, (value) => { cur = value; }, null);
                                    break;
                                case LoopType.PingPong:
                                    if (cur.Equals(start))
                                        _tv.Config(start, end, dur, (value) => { cur = value; }, null);
                                    else if(cur .Equals(end))
                                        _tv.Config(end, start, dur, (value) => { cur = value; }, null);
                                    else
                                        _tv.Config(start, end, dur, (value) => { cur = value; }, null);

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
                    if (onCompelete != null)
                        onCompelete();
                    TryRecyleSelf();
                })
                .Run();
        }

        public void ReStart()
        {
            if (recyled) return;

            RecycleInner();
            Run();
        }
        public void Rewind(float dur)
        {
            if (recyled) return;

            RecycleInner();

            _tv = TweenValue.Get<T>(env.envType);
            _tv.curve = curve;
            _tv.Config(cur, start, dur,
                (value) => { cur = value; },
                () => {
                    TryRecyleSelf();
                });
            _tv.Run();
        }
        public void Complete(bool invoke)
        {
            if (recyled) return;

            if (invoke && onCompelete != null)
            {
                onCompelete();
            }
            TryRecyleSelf();
        }


        protected override void OnDataReset()
        {
            RecycleInner();
            _cur = _start = _end = default(T);
            _dur = 0;
            _loop = 1;
            _autoRecyle = true;
            _curve = ValueCurve.linecurve;
            _loopType = LoopType.ReStart;
            getter = null;
            onCompelete = null;

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

        protected override void OnRecyle()
        {
            base.OnRecyle();
            Debug.Log("rec");
        }
    }

}
