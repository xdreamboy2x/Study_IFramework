/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.Linq;
using UnityEngine;
using IFramework.GUITool;

namespace IFramework
{
    partial class EditorWindowCollection : EditorWindow, IRectGUIDrawer, ILayoutGUIDrawer
    {
        [MenuItem("IFramework/EditorWindowCollection")]
        static void ShowWindow()
        {
            GetWindow<EditorWindowCollection>();
        }
        private class Styles
        {
            public static GUIStyle titlestyle = GUIStyles.Get("IN BigTitle");
            public static GUIStyle entryBackodd = GUIStyles.Get("CN EntryBackodd");
            public static GUIStyle entryBackEven = GUIStyles.Get("CN EntryBackEven");
        }
        private class Contents
        {
            public const string name = "Name";
            public const string dock = "Dock";
            public const string get = "Get";
            public const string close = "Close";
            public const float lineHeight = 20f;
            public static Texture tx = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
        }
        private string _search = "";
        private SearchFieldDrawer _sear;
        private TableViewCalculator _table = new TableViewCalculator();
        private Vector2 _scroll;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private ListViewCalculator.ColumnSetting[] setting = new ListViewCalculator.ColumnSetting[]
        {
            new ListViewCalculator.ColumnSetting()
            {
                name=Contents.name,
                width=200
            },
        };
    }
    partial class EditorWindowCollection 
    {
        private void OnEnable()
        {
            _sear = new SearchFieldDrawer() { value = "", };
            _sear.onValueChange += (str) => { _search = str; };
        }
        private void OnGUI()
        {
            var rs = localPosition.HorizontalSplit(position.height - 2 * Contents.lineHeight);

            var fitterWindows = EditorWindowUtil.windows.FindAll((w) => { return w.searchName.ToLower().Contains(_search); }).ToArray();
            _table.Calc(rs[0], new Vector2(0, Contents.lineHeight), _scroll, Contents.lineHeight, fitterWindows.Length, setting);

            Event e = Event.current;

            this.LabelField(_table.titleRow.position, "", Styles.titlestyle)
                .Pan(() => {
                    _sear.OnGUI(_table.titleRow[Contents.name].localPostion);
                })
                .DrawScrollView(() =>
                {
                    for (int i = _table.firstVisibleRow; i < _table.lastVisibleRow + 1; i++)
                    {
                        int index = i;
                        EditorWindowUtil.EditorWindowItem window = fitterWindows[i];
                        if (e.type == EventType.Repaint)
                        {
                            GUIStyle style = index % 2 == 0 ? Styles.entryBackEven : Styles.entryBackodd;
                            style.Draw(_table.rows[index].position, false, false, _table.rows[i].selected, false);
                        }
                        if (e.button == 0 && e.clickCount == 1 && _table.rows[index].position.Contains(e.mousePosition))
                        {
                            _table.SelectRow(index);
                            Repaint();
                        }
                        if (window.type.Namespace.Contains("UnityEditor"))
                            this.Label(_table.rows[index][Contents.name].position, new GUIContent(window.searchName, Contents.tx));
                        else
                            this.Label(_table.rows[index][Contents.name].position, window.searchName);
                    }
                }, _table.view, ref _scroll, _table.content, false, false);


            Handles.color = Color.black;
            for (int i = 0; i < _table.titleRow.columns.Count; i++)
            {
                var item = _table.titleRow.columns[i];

                if (i != 0)
                    Handles.DrawAAPolyLine(1, new Vector3(item.position.x,
                                                            item.position.y,
                                                            0),
                                              new Vector3(item.position.x,
                                                            item.position.y + item.position.height - 2,
                                                            0));
            }
            _table.position.DrawOutLine(2, Color.black);
            Handles.color = Color.white;

            string windowName = "";
            for (int j = _table.selectedRows.Count - 1; j >= 0; j--)
            {
                if (_table.selectedRows[j].selected)
                {
                    windowName = fitterWindows[j].searchName;
                    break;
                }
            }
            using (new EditorGUI.DisabledGroupScope(_table.selectedRows.Count < 0))
            {
                this.BeginArea(rs[1])
                        .FlexibleSpace()
                        .BeginHorizontal()
                            .FlexibleSpace()
                                .Button(() =>
                                {
                                    var w = EditorWindowUtil.FindOrCreate(windowName);
                                    if (w != null)
                                    {
                                        w.Focus();
                                    }
                                }, Contents.get)
                                .Button(() =>
                                {
                                    var w = EditorWindowUtil.FindOrCreate(windowName);
                                    if (w != null)
                                    {
                                        this.DockWindow(w, EditorWindowUtil.DockPosition.Right);
                                        w.Focus();
                                    }
                                }, Contents.dock)
                                .Button(() =>
                                {

                                    EditorWindowUtil.FindAll(windowName).ToList().ForEach((w) =>
                                    {
                                        w.Close();
                                    });
                                }, Contents.close)
                         .EndHorizontal()
                    .FlexibleSpace()
                .EndArea();
            }

        }

    }
}
