/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace IFramework.Hotfix.AB
{
    public class Bundle : Reference
    {
        protected void ThrowError(string err)
        {
            InvokeError(string.Format("URL: {0}  Err: {1}", url, err));
        }

        protected bool _isdone;
        protected AssetBundle _assetbundle;


        public string name { get; private set; }
        public string url { get; private set; }
        public Hash128 version { get; private set; }


        public virtual float progress
        {
            get
            {
                return _assetbundle == null ? 0 : 1;
            }
        }
        public override bool isDone
        {
            get
            {
                return DependenceBundleIsDone() && _isdone;
            }
        }
        protected bool DependenceBundleIsDone()
        {
            for (int i = 0; i < dpBundles.Count; i++)
                if (!dpBundles[i].isDone)
                    return false;
            return true;
        }


        public AssetBundle assetbundle { get { return _assetbundle; } }
        private readonly List<Bundle> dpBundles = new List<Bundle>();



        public Bundle(string url, string name, List<Bundle> dps, Hash128 version)
        {
            this.name = name;
            this.url = url;
            this.version = version;
            for (int i = 0; i < dps.Count; i++)
            {
                dps[i].onError += ThrowError;
                dpBundles.Add(dps[i]);
            }
        }
        public override void Dispose()
        {
            for (int i = 0; i < dpBundles.Count; i++)
            {
                dpBundles[i].onError -= ThrowError;
                dpBundles[i].Release();
            }
            dpBundles.Clear();
        }

        protected override void OnLoad()
        {
            _assetbundle = AssetBundle.LoadFromFile(url);
            if (_assetbundle == null)
                ThrowError(" LoadFromFile failed.");
            else
            {
                _isdone = true;
            }
        }
        protected override void OnUnLoad()
        {
            if (_assetbundle == null) return;
            _assetbundle.Unload(true);
            _assetbundle = null;
        }
        public T LoadAsset<T>(string assetName) where T : Object
        {
            return LoadAsset(assetName, typeof(T)) as T;
        }
        public Object LoadAsset(string assetName, Type type)
        {
            if (!string.IsNullOrEmpty(error) || assetbundle == null) return null;
            return assetbundle.LoadAsset(assetName, type);
        }
        public AssetBundleRequest LoadAssetAsync(string assetName, Type type)
        {
            if (!string.IsNullOrEmpty(error) || assetbundle == null) return null;
            return assetbundle.LoadAssetAsync(assetName, type);
        }
    }
    public class AsyncBundle : Bundle
    {
        private AssetBundleCreateRequest request;
        public override float progress
        {
            get
            {
                if (request == null)
                    return 0;
                return request.progress;
            }
        }

        public AsyncBundle(string url, string name, List<Bundle> dps, Hash128 hash) : base(url, name, dps, hash) { }
        protected override void OnLoad()
        {
            request = AssetBundle.LoadFromFileAsync(url);
            request.completed += (op) =>
            {
                _isdone = true;
                _assetbundle = request.assetBundle;
            };
            if (request == null)
            {
                ThrowError(" LoadFromFileAsync falied.");
                _isdone = true;
            }
        }
        protected override void OnUnLoad()
        {
            if (request == null) return;
            if (request.assetBundle != null)
                request.assetBundle.Unload(true);
            request = null;
        }

    }
    public class WebRequestBundle : Bundle
    {
        private UnityWebRequest request;
        public WebRequestBundle(string url, string name, List<Bundle> dps, Hash128 hash) : base(url, name, dps, hash) { }
        public override float progress
        {
            get
            {
                if (request == null)
                    return 0;
                return request.downloadProgress;
            }
        }
        protected override void OnLoad()
        {
            request = UnityWebRequestAssetBundle.GetAssetBundle(url, version);
            request.timeout = 30;
            UnityWebRequestAsyncOperation op = request.SendWebRequest();
            op.completed += Completed;
        }

        private void Completed(AsyncOperation obj)
        {
            if (request.isNetworkError)
                ThrowError(request.error);
            else
                _assetbundle = DownloadHandlerAssetBundle.GetContent(request);
            _isdone = true;
        }

        protected override void OnUnLoad()
        {
            if (request == null) return;
            if (_assetbundle != null)
                _assetbundle.Unload(true);
            request.Dispose();
            request = null;
        }
    }
}
