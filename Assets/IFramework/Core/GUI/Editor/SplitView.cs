/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.90
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool
{
    [Serializable]
    public class SplitView
    {
        private SplitType _splitType = SplitType.Vertical;
        private float _split = 200;
        public event Action<Rect> fistPan, secondPan;
        public event Action onBeginResize;
        public event Action onEndResize;
        public bool dragging
        {
            get { return _resizing; }
            private set
            {
                if (_resizing != value)
                {
                    _resizing = value;
                    if (value)
                    {
                        if (onBeginResize != null)
                        {
                            onBeginResize();
                        }
                    }
                    else
                    {
                        if (onEndResize != null)
                        {
                            onEndResize();
                        }
                    }
                }
            }
        }
        private bool _resizing;
        public void OnGUI(Rect position)
        {
            var rs = position.Split(_splitType, _split, 4);
            var mid = position.SplitRect(_splitType, _split, 4);
            if (fistPan != null)
            {
                fistPan(rs[0]);
            }
            if (secondPan != null)
            {
                secondPan(rs[1]);
            }
            GUI.Box(mid, "");
            Event e = Event.current;
            if (mid.Contains(e.mousePosition))
            {
                if (_splitType == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(mid, MouseCursor.ResizeVertical);
            }
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (mid.Contains(Event.current.mousePosition))
                    {
                        dragging = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (dragging)
                    {
                        switch (_splitType)
                        {
                            case SplitType.Vertical:
                                _split += Event.current.delta.x;
                                break;
                            case SplitType.Horizontal:
                                _split += Event.current.delta.y;
                                break;
                        }
                        _split = Mathf.Clamp(_split, 100, position.width - 100);
                        if (EditorWindow.focusedWindow != null)
                        {
                            EditorWindow.focusedWindow.Repaint();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (dragging)
                    {
                        dragging = false;
                    }
                    break;
            }
        }
    }
}
