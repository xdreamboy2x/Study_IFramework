/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Modules;
using IFramework.Serialization;
using IFramework.Serialization.DataTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace IFramework.Language
{
    public class LanguageKeyAttribute : PropertyAttribute { }

    [Serializable]
    public class LanPair
    {
        public SystemLanguage lan;
        public string key;
        public string value;
    }
    public class LanguageModule : FrameworkModule
    {
        public class LanObserver : IDisposable
        {
            private bool _disposed;
            private bool _paused;
            public string key { get; private set; }
            public SystemLanguage curLan { get { return _moudle._lan; } }
            public SystemLanguage fallback { get; private set; }
            private List<LanPair> _pairs { get { return _moudle._keyDic[key]; } }
            private LanguageModule _moudle;
            private Action<SystemLanguage, string> _observeEvent;


            internal LanObserver(LanguageModule moudle, string key, SystemLanguage fallback, bool autoStart)
            {
                this._moudle = moudle;
                moudle._lanObservers.Add(this);
                this.key = key;
                this.fallback = fallback;
                moudle._observeEvent += ObserveEvent;
                _disposed = false;
                Pause();
                if (autoStart) Start();
            }
            public LanObserver ObserveEvent(Action<SystemLanguage, string> eve)
            {
                this._observeEvent = eve;
                return this;
            }
            private string GetValueInPairs()
            {
                LanPair pair = _pairs.Find(p => { return p.lan == curLan; });
                if (pair == null)
                    pair = _pairs.Find(p => { return p.lan == fallback; });
                return pair == null ? string.Empty : pair.value;
            }
            private void ObserveEvent()
            {
                if (_disposed || _paused) return;
                if (_observeEvent != null)
                    _observeEvent(curLan, GetValueInPairs());
            }
            public void Start() { UnPause(); }

            public void Pause()
            {
                _paused = true;
            }
            public void UnPause()
            {
                _paused = false;
            }
            public void Dispose()
            {
                Pause();
                _disposed = true;
                _observeEvent = null;
                _moudle._observeEvent -= ObserveEvent;
                _moudle._lanObservers.Remove(this);
            }
        }

        private List<LanPair> _lanPairs;
        private Dictionary<string, List<LanPair>> _keyDic;
        private List<LanObserver> _lanObservers;
        private event Action _observeEvent;

        private SystemLanguage _lan = SystemLanguage.Unknown;
        public override int priority { get { return 90; } }

        public SystemLanguage lan
        {
            get { return _lan; }
            set
            {
                if (_lan == value) return;
                _lan = value;
                PublishLanguage();
            }
        }

        public void Load(List<LanPair> pairs, bool reWrite = true)
        {
            pairs.ForEach((tmpPair) => {
                LanPair pair = _lanPairs.Find((p) => { return p.lan == tmpPair.lan && p.key == tmpPair.key; });
                if (pair != null && reWrite && pair.value != tmpPair.value)
                    pair.value = tmpPair.value;
                else
                    _lanPairs.Add(tmpPair);
            });
            pairs.Clear();
            _keyDic = _lanPairs.GroupBy(lanPair => { return lanPair.key; }, (key, list) => { return new { key, list }; })
                     .ToDictionary((v) => { return v.key; }, (v) => { return v.list.ToList(); });
            if (_observeEvent != null) _observeEvent();
        }

        public void PublishLanguage()
        {
            if (_observeEvent != null)
                _observeEvent();
        }
       
        public LanObserver CreatObserver(string key, SystemLanguage fallback, bool autoStart = true)
        {
            if (!_keyDic.ContainsKey(key)) throw new Exception(string.Format("Key Not Found {0}", key));
            List<LanPair> pairs = _keyDic[key];
            var fallbackPair = pairs.Find((p) => { return p.lan == fallback; });
            if (fallbackPair == null) Log.W(string.Format("Fallback Language Not Exist Key: {0} : Lan: {1}", key, fallback));
            LanObserver observer = new LanObserver(this, key, fallback, autoStart);
            return observer;
        }

        protected override void Awake()
        {
            _lanPairs = new List<LanPair>();
            _lanObservers = new List<LanObserver>();
        }
        protected override void OnDispose()
        {
            _lanPairs.Clear();
            _lanObservers.Clear();
        }
    }

    public static class LanguageModuleEx
    {
        public static void LoadCsv(this LanguageModule module, string path, bool reWrite = true)
        {
            DataReader dw = new DataReader(new StreamReader(path, System.Text.Encoding.UTF8), new DataRow(), new DataExplainer());
            var pairs = dw.Get<LanPair>();
            module.Load(pairs, reWrite);
        }
        public static void LoadXml(this LanguageModule module, string xml, bool reWrite = true)
        {
            var pairs = Xml.ToObject<List<LanPair>>(xml);
            module.Load(pairs, reWrite);
        }
        public static void LoadJson(this LanguageModule module, string json, bool reWrite = true)
        {
            var pairs = JsonUtility.FromJson<List<LanPair>>(json);
            module.Load(pairs, reWrite);
        }
        public static void LoadScriptableObject(this LanguageModule module, LanGroup group, bool reWrite = true)
        {
            var pairs = group.pairs;
            module.Load(pairs, reWrite);
        }
    }
}
