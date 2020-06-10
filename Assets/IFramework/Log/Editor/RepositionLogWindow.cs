/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-17
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.IO;
using UnityEngine;
using IFramework.GUITool;

namespace IFramework
{
    [EditorWindowCacheAttribute("IFramework.Log")]
     partial class RepositionLogWindow : EditorWindow,IRectGUIDrawer,ILayoutGUIDrawer
    {

        private const string Preview = "Preview";
        private const string Name = "Name";
        private const string Path = "Path";

        private const string TitleStyle = "IN BigTitle";
        private const string EntryBackodd = "CN EntryBackodd";
        private const string EntryBackEven = "CN EntryBackEven";

    }

     partial class RepositionLogWindow : EditorWindow
    {
        private LogSetting info;
        private string infoPath;
        private Vector2 ScrollPos;
        private ListViewCalculator.ColumnSetting[] ViewSetting
        {
            get
            {
                return new ListViewCalculator.ColumnSetting[3]
                {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Preview,
                        width=100,
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Name,
                        width=200,
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name = Path,
                        width = 600
                    }
                };

            }
        }

        private void OnEnable()
        {
            LoadInfo();

        }
        public void LoadInfo()
        {
            infoPath = EditorEnv.frameworkPath.
                CombinePath("Log/Resources/" + RepositionLog.StoName + ".asset");
            if (!File.Exists(infoPath)) ScriptableObj.Create<LogSetting>(infoPath);
            info = ScriptableObj.Load<LogSetting>(infoPath);
            for (int i = 0; i < info.Infos.Count; i++)
            {
                LogEliminateItem item = info.Infos[i];
                if (item.text == null)
                {
                    if (string.IsNullOrEmpty(item.path))
                    {
                        ShowNotification(new GUIContent("Null Err"));
                    }
                    else
                    {
                        TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(item.path);
                        if (txt == null)
                        {
                            ShowNotification(new GUIContent("Not Found Err"));
                        }
                        else
                        {
                            item.text = txt;
                            item.name = item.path.GetFileName();
                        }
                    }
                }
                else
                {
                    string path = AssetDatabase.GetAssetPath(item.text);
                    item.path = path;
                    item.name = path.GetFileName();
                }
            }
            EditorUtility.ClearProgressBar();
        }


        private void OnDisable()
        {
            ScriptableObj.Update(info);
        }


        private TableViewCalculator table = new TableViewCalculator();
        public void ListView(Rect rect)
        {
            float LineHeight = 20;
            table.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ScrollPos, LineHeight, info.Infos.Count, ViewSetting);
            if (Event.current.type == EventType.Repaint)
                new GUIStyle(EntryBackodd).Draw(table.position, false, false, false, false);

            this.LabelField(table.titleRow.position, "",new GUIStyle(TitleStyle))
                .LabelField(table.titleRow[Preview].position, Preview)
                .LabelField(table.titleRow[Name].position, Name)
                .LabelField(table.titleRow[Path].position, Path)
                .DrawScrollView(() =>
                    {
                        for (int i = table.firstVisibleRow; i < table.lastVisibleRow + 1; i++)
                        {

                            GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                            if (Event.current.type == EventType.Repaint)
                                style.Draw(table.rows[i].position, false, false, table.rows[i].selected, false);
                            if (Event.current.modifiers == EventModifiers.Control &&
                                    Event.current.button == 0 && Event.current.clickCount == 1 &&
                                    table.rows[i].position.Contains(Event.current.mousePosition))
                            {
                                table.ControlSelectRow(i);
                                Repaint();
                            }
                            else if (Event.current.modifiers == EventModifiers.Shift &&
                                            Event.current.button == 0 && Event.current.clickCount == 1 &&
                                            table.rows[i].position.Contains(Event.current.mousePosition))
                            {
                                table.ShiftSelectRow(i);

                                Repaint();
                            }
                            else if (Event.current.button == 0 && Event.current.clickCount == 1 &&
                                            table.rows[i].position.Contains(Event.current.mousePosition))
                            {
                                table.SelectRow(i);
                                Repaint();
                            }
                            Texture2D tx = AssetPreview.GetMiniThumbnail(info.Infos[i].text);
                            this.Label(table.rows[i][Preview].position, new GUIContent(tx));
                            this.Label(table.rows[i][Name].position, info.Infos[i].name);
                            this.Label(table.rows[i][Path].position, info.Infos[i].path);
                        }
                    }, 
                    table.view,ref ScrollPos,table.content, false, false
            );



            Handles.color = Color.black;
            for (int i = 0; i < table.titleRow.columns.Count; i++)
            {
                var item = table.titleRow.columns[i];

                if (i != 0)
                    Handles.DrawAAPolyLine(1, new Vector3(item.position.x,
                                                            item.position.y,
                                                            0),
                                              new Vector3(item.position.x,
                                                            item.position.y + item.position.height - 2,
                                                            0));
            }
            table.position.DrawOutLine(2, Color.black);


            Handles.color = Color.white;
        }

        private void OnGUI()
        {

            GUIStyle title = new GUIStyle("IN BigTitle");
            title.fontSize = 20;
            this.Label(new Rect(10, 5, position.width - 20, 35), "Log Ignore List", title);
            ListView(new Rect(10, 40, position.width - 20, position.height - 200));
            Eve();
            this.DrawArea(() => {
                this.DrawHorizontal(() =>
                {
                    this.Toggle("Enable",ref info.enable);
                    using (new EditorGUI.DisabledScope(!info.enable))
                    {
                        this.Toggle("LogEnable",ref info.enable_L);
                        this.Toggle("WarnningEnable",ref info.enable_W);
                        this.Toggle("ErrEnable",ref info.enable_E);
                    }
                });
                using (new EditorGUI.DisabledScope(!info.enable))
                {
                    this.IntSlider("LogLevel ",ref info.lev_L, 0, 100);
                    this.IntSlider("WarnningLevel ",ref info.lev_W, 0, 100);
                    this.IntSlider("ErrLevel ",ref info.lev_E, 0, 100);
                }


            }, new Rect(10, position.height - 160, position.width - 20, 180));
        }


        private void Eve()
        {
            Event eve = Event.current;
            if (eve.button == 0 && eve.clickCount == 1 &&
                    (!table.view.Contains(eve.mousePosition) ||
                        (table.view.Contains(eve.mousePosition) &&
                         !table.content.Contains(eve.mousePosition))))
            {
                table.SelectNone();
                Repaint();
            }
            DragAndDropIInfo dragInfo = DragAndDropUtil.Drag(eve, table.view);
            if (table.view.Contains(eve.mousePosition) && dragInfo.enterArera && dragInfo.compelete)
            {
                for (int i = 0; i < dragInfo.paths.Length; i++)
                {
                    float progress = (float)i / dragInfo.paths.Length;
                    EditorUtility.DisplayProgressBar(string.Format("Add Script {0}/{1}",i, dragInfo.paths.Length), dragInfo.paths[i], progress);
                    SaveInfo(dragInfo.paths[i]);
                }
                ScriptableObj.Update(info);
                EditorUtility.ClearProgressBar();
            }

            if (eve.button == 1 && eve.clickCount == 1 &&
                        table.content.Contains(eve.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, () => {

                    for (int i = table.welectedRows.Count - 1; i >= 0; i--)
                    {
                        float progress = (float)i / table.welectedRows.Count;
                        EditorUtility.DisplayProgressBar(string.Format("Delete Script {0}/{1}", i, table.welectedRows.Count),"", progress);
                        this.info.Infos.RemoveAt(table.welectedRows[i].rowID);
                    }
                    EditorUtility.ClearProgressBar();
                    ScriptableObj.Update(info);
                    table.SelectNone();
                });
                menu.AddItem(new GUIContent("Select Script"), false, () =>
                {
                    for (int i = table.rows.Count - 1; i >= 0; i--)
                    {
                        if (table.rows[i].position.Contains(eve.mousePosition))
                        {
                            Selection.activeObject = this.info.Infos[i].text;
                            break;
                        }
                    }
                });
                menu.ShowAsContext();
                if (eve.type != EventType.Layout)
                    eve.Use();
            }
        }

        public void SaveInfo(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            TextAsset txt = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (txt == null) return;
            for (int i = 0; i < info.Infos.Count; i++)
            {
                if (info.Infos[i].path == path) return;
            }
            info.Infos.Add(new LogEliminateItem()
            {
                path = path,
                name = path.GetFileName(),
                text = txt
            });
        }
    }
}

