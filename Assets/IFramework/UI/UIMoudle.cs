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
using IFramework.Modules.MVVM;
using XLua;

namespace IFramework.UI
{
    public interface IGroups:IDisposable
    {
        UIPanel FindPanel(string name);
        void InvokeUIModuleEventListenner(UIEventArgs arg);
        void Subscribe(UIPanel panel);
        void UnSubscribe(UIPanel panel);
    }
    public class Groups:IGroups
    {
        private MVVMModule _moudule;
        private Dictionary<Type, Tuple<Type, Type, Type>> _map;

        private MVVMGroup FindGroup(string panelName)
        {
            return _moudule.FindGroup(panelName);
        }
        private MVVMGroup FindGroup(UIPanel panel)
        {
            return FindGroup(panel.PanelName);
        }



        public Groups(Dictionary<Type, Tuple<Type, Type, Type>> map)
        {
            _moudule = MVVMModule.CreatInstance<MVVMModule>("UIGroup");
            this._map = map;
        }
        public UIPanel FindPanel(string panelName)
        {
            var group = FindGroup(panelName);
            if (group == null) return null;
                return (group.view as UIView).panel;
        }
        public void InvokeUIModuleEventListenner(UIEventArgs arg)
        {
            if (arg.pressPanel != null)
                (FindGroup(arg.pressPanel).view as IUIModuleEventListenner).OnPress(arg);
            if (arg.popPanel != null)
                (FindGroup(arg.popPanel).view as IUIModuleEventListenner).OnPop(arg);
            if (arg.curPanel != null)
                (FindGroup(arg.curPanel).view as IUIModuleEventListenner).OnTop(arg);
        }
        public void Subscribe(UIPanel panel)
        {
            var _group = FindGroup(panel);
            if (_group != null) throw new Exception(string.Format("Have Subscribe Panel Name: {0}", panel.PanelName));

            Tuple<Type, Type, Type> tuple;
            _map.TryGetValue(panel.GetType(), out tuple);
            if (tuple == null) throw new Exception(string.Format("Could Not Find map with Type: {0}", panel.GetType()));


            var model = Activator.CreateInstance(tuple.Item1) as IDataModel;
            var view = Activator.CreateInstance(tuple.Item2) as UIView;
            var vm = Activator.CreateInstance(tuple.Item3) as UIViewModel;
            view.panel = panel;

            UIGroup group = new UIGroup(panel.name, view, vm, model);
            _moudule.AddGroup(group);
            (view as IUIModuleEventListenner).OnLoad();
        }
        public void UnSubscribe(UIPanel panel)
        {
            var group = FindGroup(panel);
            if (group != null)
            {
                (group.view as IUIModuleEventListenner).OnClear();
                group.Dispose();
            }
        }
        public void Dispose()
        {
            _moudule.Dispose();
        }
    }
    public partial class UIModule
    {
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
        private void InitTransform()
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
    }
    public partial class UIModule
    {
        private List<LoadDel> loaders;
        public int LoaderCount { get { return loaders.Count; } }

        public Stack<UIPanel> UIStack;
        public Stack<UIPanel> UICache;
        public int StackCount { get { return UIStack.Count; } }
        public int CacheCount { get { return UICache.Count; } }
        public bool IsInStack(UIPanel panel) { return UIStack.Contains(panel); }
        public bool IsInCache(UIPanel panel) { return UICache.Contains(panel); }
        public bool IsInUse(UIPanel panel) { return IsInCache(panel) || IsInStack(panel); }
        public UIPanel Current
        {
            get
            {
                if (UIStack.Count == 0)
                    return null;
                return UIStack.Peek();
            }
        }
        public UIPanel CachePeek()
        {
            if (UICache.Count == 0)
                return null;
            return UICache.Peek();
        }
    }
    public delegate UIPanel LoadDel(Type type, string path, string name, UIPanelLayer layer);
    public partial class UIModule : FrameworkModule
    {

        private IGroups _groups;
        protected override void Awake()
        {
            InitTransform();

            UIStack = new Stack<UIPanel>();
            UICache = new Stack<UIPanel>();
            loaders = new List<LoadDel>();

        }
        protected override void OnDispose()
        {

            UIStack.Clear();
            UICache.Clear();
            loaders.Clear();

            if (_groups != null)
                _groups.Dispose();
            if (Canvas != null)
                GameObject.Destroy(Canvas.gameObject);
        }

   

        public void AddLoader(LoadDel loader)
        {
            loaders.Add(loader);
        }

        public void SetGroups(IGroups groups)
        {
            this._groups = groups;
            Debug.Log(groups);
        }


        public UIPanel Load(Type type, string path, UIPanelLayer layer, string name)
        {
            if (_groups == null)
                throw new Exception("Please Set ModuleType First");
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
                ui.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                 _groups.Subscribe(ui);

                return ui;
            }
            Log.E(string.Format("NO ui Type: {0}  Path: {1}  Layer: {2}  Name: {3}", type, path, layer, name));
            return ui;
        }
        public T Load<T>(string path, string name, UIPanelLayer layer) where T : UIPanel
        {
            return (T)Load(typeof(T), path, layer, name);
        }
        public bool HaveLoadPanel(string panelName)
        {
            return _groups.FindPanel(panelName)!=null;
        }


        public void Push(UIPanel ui)
        {
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.Push;
            if (StackCount > 0)
                arg.pressPanel = Current;
            arg.curPanel = ui;

            UIStack.Push(ui);
            InvokeUIModuleEventListenner(arg);
            arg.Recyle();
            if (UICache.Count > 0) ClearCache();
        }
        public void GoForWard()
        {
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.GoForward;

            if (CacheCount <= 0) return;
            if (StackCount > 0)
                arg.pressPanel = Current;

            var ui = UICache.Pop();
            arg.curPanel = ui;
            UIStack.Push(ui);
            InvokeUIModuleEventListenner(arg);
            arg.Recyle();
        }
        public void GoBack()
        {
            UIEventArgs arg = UIEventArgs.Allocate<UIEventArgs>(this.container.env.envType);
            arg.code = UIEventArgs.Code.GoBack;
            if (StackCount <= 0) return;

            var ui = UIStack.Pop();
            arg.popPanel = ui;
            UICache.Push(ui);

            if (StackCount > 0)
                arg.curPanel = Current;
            InvokeUIModuleEventListenner(arg);
            arg.Recyle();

        }
        private void InvokeUIModuleEventListenner(UIEventArgs arg)
        {
            _groups.InvokeUIModuleEventListenner(arg);
        }


        public void ClearCache()
        {
            while (UICache.Count != 0)
            {
                UIPanel p = UICache.Pop();
                if (p != null && !IsInStack(p))
                {
                    _groups.UnSubscribe(p);
                }
            }
        }


        public UIPanel Get(Type type, string name, string path = "", UIPanelLayer layer = UIPanelLayer.Common)
        {
            //if (UICache.Count > 0) ClearCache(arg);

            if (Current != null && Current.PanelName == name && Current.GetType() == type)
                return Current;
            var panel = _groups.FindPanel(name);
            if (panel == null)
                panel = Load(type, path, layer, name);
            Push(panel);
            return panel;
        }
        public T Get<T>(string name, string path = "", UIPanelLayer layer = UIPanelLayer.Common) where T : UIPanel
        {
            return (T)Get(typeof(T), name, path, layer);
        }
    }
}
