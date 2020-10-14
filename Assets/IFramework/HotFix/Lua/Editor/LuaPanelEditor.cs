/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-07-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace IFramework.Hotfix.Lua
{
    [CustomEditor(typeof(LuaPanel))]
    public class LuaPanelEditor:Editor
	{
        private LuaPanel self { get { return target as LuaPanel; } }
        private ReorderableList list;
        private void OnEnable()
        {
            var p = this.serializedObject.FindProperty("fields");
            list = EditorTools.ReorderableListTool.Create(p,
                new List<EditorTools.ReorderableListTool.Column>()
                {
                    new EditorTools.ReorderableListTool.Column() {DisplayName="Name" ,Width=60},
                    new EditorTools.ReorderableListTool.Column() {DisplayName="GameObject" },
                }
                , 10);
        }
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            //  base.OnInspectorGUI();
            EditorTools.ReorderableListTool.Draw(list);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
