/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IFramework.Hotfix.AB
{
    public class Asset : Reference
    {
        protected void ThrowError(string err)
        {
            InvokeError(string.Format("ABAsset:  {0}", err));
        }
        protected Type _assetType { get; private set; }
        protected Object _asset;
        protected bool _isdone;


        public Object asset { get { return _asset; } }
        public string assetPath { get; private set; }
        public override bool isDone { get { return _isdone; } }


        public Asset(string path, Type type)
        {
            assetPath = path;
            _assetType = type;
        }
        protected override void OnLoad()
        {
#if UNITY_EDITOR
            try
            {
                _asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, _assetType);
                _isdone = true;
            }
            catch (Exception e)
            {
                ThrowError(e.Message);
            }
#endif
        }

        protected override void OnUnLoad()
        {
            assetPath = null;
            _asset = null;
        }

       
    }
    public class BundleAsset : Asset
    {
        protected Bundle bundle;
        internal BundleAsset(string path, Type type) : base(path, type) { }
        protected override void OnLoad()
        {
            bundle = Assets.bundles.LoadSync(Assets.GetBundleName(assetPath));
            bundle.onError += ThrowError;
            _asset = bundle.LoadAsset(Assets.GetAssetName(assetPath), _assetType);
            _isdone = true;
        }
        protected override void OnUnLoad()
        {
            base.OnUnLoad();
            if (bundle != null)
            {
                bundle.onError -= ThrowError;
                bundle.Release();
                bundle = null;
            }
        }
    }
    public class AsyncBundleAsset : BundleAsset
    {
        private AssetBundleRequest request;
        public AsyncBundleAsset(string path, Type type) : base(path, type) { }
        protected override void OnLoad()
        {
            bundle = Assets.bundles.LoadAsync(Assets.GetBundleName(assetPath));
            bundle.onError += ThrowError;
        }
        protected override void OnUnLoad()
        {
            base.OnUnLoad();
            request = null;
        }
        public override bool isDone
        {
            get
            {
                if (bundle == null) return false;
                if (!bundle.isDone) return false;
                if (request == null)
                {
                    request = bundle.LoadAssetAsync(Path.GetFileName(assetPath), _assetType);
                    request.completed += (op) => {
                        _asset = request.asset;
                    };
                    return false;
                }
                return request.isDone;
            }
        }
    }
}
