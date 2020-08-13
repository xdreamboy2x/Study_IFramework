/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using IFramework.Serialization;
using IFramework.GUITool;
using IFramework.GUITool.HorizontalMenuToorbar;
using IFramework.Serialization.DataTable;
namespace IFramework.Language
{
    [EditorWindowCache("IFramework.Language")]
    public partial class LanWindow : EditorWindow
    {
        private class Styles
        {
            public static GUIStyle EntryBackodd = "CN EntryBackodd";
            public static GUIStyle EntryBackEven = "CN EntryBackEven";
            public static GUIStyle Title = "IN BigTitle";
            public static GUIStyle TitleTxt = "IN BigTitle Inner";
            public static GUIStyle BoldLabel = EditorStyles.boldLabel;
            public static GUIStyle toolbarButton = EditorStyles.toolbarButton;
            public static GUIStyle toolbar = EditorStyles.toolbar;
            public static GUIStyle Fold = GUIStyles.Get("ToolbarDropDown");
            public static GUIStyle FoldOut = EditorStyles.foldout;
            public static GUIStyle CloseBtn = "WinBtnClose";
            public static GUIStyle minus = "OL Minus";
            public static GUIStyle BG = "box";
            public static GUIStyle box = "box";
            public static GUIStyle in_title = new GUIStyle("IN Title") { fixedHeight = 20 + 5 };
            public static GUIStyle settingsHeader = "SettingsHeader";
            public static GUIStyle header = "DD HeaderStyle";
            public static GUIStyle toolbarSeachTextFieldPopup = "ToolbarSeachTextFieldPopup";
            public static GUIStyle searchTextField = new GUIStyle("ToolbarTextField")
            {
                margin = new RectOffset(0, 0, 2, 0)
            };
            public static GUIStyle searchCancelButton = "ToolbarSeachCancelButton";
            public static GUIStyle searchCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
            public static GUIStyle foldout = "Foldout";
            public static GUIStyle ToolbarDropDown = "ToolbarDropDown";
            public static GUIStyle selectionRect = "SelectionRect";

            static Styles()
            {
                Fold.fixedHeight = BoldLabel.fixedHeight;
            }
        }
        private class Contents
        {

            public static GUIContent CreateViewTitle = new GUIContent("Create", EditorGUIUtility.IconContent("tree_icon_leaf").image);
            public static GUIContent GroupTitle = new GUIContent("Group", EditorGUIUtility.IconContent("d_tree_icon_frond").image);
            public static GUIContent CopyBtn = new GUIContent("C", "Copy");
            public static GUIContent OK = EditorGUIUtility.IconContent("vcs_add");
            public static GUIContent Warnning = EditorGUIUtility.IconContent("console.warnicon.sml");

        }
        private const string CreateViewNmae = "CreateView";
        private const string Group = "Group";
        private CreateView createView = new CreateView();
        private GroupView group = new GroupView();
        [SerializeField]
        private bool mask = true;
        private Color maskColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        [SerializeField]
        private string tmpLayout;
        private const float ToolBarHeight = 17;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree sunwin;
        private ToolBarTree ToolBarTree;


        private abstract class LanwindowItem : ILayoutGUIDrawer, IRectGUIDrawer
        {
            public static LanWindow window;
            protected Rect position;
            
            protected float TitleHeight { get { return Styles.Title.CalcHeight(titleContent, position.width); } }
            protected float smallBtnSize = 20;
            protected float describeWidth = 30;
            protected virtual GUIContent titleContent { get; }
            public void OnGUI(Rect position)
            {
                this.position = position;
                position.DrawOutLine(2, Color.black);
                this.DrawClip(() => {
                    Rect[] rs = position.HorizontalSplit(TitleHeight);
                    this.Box(rs[0]);
                    this.Box(rs[0], titleContent, Styles.Title);
                    DrawContent(rs[1]);
                }, position);

            }
            protected abstract void DrawContent(Rect rect);
        }
    }
    public partial class LanWindow : EditorWindow
    {
        private LanGroup _group;
        private List<LanPair> _pairs { get { return _group.pairs; } }
        private List<string> _keys { get { return _group.keys; } }

        private string stoPath;
        private void OnEnable()
        {
            LanwindowItem.window = this;
            stoPath = EditorEnv.frameworkPath.CombinePath(LanGroup.assetPath);
            LoadLanGroup();
            this.titleContent = new GUIContent("Lan", EditorGUIUtility.IconContent("d_WelcomeScreen.AssetStoreLogo").image);
            SubwinInit();
        }
        private void LoadLanGroup()
        {
            if (File.Exists(stoPath))
                _group = ScriptableObj.Load<LanGroup>(stoPath);
            else
                _group = ScriptableObj.Create<LanGroup>(stoPath);
        }
        private void UpdateLanGroup()
        {
            ScriptableObj.Update(_group);
        }
        private void OnDisable()
        {
            tmpLayout = sunwin.Serialize();
            UpdateLanGroup();
        }

        private void Views(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < sunwin.allLeafCount; i++)
            {
                SubWinTree.TreeLeaf leaf = sunwin.allLeafs[i];
                menu.AddItem(leaf.titleContent, !sunwin.closedLeafs.Contains(leaf), () => {
                    if (sunwin.closedLeafs.Contains(leaf))
                        sunwin.DockLeaf(leaf, SubWinTree.DockType.Left);
                    else
                        sunwin.CloseLeaf(leaf);
                });
            }
            menu.DropDown(rect);
            Event.current.Use();
        }
        private void SubwinInit()
        {
            sunwin = new SubWinTree();
            sunwin.repaintEve += Repaint;
            sunwin.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };
            if (string.IsNullOrEmpty(tmpLayout))
            {
                for (int i = 1; i <= 2; i++)
                {
                    string userdata = i == 1 ? "Group" :  "CreateView";
                    SubWinTree.TreeLeaf L = sunwin.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    sunwin.DockLeaf(L, SubWinTree.DockType.Left);
                }
            }
            else
            {
                sunwin.DeSerialize(tmpLayout);
            }
            sunwin[Group].titleContent = new GUIContent(Group);
            sunwin[Group].minSize = new Vector2(250, 250);
            sunwin[CreateViewNmae].minSize = new Vector2(300, 300);
            sunwin[Group].paintDelegate += group.OnGUI;
            sunwin[CreateViewNmae].paintDelegate += createView.OnGUI;


            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"), Views, 60)
                            .FlexibleSpace()
                            .Toggle(new GUIContent("mask"), m => mask = m, mask, 60)
                            .Delegate((r) => {
                                maskColor = EditorGUI.ColorField(r, maskColor);
                            }, 80)
                            .Toggle(new GUIContent("Title"), (bo) => { sunwin.isShowTitle = bo; }, sunwin.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { sunwin.isLocked = bo; }, sunwin.isLocked, 60);

        }

        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            ToolBarTree.OnGUI(rs[0]);
            sunwin.OnGUI(rs[1]);
            this.minSize = sunwin.minSize + new Vector2(0, ToolBarHeight);
            Repaint();
            if (mask)
            {
                GUI.backgroundColor = maskColor;
                GUI.Box(localPosition, "");
            }
        }

        private void DeletePairsByKey(string key)
        {
            _group.DeletePairsByKey(key);
            UpdateLanGroup();
        }
        private void DeleteLanPair(LanPair pair)
        {
            _group.DeletePair(pair);
        }
        private void AddLanPair(LanPair pair)
        {
            if (string.IsNullOrEmpty(pair.value.Trim()))
            {
                ShowNotification(new GUIContent("Value Can't be Null"));
                return;
            }
            LanPair tmpPair = new LanPair()
            {
                lan = pair.lan,
                key = pair.key,
                value = pair.value
            };
            LanPair lp = _pairs.Find((p) => { return p.lan == tmpPair.lan && p.key == tmpPair.key; });
            if (lp == null)
            {
                _pairs.Add(tmpPair);
                UpdateLanGroup();
            }
            else
            {
                if (lp.value == tmpPair.value)
                    ShowNotification(new GUIContent("Don't Add Same"));
                else
                {
                    if (EditorUtility.DisplayDialog("Warn",
                        string.Format("Replace Old Value ?\n Old Value  {0}\n New Vlaue  {1}", lp.value, tmpPair.value), "Yes", "No"))
                    {
                        lp.value = tmpPair.value;
                    }
                }
            }
        }

        private void AddLanGroupKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ShowNotification(new GUIContent("Err: key is Empty " + key));
                return;
            }
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                UpdateLanGroup();
            }
            else
            {
                ShowNotification(new GUIContent("Err: key Has Exist " + key));
            }
        }
        private void DeleteLanKey(string key)
        {
            if (_keys.Contains(key))
            {
                _keys.Remove(key);
                DeletePairsByKey(key);
            }
        }
        private void CleanData()
        {
            _pairs.Clear();
            _keys.Clear();
            UpdateLanGroup();
        }
        private void WriteXml(string path)
        {
            path.WriteText(Xml.ToXmlString(_pairs), Encoding.UTF8);
        }
        private void ReadXml(string path)
        {
            List<LanPair> ps = Xml.ToObject<List<LanPair>>(path.ReadText(Encoding.UTF8))
                .Distinct()
                .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.value); });
            AddLanPairs(ps);
        }
        private void WriteJson(string path)
        {
            path.WriteText(JsonUtility.ToJson(_pairs), Encoding.UTF8);
        }
        private void ReadJson(string path)
        {
            List<LanPair> ps = JsonUtility.FromJson<List<LanPair>>(path.ReadText(Encoding.UTF8))
               .Distinct()
               .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.value); });
            AddLanPairs(ps);
        }
        private bool IsKeyInUse(string key)
        {
            for (int i = 0; i < _pairs.Count; i++)
            {
                if (_pairs[i].key==key)
                {
                    return true;
                }
            }
            return false;
        }
        private void ReadCsv(string path)
        {
            DataReader dw = new DataReader(new StreamReader(path, System.Text.Encoding.UTF8), new DataRow(), new DataExplainer());
           var pairs= dw.Get<LanPair>().Distinct()
                .ToList().FindAll((p) => { return !string.IsNullOrEmpty(p.key) && !string.IsNullOrEmpty(p.value); });
              dw.Dispose();
            AddLanPairs(pairs);
        }
        private void ReadScriptableObject(string path)
        {
            var g = AssetDatabase.LoadAssetAtPath<LanGroup>(path.ToAssetsPath());
            if (g == null) return;
            AddLanPairs(g.pairs);
        }
        private void WriteScriptableObject(string path)
        {
            var g = AssetDatabase.LoadAssetAtPath<LanGroup>(path.ToAssetsPath());
            if (g == null) return;
            g.pairs.AddRange(_pairs);
            g.pairs.Distinct();
            ScriptableObj.Update(g);
        }

        private void WriteCsv(string path)
        {
            DataWriter w = new DataWriter(new System.IO.StreamWriter(path, false),
                           new DataRow(),
                           new DataExplainer());
            w.Write(_pairs);
            w.Dispose();
        }


        private void AddLanPairs(List<LanPair> pairs)
        {
            if (pairs == null || pairs.Count == 0) return;
            for (int i = 0; i < pairs.Count; i++)
            {
                var filePair = pairs[i];
                if (!_keys.Contains(filePair.key)) _keys.Add(filePair.key);
                LanPair oldPair = _pairs.Find((pair) => { return pair.key == filePair.key && pair.lan == filePair.lan; });
                if (oldPair == null) _pairs.Add(filePair);
                else
                {
                    if (oldPair.value != filePair.value)
                    {
                        if (EditorUtility.DisplayDialog("Warning",
                                            "The LanPair Is Same Do You Want Replace \n"
                                            .Append(string.Format("Lan {0}\t\t Key {0}\t \n", oldPair.lan, oldPair.key))
                                            .Append(string.Format("Old  Val\t\t {0}\n", oldPair.value))
                                            .Append(string.Format("New  Val\t\t {0}\n", filePair.value))
                                            , "Yes", "No"))
                        {
                            oldPair.value = filePair.value;
                        }
                    }
                }
            }
            UpdateLanGroup();
        }
        [Serializable]
        private class CreateView : LanwindowItem
        {
            public CreateView()
            {
                searchField = new SearchFieldDrawer("", null, 0);

                searchField.onValueChange += (str) => {
                    keySearchStr = str;
                };
            }
            protected override GUIContent titleContent { get { return Contents.CreateViewTitle; } }
            [SerializeField] private bool toolFoldon;
            private void Tool()
            {
                Rect rect;
                this.EBeginHorizontal(out rect, Styles.Fold)
                    .Foldout(ref toolFoldon, "Tool", true);
                this.EEndHorizontal()
                    .Pan(() => {
                        if (!toolFoldon) return;
                        this.BeginHorizontal()
                                      .Button(() => {
                                          window.LoadLanGroup();
                                      }, "Fresh")
                                      .Button(() => {
                                          window.UpdateLanGroup();
                                      }, "Save")
                                      .Button(() => {
                                          window.CleanData();
                                      }, "Clear")
                                 .EndHorizontal()
                                 .BeginHorizontal()
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("xml (End with  xml)", Application.dataPath, "xml");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".xml")) return;
                                         window.ReadXml(path);
                                     }, "Read Xml")
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("xml (End with  xml)", Application.dataPath, "xml");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".xml")) return;
                                         window.WriteXml(path);
                                     }, "Write Xml")
                                 .EndHorizontal()
                                 .BeginHorizontal()
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("json (End With json)", Application.dataPath, "json");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".json")) return;
                                         window.ReadJson(path);
                                     }, "Read Json")
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("json (End With json)", Application.dataPath, "json");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".json")) return;
                                         window.WriteJson(path);
                                     }, "Write Json")
                                 .EndHorizontal()
                                 .BeginHorizontal()
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("csv (End With csv)", Application.dataPath, "csv");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".csv")) return;
                                         window.ReadCsv(path);
                                     }, "Read Csv")
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("csv (End With csv)", Application.dataPath, "csv");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".csv")) return;
                                         window.WriteCsv(path);
                                     }, "Write Csv")
                                 .EndHorizontal()
                                 .BeginHorizontal()
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("ScriptableObject (End With asset)", Application.dataPath, "asset");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset")) return;
                                         window.ReadScriptableObject(path);
                                     }, "Read ScriptableObject")
                                     .Button(() => {
                                         string path = EditorUtility.OpenFilePanel("ScriptableObject (End With asset)", Application.dataPath, "asset");
                                         if (string.IsNullOrEmpty(path) || !path.EndsWith(".asset")) return;
                                         window.WriteScriptableObject(path);
                                     }, "Write ScriptableObject")
                                 .EndHorizontal();
                    });
            }
            [SerializeField] private bool creatingKeyFoldon;
            [SerializeField] private string tmpKey;
            private void CreateLanKey()
            {
                Rect rect;
                this.EBeginHorizontal(out rect, Styles.Fold)
                    .Foldout(ref creatingKeyFoldon, "Create Key", true)
                    .EEndHorizontal()
                    .Pan(() => {
                        if (!creatingKeyFoldon) return;
                        this.EBeginHorizontal(out rect, Styles.BG)
                                   .TextField(ref tmpKey)
                                   .Button(() =>
                                   {
                                       window.AddLanGroupKey(tmpKey);
                                       tmpKey = string.Empty;
                                   }, Contents.OK, GUILayout.Width(describeWidth))
                                  .EEndHorizontal();
                    });
            }

            [SerializeField] private bool createLanPairFlodon;
            [SerializeField] private LanPair tmpLanPair;
            [SerializeField] private int hashID;
            private void AddLanPairFunc()
            {
                if (window._keys.Count == 0) return;
                Rect rect;
                this.EBeginHorizontal(out rect, Styles.Fold)
                    .Foldout(ref createLanPairFlodon, "Create LanPair", true)
                    .EEndHorizontal()
                    .Pan(() =>
                    {
                        if (!createLanPairFlodon) return;
                        if (tmpLanPair == null) tmpLanPair = new LanPair() { key = window._keys[0] };
                        if (hashID == 0) hashID = "CreateView".GetHashCode();
                        this.DrawVertical(() =>
                        {
                            this.BeginHorizontal()
                                    .Label("Lan", GUILayout.Width(describeWidth))
                                    .Pan(() => { tmpLanPair.lan = (SystemLanguage)EditorGUILayout.EnumPopup(tmpLanPair.lan); })
                                .EndHorizontal()
                                .BeginHorizontal()
                                    .Label("Key", GUILayout.Width(describeWidth))
                                    .Label(tmpLanPair.key)
                                    .Label(EditorGUIUtility.IconContent("editicon.sml"), GUILayout.Width(smallBtnSize))
                                .EndHorizontal()
                                .Pan(() => {
                                    Rect pos = GUILayoutUtility.GetLastRect();
                                    int ctrlId = GUIUtility.GetControlID(hashID, FocusType.Keyboard, pos);
                                    {
                                        if (DropdownButton(ctrlId, pos, new GUIContent(string.Format("Key: {0}", tmpLanPair.key))))
                                        {
                                            int index = -1;
                                            for (int i = 0; i < window._keys.Count; i++)
                                            {
                                                if (window._keys[i] == tmpLanPair.key)
                                                {
                                                    index = i; break;
                                                }
                                            }
                                            SearchablePopup.Show(pos, window._keys.ToArray(), index, (i, str) =>
                                            {
                                                tmpLanPair.key = str;
                                                window.Repaint();
                                            });
                                        }
                                    }
                                })
                                .BeginHorizontal()
                                    .Label("Val", GUILayout.Width(describeWidth))
                                    .TextField(ref tmpLanPair.value)
                                    .EndHorizontal()
                                .BeginHorizontal()
                                    .FlexibleSpace()
                                    .Button(() => {
                                        //createLanPairFlodon = false;
                                        window.AddLanPair(tmpLanPair);
                                        //tmpLanPair = null;
                                    }, Contents.OK)
                                .EndHorizontal();
                        }, Styles.BG);
                    });
            }
            private bool DropdownButton(int id, Rect position, GUIContent content)
            {
                Event e = Event.current;
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (position.Contains(e.mousePosition) && e.button == 0)
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.KeyDown:
                        if (GUIUtility.keyboardControl == id && e.character == '\n')
                        {
                            Event.current.Use();
                            return true;
                        }
                        break;
                    case EventType.Repaint:
                        //Styles.BoldLabel.Draw(position, content, id, false);
                        break;
                }
                return false;
            }

            [SerializeField] private bool keyFoldon;
            [SerializeField] private Vector2 scroll;
            [SerializeField] private string keySearchStr = string.Empty;
            private SearchFieldDrawer searchField;
            private void LanGroupKeysView()
            {
                this.DrawHorizontal(() => {
                    this.Foldout(ref keyFoldon, string.Format("Keys  Count: {0}", window._keys.Count), true);
                    this.Label("");
                  searchField.OnGUI(GUILayoutUtility.GetLastRect());
                }, Styles.Fold);
                if (keyFoldon)
                {
                    this.DrawScrollView(() => {
                        window._keys.ForEach((index,key) => {
                            if (key.ToLower().Contains(keySearchStr.ToLower()))
                            {
                                this.BeginHorizontal(Styles.BG)
                                    .SelectableLabel(key,GUILayout.Height(20))
                                    .Label(window.IsKeyInUse(key) ? GUIContent.none : Contents.Warnning, GUILayout.Width(smallBtnSize))
                                    .Button(() => {
                                        if (EditorUtility.DisplayDialog("Make sure","You Will Delete All Pairs with this key","ok","no"))
                                            window.DeleteLanKey(key);
                                    }, string.Empty, Styles.CloseBtn, GUILayout.Width(smallBtnSize), GUILayout.Height(smallBtnSize))
                                    .EndHorizontal();
                            }
                        });
                    }, ref scroll);
                }
            }

            protected override void DrawContent(Rect rect)
            {
                this
                    .Pan(() =>
                    {
                        this.BeginArea(rect.Zoom(AnchorType.MiddleCenter, -10))
                            .Pan(Tool)
                            .Space(5)
                            .Pan(AddLanPairFunc)
                            .Space(5)
                            .Pan(CreateLanKey)
                            .Space(5)
                            .Pan(LanGroupKeysView)
                            .EndArea();
                    });
            }

            private class SearchablePopup : PopupWindowContent
            {
                public class InnerSearchField : FocusAbleGUIDrawer
                {
                    private class Styles
                    {
                        public static GUIStyle SearchTextFieldStyle = GUIStyles.Get("ToolbarSeachTextField");
                        public static GUIStyle SearchCancelButtonStyle = GUIStyles.Get("SearchCancelButton");
                        public static GUIStyle SearchCancelButtonEmptyStyle = GUIStyles.Get("SearchCancelButtonEmpty");
                    }

                    public string OnGUI(Rect position, string value)
                    {
                        GUIStyle cancelBtnStyle = string.IsNullOrEmpty(value) ? Styles.SearchCancelButtonEmptyStyle : Styles.SearchCancelButtonStyle;
                        position.width -= cancelBtnStyle.fixedWidth;

                        Styles.SearchTextFieldStyle.fixedHeight = position.height;
                        cancelBtnStyle.fixedHeight = position.height;

                        Styles.SearchTextFieldStyle.alignment = TextAnchor.MiddleLeft;
                        while (Styles.SearchTextFieldStyle.lineHeight < position.height - 15)
                        {
                            Styles.SearchTextFieldStyle.fontSize++;
                        }
                        GUI.SetNextControlName(focusID);

                        value = GUI.TextField(new Rect(position.x,
                                                                         position.y + 1,
                                                                         position.width,
                                                                         position.height - 1),
                                                                         value,
                                                                         Styles.SearchTextFieldStyle);
                        if (GUI.Button(new Rect(position.x + position.width,
                                                position.y + 1,
                                                cancelBtnStyle.fixedWidth,
                                                cancelBtnStyle.fixedHeight
                                                ),
                                                GUIContent.none,
                                                cancelBtnStyle))
                        {
                            value = string.Empty;
                            GUI.changed = true;
                            GUIUtility.keyboardControl = 0;
                        }


                        Event e = Event.current;
                        if (position.Contains(e.mousePosition))
                        {
                            if (!focused)
                                if ((e.type == EventType.MouseDown /*&& e.clickCount == 2*/) /*|| e.keyCode == KeyCode.F2*/)
                                {
                                    focused = true;
                                    GUIFocusControl.Focus(this);
                                    if (e.type != EventType.Repaint && e.type != EventType.Layout)
                                        Event.current.Use();
                                }
                        }
                        //if ((/*e.keyCode == KeyCode.Return ||*/ e.keyCode == KeyCode.Escape || e.character == '\n'))
                        //{
                        //    GUIFocusControl.Focus(null, Focused);
                        //    Focused = false;
                        //    if (e.type != EventType.Repaint && e.type != EventType.Layout)
                        //        Event.current.Use();
                        //}
                        return value;
                    }

                }
                private const float rowHeight = 16.0f;
                private const float rowIndent = 8.0f;
                public static void Show(Rect activatorRect, string[] options, int current, Action<int, string> onSelectionMade)
                {
                    SearchablePopup win = new SearchablePopup(options, current, onSelectionMade);
                    PopupWindow.Show(activatorRect, win);
                }
                private static void Repaint() { EditorWindow.focusedWindow.Repaint(); }
                private static void DrawBox(Rect rect, Color tint)
                {
                    Color c = GUI.color;
                    GUI.color = tint;
                    GUI.Box(rect, "", Selection);
                    GUI.color = c;
                }
                private class FilteredList
                {
                    public struct Entry
                    {
                        public int Index;
                        public string Text;
                    }
                    private readonly string[] allItems;
                    public FilteredList(string[] items)
                    {
                        allItems = items;
                        Entries = new List<Entry>();
                        UpdateFilter("");
                    }
                    public string Filter { get; private set; }
                    public List<Entry> Entries { get; private set; }
                    public int Count { get { return allItems.Length; } }
                    public bool UpdateFilter(string filter)
                    {
                        if (Filter == filter)
                            return false;
                        Filter = filter;
                        Entries.Clear();
                        for (int i = 0; i < allItems.Length; i++)
                        {
                            if (string.IsNullOrEmpty(Filter) || allItems[i].ToLower().Contains(Filter.ToLower()))
                            {
                                Entry entry = new Entry
                                {
                                    Index = i,
                                    Text = allItems[i]
                                };
                                if (string.Equals(allItems[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                                    Entries.Insert(0, entry);
                                else
                                    Entries.Add(entry);
                            }
                        }
                        return true;
                    }
                }

                private readonly Action<int, string> onSelectionMade;
                private readonly int currentIndex;
                private readonly FilteredList list;
                private Vector2 scroll;
                private int hoverIndex;
                private int scrollToIndex;
                private float scrollOffset;
                private static GUIStyle Selection = "SelectionRect";

                private SearchablePopup(string[] names, int currentIndex, Action<int, string> onSelectionMade)
                {
                    list = new FilteredList(names);
                    this.currentIndex = currentIndex;
                    this.onSelectionMade = onSelectionMade;
                    hoverIndex = currentIndex;
                    scrollToIndex = currentIndex;
                    scrollOffset = GetWindowSize().y - rowHeight * 2;
                }

                public override void OnOpen()
                {
                    base.OnOpen();
                    EditorEnv.update += Repaint;
                }
                public override void OnClose()
                {
                    base.OnClose();
                    EditorEnv.update -= Repaint;
                }

                public override Vector2 GetWindowSize()
                {
                    return new Vector2(base.GetWindowSize().x,
                        Mathf.Min(600, list.Count * rowHeight + EditorStyles.toolbar.fixedHeight));
                }

                public override void OnGUI(Rect rect)
                {
                    Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
                    Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

                    HandleKeyboard();
                    DrawSearch(searchRect);
                    DrawSelectionArea(scrollRect);
                }
                private InnerSearchField searchField = new InnerSearchField();
                private void DrawSearch(Rect rect)
                {
                    GUI.Label(rect, "", EditorStyles.toolbar);
                    if (list.UpdateFilter(searchField.OnGUI(rect.Zoom(AnchorType.MiddleCenter, -2), list.Filter)))
                    {
                        hoverIndex = 0;
                        scroll = Vector2.zero;
                    }
                }

                private void DrawSelectionArea(Rect scrollRect)
                {
                    Rect contentRect = new Rect(0, 0,
                        scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth,
                        list.Entries.Count * rowHeight);

                    scroll = GUI.BeginScrollView(scrollRect, scroll, contentRect);

                    Rect rowRect = new Rect(0, 0, scrollRect.width, rowHeight);

                    for (int i = 0; i < list.Entries.Count; i++)
                    {
                        if (scrollToIndex == i &&
                            (Event.current.type == EventType.Repaint
                             || Event.current.type == EventType.Layout))
                        {
                            Rect r = new Rect(rowRect);
                            r.y += scrollOffset;
                            GUI.ScrollTo(r);
                            scrollToIndex = -1;
                            scroll.x = 0;
                        }

                        if (rowRect.Contains(Event.current.mousePosition))
                        {
                            if (Event.current.type == EventType.MouseMove ||
                                Event.current.type == EventType.ScrollWheel)
                                hoverIndex = i;
                            if (Event.current.type == EventType.MouseDown)
                            {
                                onSelectionMade(list.Entries[i].Index, list.Entries[i].Text);
                                EditorWindow.focusedWindow.Close();
                            }
                        }

                        DrawRow(rowRect, i);

                        rowRect.y = rowRect.yMax;
                    }

                    GUI.EndScrollView();
                }

                private void DrawRow(Rect rowRect, int i)
                {
                    if (list.Entries[i].Index == currentIndex)
                        DrawBox(rowRect, Color.cyan);
                    else if (i == hoverIndex)
                        DrawBox(rowRect, Color.white);
                    Rect labelRect = new Rect(rowRect);
                    labelRect.xMin += rowIndent;
                    GUI.Label(labelRect, list.Entries[i].Text);
                }
                private void HandleKeyboard()
                {
                    Event e = Event.current;
                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.DownArrow)
                        {
                            hoverIndex = Mathf.Min(list.Entries.Count - 1, hoverIndex + 1);
                            Event.current.Use();
                            scrollToIndex = hoverIndex;
                            scrollOffset = rowHeight;
                        }
                        if (e.keyCode == KeyCode.UpArrow)
                        {
                            hoverIndex = Mathf.Max(0, hoverIndex - 1);
                            Event.current.Use();
                            scrollToIndex = hoverIndex;
                            scrollOffset = -rowHeight;
                        }
                        if (e.keyCode == KeyCode.Return || e.character == '\n')
                        {
                            if (hoverIndex >= 0 && hoverIndex < list.Entries.Count)
                            {
                                onSelectionMade(list.Entries[hoverIndex].Index, list.Entries[hoverIndex].Text);
                                EditorWindow.focusedWindow.Close();
                            }
                        }
                        if (e.keyCode == KeyCode.Escape)
                        {
                            EditorWindow.focusedWindow.Close();
                        }
                    }
                }
            }
        }



        [Serializable]
        private class GroupView : LanwindowItem
        {
            protected override GUIContent titleContent { get { return  Contents.GroupTitle; } }
            private TableViewCalculator _table = new TableViewCalculator();
            private SearchFieldDrawer search;
            private Vector2 _scroll;
            private const string key = "Key";
            private const string lan = "Language";
            private const string value = "Value";
            private const string minnus = "minnus";
            private enum SearchType
            {
                Key, Language, Value
            }
            private SearchType _searchType;
            private string _search;
            private const float lineHeight = 20;
            public GroupView()
            {
                search = new SearchFieldDrawer("", Enum.GetNames(typeof(SearchType)), 0);
                search.onModeChange += (value) => { _searchType = (SearchType)value; };
                search.onValueChange += (value) => { _search = value; };
            }
            private ListViewCalculator.ColumnSetting[] setting
            {
                get
                {
                    return new ListViewCalculator.ColumnSetting[] {
                        new ListViewCalculator.ColumnSetting()
                        {
                            width=20,
                            name=minnus,

                        },
                         new ListViewCalculator.ColumnSetting()
                        {
                            width=100,
                            name=lan,
                        },
                        new ListViewCalculator.ColumnSetting()
                        {
                            width=100,
                            name=key,
                        },
                          new ListViewCalculator.ColumnSetting()
                        {
                            width=100,
                            name=value,

                        }

                    };
                }
            }
            protected override void DrawContent(Rect rect)
            {
                var rs = rect.Zoom( AnchorType.MiddleCenter,-10).Split(SplitType.Horizontal, 30, 4);
                search.OnGUI(rs[0]);
                var ws = window._pairs.FindAll((w) => {
                    if (string.IsNullOrEmpty(_search))
                        return true;
                    switch (_searchType)
                    {
                        case SearchType.Key:
                            return w.key.ToLower().Contains(_search.ToLower());
                        case SearchType.Language:
                            return w.lan.ToString().ToLower().Contains(_search.ToLower());
                        case SearchType.Value:
                            return w.value.ToLower().Contains(_search.ToLower());
                    }
                    return true;
                    }).ToArray();
                _table.Calc(rs[1], new Vector2(rs[1].x, rs[1].y + lineHeight), _scroll, lineHeight, ws.Length, setting);
                this.LabelField(_table.titleRow.position, "", Styles.Title)
                    .LabelField(_table.titleRow[key].position, key)
                    .LabelField(_table.titleRow[lan].position, lan)
                    .LabelField(_table.titleRow[value].position, value);
                Event e = Event.current;
                this.DrawScrollView(() => {

                    for (int i = _table.firstVisibleRow; i < _table.lastVisibleRow+1; i++)
                    {
                        if (e.modifiers == EventModifiers.Control &&
                               e.button == 0 && e.clickCount == 1 &&
                               _table.rows[i].position.Contains(e.mousePosition))
                        {
                            _table.ControlSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.modifiers == EventModifiers.Shift &&
                                        e.button == 0 && e.clickCount == 1 &&
                                        _table.rows[i].position.Contains(e.mousePosition))
                        {
                            _table.ShiftSelectRow(i);
                            window.Repaint();
                        }
                        else if (e.button == 0 && e.clickCount == 1 &&
                                        _table.rows[i].position.Contains(e.mousePosition)
                                      /*  && ListView.viewPosition.Contains(Event.current.mousePosition) */)
                        {
                            _table.SelectRow(i);
                            window.Repaint();
                        }
                        if (e.button == 0 && e.clickCount == 1 &&
              (!_table.view.Contains(e.mousePosition) ||
                  (_table.view.Contains(e.mousePosition) &&
                   !_table.content.Contains(e.mousePosition))))
                        {
                            _table.SelectNone();
                            window.Repaint();
                        }

                        if (e.button == 1 && e.clickCount == 1 &&
                        _table.content.Contains(e.mousePosition))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Delete"), false, () => {
                                for (int j = _table.rows.Count - 1; j >= 0; j--)
                                {
                                    if (_table.rows[j].selected)
                                        window.DeleteLanPair(ws[j]);
                                }
                                window.UpdateLanGroup();
                            });

                            menu.ShowAsContext();
                            if (e.type != EventType.Layout)
                                e.Use();

                        }


                        GUIStyle style = i % 2 == 0 ? Styles.EntryBackEven : Styles.EntryBackodd;
                        if (Event.current.type == EventType.Repaint)
                            style.Draw(_table.rows[i].position, false, false, _table.rows[i].selected, false);

                        this.Pan(() => {
                            EditorGUI.EnumPopup(_table.rows[i][lan].position, ws[i].lan);
                        })
                        .Button(() => {
                            window.DeleteLanPair(ws[i]);
                            window.UpdateLanGroup();
                        }, _table.rows[i][minnus].position,"",Styles.minus)
                             .SelectableLabel(_table.rows[i][key].position, ws[i].key)
                             .SelectableLabel(_table.rows[i][value].position, ws[i].value);
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
            }
        }
    }
}