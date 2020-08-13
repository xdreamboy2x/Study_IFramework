/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IFramework.GUITool.HorizontalMenuToorbar
{
    class Styles
    {
        public static GUIStyle toolBar = GUIStyles.Get("ToolBar");
        public static GUIStyle toolbarbutton = GUIStyles.Get("toolbarbutton");
        public static GUIStyle tooltip = GUIStyles.Get("Tooltip");
        public static GUIStyle dropDown = GUIStyles.Get("ToolbarDropDown");
    }
    public abstract class ToolbarNode : ILayoutGUIDrawer
    {
        protected GUIContent content;
        protected float width;
        protected Func<bool> canshowFunc;
        public bool canshow
        {
            get
            {
                if (canshowFunc == null)
                {
                    return true;
                }
                return canshowFunc();
            }
        }
        public ToolbarNode(GUIContent content, int width = 100, Func<bool> canshow=null) { this.width = width; this.content = content; this.canshowFunc = canshow; }
        public abstract void OnGUI();
    }

    class ToolBarSpace : ToolbarNode
    {
        public ToolBarSpace(int width=100, Func<bool> canshow=null) : base(null,width,canshow) { }
        public override void OnGUI()
        {
            this.Space(width);
        }
    }
    class ToolBarFlexibleSpace : ToolbarNode
    {
        public ToolBarFlexibleSpace(Func<bool> canshow=null) : base(null, 0,canshow) { }
        public override void OnGUI()
        {
            this.FlexibleSpace();
        }
    }
    class DelegateLabel : ToolbarNode
    {
        public event Action<Rect> panDel;
        public DelegateLabel(Action<Rect> panDel, int width=100, Func<bool> canshow=null) : base(null, width,canshow) { this.panDel = panDel; }

        public override void OnGUI()
        {
            this.Label("", GUILayout.Width(width));
            if (panDel != null)
                panDel(GUILayoutUtility.GetLastRect());
        }
    }
    class ToolBarLabel : ToolbarNode
    {
        public ToolBarLabel(GUIContent content, int width=100, Func<bool> canshow=null) : base(content, width,canshow) { }

        public override void OnGUI()
        {
            this.Label(content, GUILayout.Width(width));
        }
    }
    class ToolBarToolTip : ToolbarNode
    {
        public ToolBarToolTip(GUIContent content, int width=100, Func<bool> canshow=null) : base(content, width,canshow) { }

        public override void OnGUI()
        {
            this.Label(content, Styles.tooltip, GUILayout.Width(width));
        }
    }
    class ToolBarSearchField : ToolbarNode
    {

        private event Action<string> onValueChange;
        public ToolBarSearchField(Action<string> onValueChange, string value, int width=100 ,Func<bool> canshow=null) : base(null, width, canshow) {

            this.onValueChange = onValueChange;
            s = new SearchFieldDrawer("", null, 0);

            s.onValueChange += (str) => {
                if (this.onValueChange != null)
                    this.onValueChange(str);
                value = str;
            };
        }

        private SearchFieldDrawer s;
        public override void OnGUI()
        {
            this.Label("", GUILayout.Width(width));
            s.OnGUI(GUILayoutUtility.GetLastRect());
        }
    }
    class ToolBarButton : ToolbarNode
    {
        private event Action<Rect> onClick;
        public ToolBarButton(GUIContent content, Action<Rect> onClick, int width=100 ,Func<bool> canshow=null) : base(content, width, canshow) { this.onClick = onClick; }

        public override void OnGUI()
        {
            this.Label(string.Empty, GUILayout.Width(width));
            Rect r = GUILayoutUtility.GetLastRect();
            this.GetRectDrawer().Button(() => { if (onClick != null) onClick(r); }, r, content, Styles.toolbarbutton);
        }
    }
    class ToolBarDropDownButton : ToolbarNode
    {
        private event Action<Rect> onClick;
        public ToolBarDropDownButton(GUIContent content, Action<Rect> onClick, int width=100,Func<bool> canshow=null) : base(content, width, canshow) { this.onClick = onClick; }

        public override void OnGUI()
        {
            GUILayout.Label(string.Empty, GUILayout.Width(width));
            Rect r = GUILayoutUtility.GetLastRect();
            this.GetRectDrawer().Button(() => { if (onClick != null) onClick(r); }, r, content, Styles.dropDown);
        }
    }
    class ToolBarToggle : ToolbarNode
    {
        private bool value;
        private event Action<bool> onValueChange;

        public ToolBarToggle(GUIContent content, Action<bool> onValueChange, bool value = false , int width=100 , Func<bool> canshow=null) : base(content, width,canshow)
        {
            this.onValueChange = onValueChange;
            this.value = value;
        }

        public override void OnGUI()
        {

            bool val = value;
            this.Toggle(ref val, content, Styles.toolbarbutton, GUILayout.Width(width));
            if (val != value)
            {
                value = val;
                if (onValueChange != null) onValueChange(value);
            }
        }
    }


    class ToolBarPopup : ToolbarNode
    {
        private int value = 0;
        private string[] ops;
        private Action<int> onValueChange;

        public ToolBarPopup(Action<int> onValueChange,  string[] ops, int value = 0, int width=100, Func<bool> canshow=null) : base(null, width, canshow)
        {
            this.value = value;
            this.ops = ops;
            this.onValueChange = onValueChange;
        }

        public override void OnGUI()
        {
            int tmp = EditorGUILayout.Popup(value, ops, Styles.dropDown, GUILayout.Width(width));
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null)
                {
                    onValueChange(value);
                }
            }
        }
    }
    public class ToolBarTree
    {
        private List<ToolbarNode> Nodes = new List<ToolbarNode>();
        public void OnGUI(Rect position)
        {
            Styles.toolBar.fixedHeight = position.height;
            GUILayout.BeginArea(position);
            GUILayout.BeginHorizontal(Styles.toolBar, GUILayout.Width(position.width));
            Nodes.ForEach((n) =>
            {
                if (n.canshow)
                {
                    n.OnGUI();
                }
            });
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public ToolBarTree DockNode(ToolbarNode node)
        {
            Nodes.Add(node);
            return this;
        }
    }


    public static class ToolBarTreeEx
    {
        public static ToolBarTree Popup(this ToolBarTree t, Action<int> onValueChange, string[] ops, int value = 0, int width = 100, Func<bool> canshow = null)
        {
            ToolBarPopup btn = new ToolBarPopup(onValueChange, ops, value, width,canshow);
           t. DockNode(btn);
            return t;
        }
        public static ToolBarTree Button(this ToolBarTree t, GUIContent content, Action<Rect> onClick, int width = 100, Func<bool> canshow = null)
        {
            ToolBarButton btn = new ToolBarButton(content, onClick, width,canshow);
            t.DockNode(btn);
            return t;
        }
        public static ToolBarTree DropDownButton(this ToolBarTree t, GUIContent content, Action<Rect> onClick, int width = 100, Func<bool> canshow = null)
        {
            ToolBarDropDownButton btn = new ToolBarDropDownButton(content, onClick, width,canshow);
            t.DockNode(btn);
            return t;
        }
        public static ToolBarTree Toggle(this ToolBarTree t, GUIContent content, Action<bool> onValueChange, bool value = false, int width = 100, Func<bool> canshow = null)
        {
            ToolBarToggle tog = new ToolBarToggle(content, onValueChange, value, width,canshow);
            t.DockNode(tog);
            return t;
        }
        public static ToolBarTree SearchField(this ToolBarTree t, Action<string> onValueChange, string value, int width = 100, Func<bool> canshow = null)
        {
            ToolBarSearchField tog = new ToolBarSearchField(onValueChange, value, width,canshow);
            t.DockNode(tog);
            return t;
        }
        public static ToolBarTree Label(this ToolBarTree t, GUIContent content, int width = 100, Func<bool> canshow = null)
        {
            ToolBarLabel la = new ToolBarLabel(content, width,canshow);
            t.DockNode(la);
            return t;
        }
        public static ToolBarTree ToolTip(this ToolBarTree t, GUIContent content, int width = 100, Func<bool> canshow = null)
        {
            ToolBarToolTip la = new ToolBarToolTip(content, width,canshow);
            t.DockNode(la);
            return t;
        }
        public static ToolBarTree Space(this ToolBarTree t, int width = 100, Func<bool> canshow = null)
        {
            ToolBarSpace sp = new ToolBarSpace(width,canshow);
            t.DockNode(sp);
            return t;
        }
        public static ToolBarTree Delegate(this ToolBarTree t, Action<Rect> panDel, int width = 100, Func<bool> canshow = null)
        {
            DelegateLabel sp = new DelegateLabel(panDel, width,canshow);
            t.DockNode(sp);
            return t;
        }

        public static ToolBarTree FlexibleSpace(this ToolBarTree t, Func<bool> canshow = null)
        {
            ToolBarFlexibleSpace sp = new ToolBarFlexibleSpace(canshow);
            t.DockNode(sp);
            return t;
        }

    }
}
