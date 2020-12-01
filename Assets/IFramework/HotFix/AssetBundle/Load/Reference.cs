/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Utility;
using System;

namespace IFramework.Hotfix.AB
{
    public abstract class Reference : SimpleReference, IDisposable
    {
        public string error { get; private set; }
        public abstract bool isDone { get; }
        public bool useful
        {
            get { return RefCount > 0; }
        }
        public event Action<string> onError;



        internal void Load() { OnLoad(); }
        internal void UnLoad() { OnUnLoad(); }


        protected void InvokeError(string err)
        {
            error = err;
            if (onError != null)
                onError(error);
        }
        protected abstract void OnLoad();
        protected abstract void OnUnLoad();

        public virtual void Dispose() { }
    }

}
