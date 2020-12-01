/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using IFramework.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using IFramework.Serialization;
using IFramework.GUITool.HorizontalMenuToorbar;

namespace IFramework.Hotfix.AB
{
    [EditorWindowCache("IFramework.AssetBundle")]
    partial class AssetBundleWindow : EditorWindow,ILayoutGUIDrawer
    {
        private abstract class GUIBase
        {
            protected Rect position { get; private set; }
            public virtual void OnGUI(Rect position)
            {
                this.position = position;
            }
            public virtual void OnDisable() { }
        }
        private class ToolGUI : GUIBase,ILayoutGUIDrawer
        {
            private ABBuiidInfo ABBuiidInfo { get { return _window.Info.ABBuiidInfo; } }

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                this.BeginArea(position)
                        .Label("BuildSetting")
                        .Label("", GUIStyles.Get("IN Title"), GUILayout.Height(5))
                        .Label("Build  Target:")
                        .Label(EditorUserBuildSettings.activeBuildTarget.ToString())
                        .Label("", GUIStyles.Get("IN Title"), GUILayout.Height(5))
                        .Label("AssetBundle OutPath:")
                        .Label("Assets/../AssetBundles")
                        .Label("", GUIStyles.Get("IN Title"), GUILayout.Height(5))
                        .Label("Manifest FilePath:")
                        .Label(ABTool.configPath)
                        .Label("", GUIStyles.Get("IN Title"), GUILayout.Height(5))
                        .Space(10)
                        .Label("LoadSetting In Editor")
                        .Pan(()=> {
                            ABTool.testmode = EditorGUILayout.Toggle(new GUIContent("AssetDataBase Load"), ABTool.testmode);
                        })
                        .Space(10)
                        .Button(() => {
                            Build.DeleteBundleFiles();
                        }, "Clear Bundle Files")
                        .Button(() => {
                            Build.BuildManifest(ABTool.configPath, ABBuiidInfo.GetAssetBundleBuilds());
                        }, "Build Manifest")
                        .Button(() => {
                            Build.BuildManifest(ABTool.configPath, ABBuiidInfo.GetAssetBundleBuilds());
                            Build.BuildAssetBundles(ABBuiidInfo.GetAssetBundleBuilds(), EditorUserBuildSettings.activeBuildTarget);
                            EditorTools.OpenFloder(ABTool.assetsDir);
                        }, "Build AssetBundle")
                        .Button(() => {
                            Build.CopyBundleFilesTo(Application.streamingAssetsPath);
                        }, "copy to Stream")
                    .EndArea();
            }
        }
        private class DirCollectGUI : GUIBase, ILayoutGUIDrawer, IRectGUIDrawer
        {
            public class AssetChooseWindow : PopupWindowContent, IRectGUIDrawer, ILayoutGUIDrawer
            {
                public ABDirCollectItem.ABSubFile assetinfo;
                public override void OnGUI(Rect rect)
                {
                    if (assetinfo == null) return;
                    this.DrawScrollView(() => {
                        Draw(assetinfo, 0);
                    }, ref scroll);
                }
                private Vector2 scroll;

                private void Draw(ABDirCollectItem.ABSubFile assetinfo, float offset)
                {
                    this.DrawHorizontal(() =>
                    {
                        this.Space(offset);
                        if (assetinfo.fileType == ABDirCollectItem.ABSubFile.FileType.InValidFile)
                            GUI.enabled = false;
                        bool s = assetinfo.Selected;
                        this.Toggle(ref s, new GUIContent(assetinfo.ThumbNail), GUILayout.Height(16), GUILayout.Width(40));
                        assetinfo.Selected = s;
                        if (assetinfo.SubFiles.Count > 0)
                            this.Foldout(ref assetinfo.isOpen, assetinfo.name);
                        else
                            this.Label(assetinfo.name);
                        GUI.enabled = true;
                    });

                    if (assetinfo.isOpen)
                        for (int i = 0; i < assetinfo.SubFiles.Count; i++)
                            Draw(assetinfo.SubFiles[i], offset + 20);
                }
            }

            private const string CollectType = "CollectType";
            private const string BundleName = "BundleName";
            private const string SearchPath = "SearchPath";
            private const string SelectButton = "Set";
            private const string TitleStyle = "IN BigTitle";
            private const string EntryBackodd = "CN EntryBackodd";
            private const string EntryBackEven = "CN EntryBackEven";
            private const float lineHeight = 20;

            private Vector2 ScrollPos;
            private AssetChooseWindow chosseWindow=new AssetChooseWindow();
            private TableViewCalculator tableViewCalc=new TableViewCalculator();
            private ABDirCollect DirCollect { get { return _window.Info.DirCollect; } }
            private ListViewCalculator.ColumnSetting[] Setting
            {
                get
                {

                    return new ListViewCalculator.ColumnSetting[]
                        {
                        new ListViewCalculator.ColumnSetting()
                        {
                            name = BundleName,
                            width = 100
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            name=CollectType,
                            width=80,
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            name=SelectButton,
                            width=50,
                            offSetY=-4,
                            offsetX=-10

                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            name=SearchPath,
                            width=200
                        },
                        };
                }
            }

            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                Event e = Event.current;
                ListView(e);
                Eve(e);
            }
            private void ListView(Event e)
            {
                tableViewCalc.Calc(position, new Vector2(position.x, position.y + lineHeight), ScrollPos, lineHeight, DirCollect.DirCollectItems.Count, Setting);
                if (Event.current.type == EventType.Repaint)
                    GUIStyles.Get(EntryBackodd).Draw(tableViewCalc.position, false, false, false, false);

                bool tog = true;
                this.Toggle(tableViewCalc.titleRow.position, ref tog, GUIStyles.Get(TitleStyle))
                    .LabelField(tableViewCalc.titleRow[CollectType].position, CollectType)
                    .LabelField(tableViewCalc.titleRow[BundleName].position, BundleName)
                    .LabelField(tableViewCalc.titleRow[SearchPath].position, SearchPath);
                this.DrawScrollView(() =>
                {
                    for (int i = tableViewCalc.firstVisibleRow; i < tableViewCalc.lastVisibleRow + 1; i++)
                    {
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (e.type == EventType.Repaint)
                            style.Draw(tableViewCalc.rows[i].position, false, false, tableViewCalc.rows[i].selected, false);
                        if (e.modifiers == EventModifiers.Control &&
                                e.button == 0 && e.clickCount == 1 &&
                                tableViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            tableViewCalc.ControlSelectRow(i);
                            _window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 && e.clickCount == 1 &&
                                        tableViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            tableViewCalc.ShiftSelectRow(i);
                            _window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        tableViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            tableViewCalc.SelectRow(i);
                            _window.Repaint();
                        }

                        ABDirCollectItem item = DirCollect.DirCollectItems[i];

                        int index = (int)item.CollectType;
                        this.Popup(tableViewCalc.rows[i][CollectType].position,
                                    ref index,
                                    Enum.GetNames(typeof(ABDirCollectItem.ABDirCollectType)));
                        item.CollectType = (ABDirCollectItem.ABDirCollectType)index;
                        this.Button(() =>
                        {
                            chosseWindow.assetinfo = item.subAsset;
                            PopupWindow.Show(tableViewCalc.rows[i][SelectButton].position, chosseWindow);
                        }
                        , tableViewCalc.rows[i][SelectButton].position, SelectButton);

                        this.Label(tableViewCalc.rows[i][SearchPath].position, item.SearchPath);
                        if (item.CollectType == ABDirCollectItem.ABDirCollectType.ABName)
                            this.TextField(tableViewCalc.rows[i][BundleName].position, ref item.BundleName);
                    }
                }, tableViewCalc.view,ref ScrollPos,tableViewCalc.content, false, false);

                Handles.color = Color.black;
                for (int i = 0; i < tableViewCalc.titleRow.columns.Count; i++)
                {
                    var item = tableViewCalc.titleRow.columns[i];

                    if (i != 0)
                        Handles.DrawAAPolyLine(1, new Vector3(item.position.x - 2,
                                                                item.position.y,
                                                                0),
                                                  new Vector3(item.position.x - 2,
                                                                item.position.yMax - 2,
                                                                0));

                }
                tableViewCalc.position.DrawOutLine(2, Color.black);
                Handles.color = Color.white;
            }
            private void Eve(Event e)
            {
                if (e.button == 0 && e.clickCount == 1 &&
                        (!tableViewCalc.view.Contains(e.mousePosition) ||
                            (tableViewCalc.view.Contains(e.mousePosition) &&
                             !tableViewCalc.content.Contains(e.mousePosition))))
                {
                    tableViewCalc.SelectNone();
                    _window.Repaint();
                }
                var info = EditorTools.DragAndDropTool.Drag(e, tableViewCalc.view);
                if (info.enterArera && info.compelete)
                {
                    for (int i = 0; i < info.paths.Length; i++)
                    {
                        AddCollectItem(info.paths[i]);
                    }
                }
                if (e.button == 1 && e.clickCount == 1 &&
                          tableViewCalc.content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        for (int i = tableViewCalc.rows.Count - 1; i >= 0; i--)
                        {
                            if (tableViewCalc.rows[i].selected)
                                DirCollect.RemoveCollectItem(DirCollect.DirCollectItems[i]);
                        }
                        _window.UpdateInfo();
                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();

                }
            }

            private void AddCollectItem(string path)
            {
                if (string.IsNullOrEmpty(path) || !path.Contains("Assets")) return;
                if (!Directory.Exists(path)) return;
                DirCollect.AddCollectItem(path);
                _window.UpdateInfo();
            }
        }
        private class AssetBundleBulidGUI : GUIBase, IRectGUIDrawer, ILayoutGUIDrawer
        {
            private const string ABBWin = "AssetBundleBuild";
            private const string ABBItemWin = "ABBItemWin";
            private const string ABBContentWin = "ABBContent";
            private const string ABBContentItemWin = "ABBContentItem";
            private const float LineHeight = 20;

            public AssetBundleBuild_Class ChossedABB;
            public ABDeprndence ChoosedAsset;
            private List<AssetBundleBuild_Class> AssetbundleBuilds { get { return _window.Info.ABBuiidInfo.AssetbundleBuilds; } }

            private const string ABName = "ABName";
            private const string RefCount = "RefCount";
            private const string TitleStyle = "IN BigTitle";
            private const string EntryBackodd = "CN EntryBackodd";
            private const string EntryBackEven = "CN EntryBackEven";
            private ListViewCalculator ABBListViewCalc;
            private Vector2 ABBScrollPos;
            private ListViewCalculator.ColumnSetting[] ABBSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[]
                    {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=ABName,
                        width=300
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=RefCount,
                        width=40
                    }
                    };
                }
            }

            private Vector2 ABBContentScrollPos;
            private TableViewCalculator ABBContentTable = new TableViewCalculator();
            private List<ABDeprndence> dpInfo { get { return _window.Info.ABBuiidInfo.Dependences; } }

            private const string Preview = "Preview";
            private const string AssetName = "AssetName";
            private const string Bundle = "Bundle";
            private const string Size = "Size";
            private const string CrossRef = "CrossRef";
            private ListViewCalculator.ColumnSetting[] ABBContentSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[] {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Preview,
                        width=40
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=AssetName,
                        width=320
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Bundle,
                        width=100
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Size,
                        width=100
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=CrossRef,
                        width=40
                    },

                };

                }
            }

            private SplitView big = new SplitView();
            private SplitView left = new SplitView() {splitType= SplitType.Horizontal };
            private SplitView right = new SplitView() { splitType = SplitType.Horizontal };

            private ListViewCalculator.ColumnSetting[] ABBContentItemSetting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[] {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=Preview,
                        width=40
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=AssetName,
                        width=400
                    },
                };
                }
            }
            private Vector2 ABBContentItemScrollPos;
            private TableViewCalculator ABBContentItemTableCalc = new TableViewCalculator();


            public AssetBundleBulidGUI()
            {
                big.fistPan += left.OnGUI;
                big.secondPan += right.OnGUI;
                left.fistPan += ABBWinGUI;
                left.secondPan += ABBItemWinGUI ;
                right.fistPan += ABBContentWinGUI;
                right.secondPan += ABBContentItemWinGUI;

                ABBListViewCalc = new ListViewCalculator();
            }

            private void ABBWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                Event e = Event.current;
                ABBListViewCalc.Calc(rect, rect.position, ABBScrollPos, LineHeight, AssetbundleBuilds.Count, ABBSetting);
                this.DrawScrollView(() =>
                {
                    for (int i = ABBListViewCalc.firstVisibleRow; i < ABBListViewCalc.lastVisibleRow + 1; i++)
                    {
                        if (e.modifiers == EventModifiers.Control &&
                                e.button == 0 && e.clickCount == 1 &&
                                ABBListViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            ABBListViewCalc.ControlSelectRow(i);
                            _window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 && e.clickCount == 1 &&
                                        ABBListViewCalc.rows[i].position.Contains(e.mousePosition))
                        {
                            ABBListViewCalc.ShiftSelectRow(i);
                            _window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        ABBListViewCalc.rows[i].position.Contains(e.mousePosition)
                                      /*  && ListView.viewPosition.Contains(Event.current.mousePosition) */)
                        {
                            ABBListViewCalc.SelectRow(i);
                            ChossedABB = AssetbundleBuilds[i];
                            _window.Repaint();
                        }

                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (e.type == EventType.Repaint)
                            style.Draw(ABBListViewCalc.rows[i].position, false, false, ABBListViewCalc.rows[i].selected, false);
                        this.Label(ABBListViewCalc.rows[i][ABName].position, AssetbundleBuilds[i].assetBundleName);
                        if (AssetbundleBuilds[i].CrossRefence)
                            this.Label(ABBListViewCalc.rows[i][RefCount].position, EditorGUIUtility.IconContent("console.warnicon.sml"));
                        else
                            this.Label(ABBListViewCalc.rows[i][RefCount].position, EditorGUIUtility.IconContent("Collab"));

                    }
                }, ABBListViewCalc.view,
                ref ABBScrollPos,
                ABBListViewCalc.content, false, false);

                if (e.button == 0 && e.clickCount == 1 &&
                            (ABBListViewCalc.view.Contains(e.mousePosition) &&
                             !ABBListViewCalc.content.Contains(e.mousePosition)))
                {
                    ABBListViewCalc.SelectNone();
                    _window.Repaint();
                    ChoosedAsset = null;
                    ChossedABB = null;
                }

                if (e.button == 1 && e.clickCount == 1 &&
                  ABBListViewCalc.content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {

                        ChoosedAsset = null;
                        ChossedABB = null;
                        ABBListViewCalc.selectedRows.ReverseForEach((row) => {

                            int index = row.rowID;
                            _window.DeleteBundle(AssetbundleBuilds[index].assetBundleName);
                        });


                        _window.UpdateInfo();

                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();

                }
            }
            private void ABBItemWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                if (ChossedABB == null) return;
                this.BeginArea(rect)
                        .Label(ChossedABB.assetBundleName)
                        .Label(ChossedABB.Size)
                        .Pan(() => {
                            if (ChossedABB.CrossRefence)
                                this.Label(EditorGUIUtility.IconContent("console.warnicon.sml"));
                            else
                                this.Label(EditorGUIUtility.IconContent("Collab"));
                        })
                    .EndArea();
            }



            private ABDeprndence GetDpByName(string AssetPath)
            {
                for (int i = 0; i < dpInfo.Count; i++)
                {
                    if (dpInfo[i].AssetPath == AssetPath) return dpInfo[i];
                }
                return default(ABDeprndence);
            }
            private void ABBContentWinGUI(Rect rect)
            {
                GUI.BeginClip(rect);
                rect = new Rect(Vector2.zero, rect.size);
                rect.DrawOutLine(2, Color.black);
                int lineCount = ChossedABB == null ? 0 : ChossedABB.assetNames.Count;
                ABBContentTable.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ABBContentScrollPos, LineHeight, lineCount, ABBContentSetting);
                    this.Label(ABBContentTable.titleRow.position, "", TitleStyle)
                        .Label(ABBContentTable.titleRow[AssetName].position, AssetName)
                        .Label(ABBContentTable.titleRow[Bundle].position, Bundle)
                        .Label(ABBContentTable.titleRow[Size].position, Size);
                Event e = Event.current;
                this.DrawScrollView(() =>
                {
                    for (int i = ABBContentTable.firstVisibleRow; i < ABBContentTable.lastVisibleRow + 1; i++)
                    {
                        ABDeprndence asset = GetDpByName(ChossedABB.assetNames[i]);
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;

                        if (e.type == EventType.Repaint)
                            style.Draw(ABBContentTable.rows[i].position, false, false, ABBContentTable.rows[i].selected, false);

                        this.Label(ABBContentTable.rows[i][Size].position, asset.Size)
                            .Label(ABBContentTable.rows[i][AssetName].position, asset.AssetName)
                            .Label(ABBContentTable.rows[i][Preview].position, asset.ThumbNail)
                            .Label(ABBContentTable.rows[i][Bundle].position, asset.BundleName)
                            .Pan(() => {
                                if (asset.AssetBundles.Count == 1)
                                    this.Label(ABBContentTable.rows[i][CrossRef].position, EditorGUIUtility.IconContent("Collab"));
                                else
                                    this.Label(ABBContentTable.rows[i][CrossRef].position, asset.AssetBundles.Count.ToString(), GUIStyles.Get("CN CountBadge"));
                            });

                        if (e.modifiers == EventModifiers.Control &&
                                e.button == 0 && e.clickCount == 1 &&
                                ABBContentTable.rows[i].position.Contains(Event.current.mousePosition))
                        {
                            ABBContentTable.ControlSelectRow(i);
                            _window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 &&e.clickCount == 1 &&
                                        ABBContentTable.rows[i].position.Contains(e.mousePosition))
                        {
                            ABBContentTable.ShiftSelectRow(i);
                            _window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        ABBContentTable.rows[i].position.Contains(e.mousePosition))
                        {
                            ABBContentTable.SelectRow(i);
                            ChoosedAsset = asset;
                            _window.Repaint();
                        }
                    }

                }, ABBContentTable.view,ref ABBContentScrollPos,ABBContentTable.content, false, false);

                Handles.color = Color.black;

                for (int i = 0; i < ABBContentTable.titleRow.columns.Count; i++)
                {
                    var item = ABBContentTable.titleRow.columns[i];

                    if (i != 0)
                        Handles.DrawAAPolyLine(1, new Vector3(item.position.x - 2,
                                                                item.position.y,
                                                                0),
                                                  new Vector3(item.position.x - 2,
                                                                item.position.yMax - 2,
                                                                0));

                }
                ABBContentTable.position.DrawOutLine(2, Color.black);

                Handles.color = Color.white;

                if (e.button == 0 && e.clickCount == 1 &&
                     (ABBContentTable.view.Contains(e.mousePosition) &&
                      !ABBContentTable.content.Contains(e.mousePosition)))
                {
                    ABBContentTable.SelectNone();
                    ChoosedAsset = null;
                    _window.Repaint();
                }
                if (e.button == 1 && e.clickCount == 1 && ABBContentTable.content.Contains(e.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        ABBContentTable.selectedRows.ReverseForEach((row) =>
                        {
                            _window.RemoveAsset(ChossedABB.assetNames[row.rowID], ChossedABB.assetBundleName);
                        });
                       _window.UpdateInfo();
                        ChoosedAsset = null;
                    });

                    menu.ShowAsContext();
                    if (e.type != EventType.Layout)
                        e.Use();
                }
                GUI.EndClip();
            }
            private void ABBContentItemWinGUI(Rect rect)
            {
                rect.DrawOutLine(2, Color.black);
                if (ChoosedAsset == null) return;
                ABBContentItemTableCalc.Calc(rect, new Vector2(rect.x, rect.y + LineHeight), ABBContentItemScrollPos, LineHeight, ChoosedAsset.AssetBundles.Count, ABBContentItemSetting);
                this.Label(ABBContentItemTableCalc.titleRow[AssetName].position, ChoosedAsset.AssetPath)
                    .Label(ABBContentItemTableCalc.titleRow[Preview].position, ChoosedAsset.ThumbNail);

                this.DrawScrollView(() =>
                {
                    for (int i = 0; i < ABBContentItemTableCalc.rows.Count; i++)
                    {
                        GUIStyle style = i % 2 == 0 ? EntryBackEven : EntryBackodd;
                        if (Event.current.type == EventType.Repaint)
                            style.Draw(ABBContentItemTableCalc.rows[i].position, false, false, ABBContentItemTableCalc.rows[i].selected, false);
                        this.Label(ABBContentItemTableCalc.rows[i][AssetName].position, ChoosedAsset.AssetBundles[i]);
                        //this.Label(table.Rows[i][Preview].Position, choo.ThumbNail);
                    }
                }, ABBContentItemTableCalc.view,ref ABBContentItemScrollPos,ABBContentItemTableCalc.content, false, false);
            }


            public override void OnGUI(Rect position)
            {
                base.OnGUI(position);
                big.OnGUI(position);
            }
        }


        private const float ToolBarHeight = 17;
        enum WindowType
        {
            Tool,
            DirectoryCollect,
            AssetBundleBuild,
        }
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private ToolBarTree ToolBarTree;
        private WindowType _windowType;
      

        public ABEditorInfo Info;
        private string EditorInfoPath;
        private AssetBundleBulidGUI abBulidWindow;
        private DirCollectGUI dirCollectWindow;
        private ToolGUI toolWindow;

        private void DeleteBundle(string bundleName)
        {
            Info.ABBuiidInfo.RemoveBundle(bundleName);
        }
        private void RemoveAsset(string assetPath, string bundleName)
        {
            Info.ABBuiidInfo.RemoveAsset(bundleName, assetPath);
        }
        private void UpdateInfo()
        {
            File.WriteAllText(EditorInfoPath, Xml.ToXmlString(Info));
            Info = Xml.ToObject<ABEditorInfo>(File.ReadAllText(EditorInfoPath));
        }
    }
    partial class AssetBundleWindow
    {
        private void ABInit()
        {
            EditorInfoPath = EditorEnv.memoryPath.CombinePath("AssetBundleEditorInfo.xml");
            if (!File.Exists(EditorInfoPath))
                File.WriteAllText(EditorInfoPath, Xml.ToXmlString(new ABEditorInfo()));
            Info = Xml.ToObject<ABEditorInfo>(File.ReadAllText(EditorInfoPath));


            if (toolWindow == null)
                toolWindow = new ToolGUI();
            if (dirCollectWindow == null)
                dirCollectWindow = new DirCollectGUI();
            if (abBulidWindow == null)
                abBulidWindow = new AssetBundleBulidGUI();
        }
        private List<Collecter> LoadCollecters()
        {
            var collecters = Info.DirCollect.DirCollectItems.ConvertAll<Collecter>((item) =>
            {
                if (string.IsNullOrEmpty(item.SearchPath)) return null;
                switch (item.CollectType)
                {
                    case ABDirCollectItem.ABDirCollectType.ABName:
                        if (!string.IsNullOrEmpty(item.BundleName))
                        {
                            return new ABNameCollecter()
                            {
                                bundleName = item.BundleName,
                                searchPath = item.SearchPath,
                                MeetFiles = item.GetSubAssetPaths()
                            };
                        }
                        return null;
                    case ABDirCollectItem.ABDirCollectType.DirName:
                        return new DirNameCollecter()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    case ABDirCollectItem.ABDirCollectType.FileName:
                        return new FileNameCollecter()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    case ABDirCollectItem.ABDirCollectType.Scene:
                        return new ScenesCollecter()
                        {
                            searchPath = item.SearchPath,
                            MeetFiles = item.GetSubAssetPaths()
                        };
                    default:
                        return null;
                }

            });
            collecters = collecters.ReverseForEach((col) =>
            {
                if (col == null)
                    collecters.Remove(col);
            });
            return collecters;
        }
        private static AssetBundleWindow _window;
        private void OnEnable()
        {
            _window = this;
            Collect.onLoadBuilders -= LoadCollecters;
            Collect.onLoadBuilders += LoadCollecters;
            ABInit();
            ToolBarTree = new ToolBarTree();
            ToolBarTree.Popup((value) => { _windowType = (WindowType)value; }, Enum.GetNames(typeof(WindowType)))
                            .Button(EditorGUIUtility.IconContent("Refresh"), (rect) =>
                            {
                                abBulidWindow.ChoosedAsset = null;
                                abBulidWindow.ChossedABB = null;
                                Info.ABBuiidInfo.ReadAssetbundleBuild(Collect.GetCollection(ABTool.configPath));
                                UpdateInfo();
                            },20,()=> { return _windowType == WindowType.AssetBundleBuild; })
                            //.Button(new GUIContent(), (rect) =>
                            //{
                            //    File.WriteAllText(tmpLayout_ABBuildInfoPath, abBulidWindow.abBuildInfoTree.Serialize());
                            //})
                            ;
        }
        private void OnDisable()
        {
            UpdateInfo();
        }

       
        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            switch (_windowType)
            {
                case WindowType.Tool:
                    toolWindow.OnGUI(rs[1]);
                    break;
                case WindowType.DirectoryCollect:
                    dirCollectWindow.OnGUI(rs[1]);
                    break;
                case WindowType.AssetBundleBuild:
                    abBulidWindow.OnGUI(rs[1]);
                    break;
                default:
                    break;
            }
            ToolBarTree.OnGUI(rs[0]);
        }
    }
}
