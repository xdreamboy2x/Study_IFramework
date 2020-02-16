/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IFramework.Modules;
using IFramework.Modules.MVP;
namespace IFramework
{
    public class UIEnity:MVPEnity 
    {
        public UIPanel panel;
        protected override void OnDestory()
        {
            base.OnDestory();
            Debug.Log("des");
            if (panel!=null && panel.gameObject!=null)
                GameObject.Destroy(panel.gameObject);
        }
    }
    internal interface IUISensor
    {
        void OnLoad();
        void OnTop(UIEventArgs arg);
        void OnPress(UIEventArgs arg);
        void OnPop(UIEventArgs arg);
        void OnClear();
    }
    public abstract class UISensor:SensorSystem, IUISensor
    {
        void IUISensor.OnLoad()
        {
            OnLoad();
        }
        void IUISensor.OnTop(UIEventArgs arg)
        {
            OnTop(arg);
        }
        void IUISensor.OnPress(UIEventArgs arg)
        {
            OnPress(arg);
        }
        void IUISensor.OnPop(UIEventArgs arg)
        {
            OnPop(arg);
        }
        void IUISensor.OnClear()
        {
            OnClear();
        }

        protected abstract void OnLoad();
        protected abstract void OnTop(UIEventArgs arg);
        protected abstract void OnPress(UIEventArgs arg);
        protected abstract void OnPop(UIEventArgs arg);
        protected abstract void OnClear();

    }
    public abstract class UIPolicy : PolicySystem
    {
        protected override void OnSensor(int code, IEventArgs args, object[] param)
        {
               
        }
    }
    public abstract class UIPolicyExecutor : PolicyExecutorSystem
    {
        protected override void OnPolicy(int code, IEventArgs args, object[] param)
        {
        }
    }
    public abstract class UIView : ViewSystem
    {
        protected override void OnPolicyPolicyExecutor(int code, IEventArgs args, object[] param)
        {
                
        }
    }
   
   

  
    public class UIModule : FrameworkModule
    {
        private class UIEnitys : FrameworkModuleContainer
        {
            public Dictionary<Type, Tuple<Type, Type, Type, Type, Type>> map;
            public UIEnitys(string chunck) : base(chunck, null, false)
            {

            }
            public MVPModule Subscribe(UIPanel panel)
            {
                var module = FindEnity(panel);
                if (module != null) throw new Exception(string.Format("Have Subscribe Panel Name: {0}", panel.PanelName));
                module = CreateModule<MVPModule>(panel.PanelName);
                Tuple<Type, Type, Type, Type, Type> tuple;
                map.TryGetValue(panel.GetType(), out tuple);
                if (tuple == null) throw new Exception(string.Format("Could Not Find map with Type: {0}", panel.GetType()));
                var enity = Activator.CreateInstance(tuple.Item1) as UIEnity;
                enity.panel = panel;

                var sensor = Activator.CreateInstance(tuple.Item2) as UISensor;
                var policy = Activator.CreateInstance(tuple.Item3) as UIPolicy;
                var executor = Activator.CreateInstance(tuple.Item4) as UIPolicyExecutor;
                var view = Activator.CreateInstance(tuple.Item5) as UIView;

                module.enity = enity;
                module.sensor = sensor;
                module.policyExecutor = executor;
                module.policy = policy;
                module.view = view;
                return module;
            }
            public MVPModule FindEnity(UIPanel panel)
            {
                return FindEnity(panel.PanelName);
            }
            public MVPModule FindEnity(string panelName)
            {
                return FindModule<MVPModule>(panelName);
            }
            public void UnSubscribe(UIPanel panel)
            {
                var module = FindEnity(panel);
                module.UnBind(false);
            }
        }
        private UIEnitys _enitys;

        public Canvas Canvas { get; private set; }
        private RectTransform UIRoot;
        public RectTransform BGBG { get; private set; }
        public RectTransform Background { get; private set; }
        public RectTransform AnimationUnderPage { get; private set; }
        public RectTransform Common { get; private set; }
        public RectTransform AnimationOnPage { get; private set; }
        public RectTransform PopUp { get; private set; }
        public RectTransform Guide { get; private set; }
        public RectTransform Toast { get; private set; }
        public RectTransform Top { get; private set; }
        public RectTransform TopTop { get; private set; }
        public RectTransform UICamera { get; private set; }

        private RectTransform InitTransform(string name)
        {
            GameObject go = new GameObject(name);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(Canvas.transform);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localPosition = Vector3.zero;
            rect.sizeDelta = Vector3.zero;
            return rect;
        }
        protected override void Awake()
        {
            UIRoot = new GameObject(name).AddComponent<RectTransform>();
            Canvas = UIRoot.gameObject.AddComponent<Canvas>();
            UIRoot.gameObject.AddComponent<CanvasScaler>();
            UIRoot.gameObject.AddComponent<GraphicRaycaster>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            BGBG = InitTransform("BGBG");
            Background = InitTransform("Background");
            AnimationUnderPage = InitTransform("AnimationUnderPage");
            Common = InitTransform("Common");
            AnimationOnPage = InitTransform("AnimationOnPage");
            PopUp = InitTransform("PopUp");
            Guide = InitTransform("Guide");
            Toast = InitTransform("Toast");
            Top = InitTransform("Top");
            TopTop = InitTransform("TopTop");
            UICamera = InitTransform("UICamera");

            //panelDic = new Dictionary<Type, List<UIPanel>>();
            UIStack = new Stack<UIPanel>();
            UICache = new Stack<UIPanel>();
            loaders = new List<LoadDel>();
            _enitys = new UIEnitys(chunck);
        }
        protected override void OnDispose()
        {
            //panelDic.Clear();
            _enitys.Dispose();
            UIStack.Clear();
            UICache.Clear();
            loaders.Clear();
            if (Canvas!=null)
                GameObject.Destroy(Canvas.gameObject);
        }
        protected override void OnUpdate() { _enitys.Update(); }


        public delegate UIPanel LoadDel(Type type, string path, string name, UIPanelLayer layer);
        private List<LoadDel> loaders;
        public int LoaderCount { get { return loaders.Count; } }
        public void AddLoader(LoadDel loader)
        {
            loaders.Add(loader);
        }
        public void SetCamera(Camera ca, bool isLast = true, int index = -1)
        {
            UICamera.SetChildWithIndex(ca.transform, !isLast ? index : UICamera.childCount);
        }
        public void SetParent(UIPanel ui, bool isLast = true, int index = -1)
        {
            switch (ui.PanelLayer)
            {
                case UIPanelLayer.BGBG:
                    BGBG.SetChildWithIndex(ui.transform, !isLast ? index : BGBG.childCount);
                    break;
                case UIPanelLayer.Background:
                    Background.SetChildWithIndex(ui.transform, !isLast ? index : Background.childCount);
                    break;
                case UIPanelLayer.AnimationUnderPage:
                    AnimationUnderPage.SetChildWithIndex(ui.transform, !isLast ? index : AnimationUnderPage.childCount);
                    break;
                case UIPanelLayer.Common:
                    Common.SetChildWithIndex(ui.transform, !isLast ? index : Common.childCount);
                    break;
                case UIPanelLayer.AnimationOnPage:
                    AnimationOnPage.SetChildWithIndex(ui.transform, !isLast ? index : AnimationOnPage.childCount);
                    break;
                case UIPanelLayer.PopUp:
                    PopUp.SetChildWithIndex(ui.transform, !isLast ? index : PopUp.childCount);
                    break;
                case UIPanelLayer.Guide:
                    Guide.SetChildWithIndex(ui.transform, !isLast ? index : Guide.childCount);
                    break;
                case UIPanelLayer.Toast:
                    Toast.SetChildWithIndex(ui.transform, !isLast ? index : Toast.childCount);
                    break;
                case UIPanelLayer.Top:
                    Top.SetChildWithIndex(ui.transform, !isLast ? index : Top.childCount);
                    break;
                case UIPanelLayer.TopTop:
                    TopTop.SetChildWithIndex(ui.transform, !isLast ? index : TopTop.childCount);
                    break;
                default:
                    break;
            }
            ui.transform.LocalIdentity();
        }
        public void SetMap(Dictionary<Type, Tuple<Type, Type, Type, Type, Type>> map)
        {
            _enitys.map = map;
        }
        public UIPanel Load(Type type, string path, UIPanelLayer layer, string name)
        {
            UIPanel ui = default(UIPanel);
            if (loaders == null || LoaderCount == 0)
            {
                Log.E("NO UILoader");
                return ui;
            }
            for (int i = 0; i < loaders.Count; i++)
            {
                var result = loaders[i].Invoke(type, path, name, layer);
                if (result == null) continue;

                ui = result;
                ui = GameObject.Instantiate(ui);
                ui.PanelLayer = layer;
                SetParent(ui);
                ui.PanelName = name;
                var enity=  _enitys.Subscribe(ui);
                (enity.sensor as IUISensor).OnLoad();
                return ui;
            }
            Log.E(string.Format("NO ui Type: {0}  Path: {1}  Layer: {2}  Name: {3}", type, path, layer, name));
            return ui;
        }
        public T Load<T>(string path, string name, UIPanelLayer layer) where T : UIPanel
        {
            return (T)Load(typeof(T), path, layer, name);
        }
        public bool HaveLoadPanel( string panelName)
        {
            return null == _enitys.FindEnity(panelName);
        }


        public Stack<UIPanel> UIStack;
        public Stack<UIPanel> UICache;
        public UIPanel StackTop { get { return UIStack.Peek(); } }
        public int StackCount { get { return UIStack.Count; } }
        public int CacheCount { get { return UICache.Count; } }
        public bool IsInStack(UIPanel panel) { return UIStack.Contains(panel); }
        public bool IsInCache(UIPanel panel) { return UICache.Contains(panel); }
        public bool IsInUse(UIPanel panel) { return IsInCache(panel) || IsInStack(panel); }

        public UIPanel Current { get { return Peek(); } }
        public UIPanel Peek() { return UIStack.Peek(); }
        public UIPanel CachePeek() { return UICache.Peek(); }

        public void Push(UIPanel ui, UIEventArgs arg)
        {
            if (StackCount > 0)
                arg.pressPanel = Current;
            arg.curPanel = ui;

            UIStack.Push(ui);

            if (arg.pressPanel!=null)
            {
                (_enitys.FindEnity(arg.pressPanel).sensor as IUISensor).OnPress(arg);
            }
            (_enitys.FindEnity(arg.curPanel).sensor as IUISensor).OnTop(arg);
            if (UICache.Count > 0) ClearCache();

        }
        public void GoForWard(UIEventArgs arg)
        {
            if (CacheCount <= 0) return;
            if (StackCount > 0)
                arg.pressPanel = Current;

            var ui = UICache.Pop();
            arg.curPanel = ui;
            UIStack.Push(ui);

            if (arg.pressPanel!=null)
                (_enitys.FindEnity(arg.pressPanel).sensor as IUISensor).OnPress(arg);
            (_enitys.FindEnity(arg.curPanel).sensor as IUISensor).OnTop(arg);
        }
        public void GoBack(UIEventArgs arg)
        {
            if (StackCount <= 0) return;

            var ui = UIStack.Pop();
            arg.popPanel = ui;
            UICache.Push(ui);

            if (StackCount > 0)
                arg.curPanel = Current;
            (_enitys.FindEnity(arg.popPanel).sensor as IUISensor).OnPop(arg);

            if (arg.curPanel!=null)
                (_enitys.FindEnity(arg.curPanel).sensor as IUISensor).OnTop(arg);
        }
        public void ClearCache()
        {
            while (UICache.Count != 0)
            {
                UIPanel p = UICache.Pop();
                if (p!=null && !IsInStack(p))
                {
                    var enity = _enitys.FindEnity(p);
                    if (enity !=null)
                    {
                        (enity.sensor as IUISensor).OnClear();
                        _enitys.UnSubscribe(p);
                    }
                }
            }
        }

        public UIPanel Get(Type type, string name, UIEventArgs arg, string path="", UIPanelLayer layer= UIPanelLayer.Common)
        {
            //if (UICache.Count > 0) ClearCache(arg);
            var enity = _enitys.FindEnity(name);
            if (enity ==null)
            {
                UIPanel ui = Load(type, path, layer, name);
                Push(ui, arg);
                return ui;
            }
            else
            {
                Push((enity.enity as UIEnity).panel, arg);
                return (enity.enity as UIEnity).panel;
            }
        }
        public T Get<T>(string name, UIEventArgs arg, string path = "", UIPanelLayer layer = UIPanelLayer.Common) where T : UIPanel
        {
            return (T)Get(typeof(T), name, arg, path, layer);
        }


    }
}
