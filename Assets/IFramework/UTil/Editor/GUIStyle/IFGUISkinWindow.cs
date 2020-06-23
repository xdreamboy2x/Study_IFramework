/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace IFramework
{
    [EditorWindowCache("GUIStyle")]
    public partial class IFGUISkinWindow : EditorWindow
    {


    }

    public partial class IFGUISkinWindow:EditorWindow,IRectGUIDrawer,ILayoutGUIDrawer
	{
        private const string Name = "Name";
        private const string Style = "Style";
        private const string Btn = "Copy";
        private bool Tog ;

        private string StoPath;
        private IFGUISKin Skin;
        private Vector2 ScrollPos;
        private float TopHeight=30;
        private string input=string .Empty;
        private SearchFieldDrawer searchField = new SearchFieldDrawer();

        private void OnEnable()
        {
            StoPath = EditorEnv.editorPath.
                CombinePath("GUIStyle/Resources/" + IFGUISkinUtil.AssetName+".asset");
            if (!System.IO.File.Exists(StoPath)) ScriptableObj.Create<IFGUISKin>(StoPath);
            Skin = ScriptableObj.Load<IFGUISKin>(StoPath);
            searchField = new SearchFieldDrawer();
            searchField.onValueChange += (str) =>
            {
                input = str;
            };
        }
        private ListViewCalculator listView = new ListViewCalculator();
        private List<GUIStyle> mathList = new List<GUIStyle>();
        private void OnGUI()
        {
            this.DrawHorizontal(() =>
            {
                this.Space(10);
                this.Button(() =>
                {
                    Fresh();
                }, EditorGUIUtility.IconContent("d_TreeEditor.Refresh"),
                   GUILayout.Width(TopHeight),
                    GUILayout.Height(TopHeight));
                this.Space(10);
                this.Button(() =>
                {
                    ShowNotification(new GUIContent("Click And See \nInfo"));
                }, "Tip", GUILayout.Height(TopHeight));
                GUILayout.Label("", GUILayout.Height(TopHeight));
                searchField.OnGUI(GUILayoutUtility.GetLastRect());
                this.Space(10);
                this.Toggle(ref Tog, GUILayout.Width(20));
            }, GUILayout.Height(TopHeight));

            mathList.Clear();
            for (int i = 0; i < Skin.Styles.Count; i++)
                if (Skin.Styles[i].name.ToLower().Contains(input.ToLower()))
                    mathList.Add(Skin.Styles[i]);

            Rect rect = new Rect(0,  TopHeight, position.width, position.height - TopHeight).Zoom(AnchorType.MiddleCenter, -5);
            listView.Calc(rect, rect.position, ScrollPos, 30, mathList.Count, new ListViewCalculator.ColumnSetting[] {
                new ListViewCalculator.ColumnSetting(){ name=Style,width=600,offSetY=-4},

                new ListViewCalculator.ColumnSetting(){ name=Name,width=200,offSetY=-4,offsetX=-10},
                new ListViewCalculator.ColumnSetting(){ name=Btn,width=100,offSetY=-4},
            });
            this.DrawScrollView(() =>
            {
                for (int i = listView.firstVisibleRow; i < listView.lastVisibleRow + 1; i++)
                {
                    this.Label(listView.rows[i][Name].position, mathList[i].name);
                    this.Button(()=> {
                        GUIUtility.systemCopyBuffer = mathList[i].name;
                    },listView.rows[i][Btn].position, Btn);
                    if (Event.current.type == EventType.Repaint)
                        if (Tog)
                        {
                            mathList[i].Draw(listView.rows[i][Style].position, mathList[i].name, false, false, false, false);
                        }
                        else
                        {
                            mathList[i].Draw(listView.rows[i][Style].position,  false, false, false, false);
                        }

                }
            }, listView.view,ref ScrollPos, listView.content);
                      
        }
        private void Fresh()
        {
            if (!System.IO.File.Exists(StoPath)) ScriptableObj.Create<IFGUISKin>(StoPath);
                Skin = ScriptableObj.Load<IFGUISKin>(StoPath);
            Skin.Styles.Clear();
            PropertyInfo[] infos = typeof(EditorStyles).
                GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < infos.Length; i++)
            {
                EditorUtility.DisplayProgressBar(string .Format( "Fresh{0}/{1}",i,infos.Length), 
                                                        string.Empty,
                                                        (float)i/ infos.Length);
                PropertyInfo info = infos[i];
                object o = info.GetValue(null, null);
                
                if (o.GetType() == typeof(GUIStyle))
                {
                    GUIStyle style = o as GUIStyle;
                 
                    Skin.Styles.Add(style);
                }
            }

            Skin.Styles.Add(GUIStyles.Get("CN Box"));
            Skin.Styles.Add(GUIStyles.Get("Button"));
            Skin.Styles.Add(GUIStyles.Get("CN CountBadge"));
            Skin.Styles.Add(GUIStyles.Get("ToolbarButton"));
            Skin.Styles.Add(GUIStyles.Get("Toolbar"));
            Skin.Styles.Add(GUIStyles.Get("CN EntryInfo"));
            Skin.Styles.Add(GUIStyles.Get("CN EntryWarn"));
            Skin.Styles.Add(GUIStyles.Get("CN EntryError"));
            Skin.Styles.Add(GUIStyles.Get("CN EntryBackEven") );
            Skin.Styles.Add(GUIStyles.Get("CN EntryBackodd"));
            Skin.Styles.Add(GUIStyles.Get("CN Message"));
            Skin.Styles.Add(GUIStyles.Get("CN StatusError"));
            Skin.Styles.Add(GUIStyles.Get("CN StatusWarn"));
            Skin.Styles.Add(GUIStyles.Get("CN StatusInfo"));
   
            Skin.Styles.Add(GUIStyles.Get("LODBlackBox"));
            Skin.Styles.Add(GUIStyles.Get("GameViewBackground"));
            Skin.Styles.Add(GUIStyles.Get("WindowBackground"));
            Skin.Styles.Add(GUIStyles.Get("MiniToolbarButton"));
            Skin.Styles.Add(GUIStyles.Get("dockarea"));
            Skin.Styles.Add(GUIStyles.Get("hostview"));
            Skin.Styles.Add(GUIStyles.Get("dragtabdropwindow"));
            Skin.Styles.Add(GUIStyles.Get("PaneOptions"));
            Skin.Styles.Add(GUIStyles.Get("SelectionRect"));
            Skin.Styles.Add(GUIStyles.Get("window"));
            Skin.Styles.Add(GUIStyles.Get("WindowBottomResize"));
            Skin.Styles.Add(GUIStyles.Get("dragtab"));
            Skin.Styles.Add(GUIStyles.Get("IN LockButton"));
            Skin.Styles.Add(GUIStyles.Get("WinBtnClose"));
           
            foreach (GUIStyle item in GUI.skin)
            {
                Skin.Styles.Add(item);

            }
            EditorUtility.ClearProgressBar();
            ScriptableObj.Update<IFGUISKin>(Skin);
        }
    }
}
