/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using IFramework.GUITool;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using System.Reflection;

namespace IFramework
{
    class EditorUtil
    {
        [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
        class TransformEditor : Editor
        {
            class CustomFloatField
            {
                private static readonly int Hint = "EditorTextField".GetHashCode();
                private static readonly Type EditorGUIType = typeof(EditorGUI);
                private static readonly Type RecycledTextEditorType = Assembly.GetAssembly(EditorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor");
                private static readonly Type[] ArgumentTypes =
                        {
                    RecycledTextEditorType,
                    typeof (Rect),
                    typeof (Rect),
                    typeof (int),
                    typeof (float),
                    typeof (string),
                    typeof (GUIStyle),
                    typeof (bool)
                    };

                private static readonly MethodInfo DoFloatFieldMethodInfo = EditorGUIType.GetMethod("DoFloatField", BindingFlags.NonPublic | BindingFlags.Static, null, ArgumentTypes, null);
                private static readonly FieldInfo FieldInfo = EditorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);
                private static readonly object RecycledEditor = FieldInfo.GetValue(null);

                public static float Draw(Rect draw, Rect drag, float value, GUIStyle style)
                {
                    var controlID = GUIUtility.GetControlID(Hint, FocusType.Keyboard, draw);
                    var parameters = new object[] { RecycledEditor, draw, drag, controlID, value, "g7", style, true };

                    return (float)DoFloatFieldMethodInfo.Invoke(null, parameters);
                }
            }
            class TransformRotationGUI
            {
                private object transformRotationGUI;
                private FieldInfo eulerAnglesField;
                private MethodInfo onEnableMethod;
                private MethodInfo rotationFieldMethod;
                private MethodInfo setLocalEulerAnglesMethod;

                private SerializedProperty property;

                public Vector3 eulerAngles { get { return (Vector3)eulerAnglesField.GetValue(transformRotationGUI); } }

                public TransformRotationGUI()
                {
                    if (transformRotationGUI == null)
                    {
                        var transformRotationGUIType = Type.GetType("UnityEditor.TransformRotationGUI,UnityEditor");
                        var transformType = typeof(Transform);
                        eulerAnglesField = transformRotationGUIType.GetField("m_EulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
                        onEnableMethod = transformRotationGUIType.GetMethod("OnEnable");
                        rotationFieldMethod = transformRotationGUIType.GetMethod("RotationField", new Type[] { });
                        setLocalEulerAnglesMethod = transformType.GetMethod("SetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);

                        transformRotationGUI = Activator.CreateInstance(transformRotationGUIType);
                    }
                }

                public void Initialize(SerializedProperty property, GUIContent content)
                {
                    this.property = property;
                    onEnableMethod.Invoke(transformRotationGUI, new object[] { property, content });
                }

                public void Draw()
                {
                    rotationFieldMethod.Invoke(transformRotationGUI, null);
                }

                public void Reset()
                {
                    var targets = property.serializedObject.targetObjects;
                    var parameters = new object[] { Vector3.zero, 0 };

                    Undo.RecordObjects(targets, "Reset Rotation");
                    foreach (var target in targets)
                        setLocalEulerAnglesMethod.Invoke(target, parameters);
                }
            }

            private class Content
            {
                public static readonly GUIContent Position = new GUIContent("Position", "The local position of this GameObject relative to the parent.");
                public static readonly GUIContent Rotation = new GUIContent("Rotation", "The local rotation of this Game Object relative to the parent.");
                public static readonly GUIContent Scale = new GUIContent("Scale", "The local scaling of this GameObject relative to the parent.");
                public static readonly GUIContent ResetPosition = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset the position.");
                public static readonly GUIContent ResetRotation = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset the rotation.");
                public static readonly GUIContent ResetScale = new GUIContent(EditorGUIUtility.IconContent("Refresh").image, "Reset the scale.");

                public const string FloatingPointWarning = "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.";
            }

            private class Styles
            {
                public static GUIStyle ResetButton;

                static Styles()
                {
                    ResetButton = new GUIStyle()
                    {
                        margin = new RectOffset(0, 0, 2, 0),
                        fixedWidth = 15,
                        fixedHeight = 15
                    };
                }
            }

            private class Properties
            {
                public SerializedProperty Position;
                public SerializedProperty Rotation;
                public SerializedProperty Scale;

                public Properties(SerializedObject obj)
                {
                    Position = obj.FindProperty("m_LocalPosition");
                    Rotation = obj.FindProperty("m_LocalRotation");
                    Scale = obj.FindProperty("m_LocalScale");
                }
            }

            private const int MaxDistanceFromOrigin = 100000;
            private const int ContentWidth = 60;

            private float xyRatio, xzRatio;

            private Properties properties;
            private TransformRotationGUI rotationGUI;

            private void OnEnable()
            {
                properties = new Properties(serializedObject);

                if (rotationGUI == null)
                    rotationGUI = new TransformRotationGUI();
                rotationGUI.Initialize(properties.Rotation, Content.Rotation);
            }

            public override void OnInspectorGUI()
            {
                if (!EditorGUIUtility.wideMode)
                {
                    EditorGUIUtility.wideMode = true;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
                }

                serializedObject.UpdateIfRequiredOrScript();

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(properties.Position, Content.Position);
                    using (new EditorGUI.DisabledGroupScope(properties.Position.vector3Value == Vector3.zero))
                        if (GUILayout.Button(Content.ResetPosition, Styles.ResetButton))
                            properties.Position.vector3Value = Vector3.zero;
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    rotationGUI.Draw();
                    using (new EditorGUI.DisabledGroupScope(rotationGUI.eulerAngles == Vector3.zero))
                        if (GUILayout.Button(Content.ResetRotation, Styles.ResetButton))
                        {
                            rotationGUI.Reset();
                            if (Tools.current == Tool.Rotate)
                            {
                                if (Tools.pivotRotation == PivotRotation.Global)
                                {
                                    Tools.handleRotation = Quaternion.identity;
                                    SceneView.RepaintAll();
                                }
                            }
                        }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(properties.Scale, Content.Scale);
                    using (new EditorGUI.DisabledGroupScope(properties.Scale.vector3Value == Vector3.one))
                        if (GUILayout.Button(Content.ResetScale, Styles.ResetButton))
                            properties.Scale.vector3Value = Vector3.one;
                }
                var dragRect = new Rect(16, 105, EditorGUIUtility.labelWidth - 10, 10);

                var e = Event.current;
                if (dragRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                {
                    var currentScale = properties.Scale.vector3Value;
                    xyRatio = currentScale.y / currentScale.x;
                    xzRatio = currentScale.z / currentScale.x;
                }

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    var c = GUI.color;
                    GUI.color = Color.clear;
                    var newScaleX = CustomFloatField.Draw(new Rect(), dragRect, properties.Scale.vector3Value.x, EditorStyles.numberField);

                    if (check.changed)
                    {
                        var currentScale = properties.Scale.vector3Value;

                        var delta = newScaleX - properties.Scale.vector3Value.x;

                        currentScale.x += delta;
                        currentScale.y += delta * xyRatio;
                        currentScale.z += delta * xzRatio;

                        properties.Scale.vector3Value = currentScale;
                    }

                    GUI.color = c;
                }

                serializedObject.ApplyModifiedProperties();

                EditorGUIUtility.labelWidth = 0;

                var transform = target as Transform;
                var position = transform.position;

                if
                (
                    Mathf.Abs(position.x) > MaxDistanceFromOrigin ||
                    Mathf.Abs(position.y) > MaxDistanceFromOrigin ||
                    Mathf.Abs(position.z) > MaxDistanceFromOrigin
                )
                    EditorGUILayout.HelpBox(Content.FloatingPointWarning, UnityEditor.MessageType.Warning);
            }

            [MenuItem("CONTEXT/Transform/Set Random Rotation")]
            private static void RandomRotation(MenuCommand command)
            {
                var transform = command.context as Transform;

                Undo.RecordObject(transform, "Set Random Rotation");
                transform.rotation = UnityEngine.Random.rotation;
            }

            [MenuItem("CONTEXT/Transform/Snap to Ground")]
            private static void SnapToGround(MenuCommand command)
            {
                var transform = command.context as Transform;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    Undo.RecordObject(transform, "SnGameed To Ground");
                    transform.position = hit.point;
                }
            }

            [MenuItem("CONTEXT/Transform/Snap to Ground (Physics)", true)]
            private static bool ValidateSnapToGroundPhysics(MenuCommand command)
            {
                return ((Transform)command.context).GetComponent<Collider>() != null;
            }
        }

        [CustomEditor(typeof(DefaultAsset))]
        class DefaultAssetEditorView : Editor, ILayoutGUIDrawer
        {
            private string _path;
            private bool _isdir;
            private void OnEnable()
            {
                _path = AssetDatabase.GetAssetPath(this.target);
                _isdir = _path.IsDirectory();
            }
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (_isdir)
                {
                    GUI.enabled = true;
                    this.Button(() => {
                        EditorUtility.OpenWithDefaultApp(_path);
                    }, "Open");

                }
            }
        }

        class WhenDeleteMonoScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            private static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
            {
                if (!AssetPath.EndsWith(".cs")) return AssetDeleteResult.DidNotDelete;
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetPath);
                if (monoScript == null) return AssetDeleteResult.DidNotDelete;
                Type spType = monoScript.GetClass();
                if (spType == null || !spType.IsSubclassOf(typeof(MonoBehaviour))) return AssetDeleteResult.DidNotDelete;

                MonoBehaviour[] monos = Object.FindObjectsOfType(spType) as MonoBehaviour[];
                monos.ForEach((m) =>
                {
                    Object.DestroyImmediate(m);
                });
                string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { @"Assets" });
                if (guids == null || guids.Length <= 0) return AssetDeleteResult.DidNotDelete;
                guids.ToList()
                     .ConvertAll((guid) => { return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)); })
                     .ForEach((o) => {
                         var cps = o.GetComponentsInChildren(spType, true);
                         if (cps != null && cps.Length > 0)
                         {
                             cps.ForEach((c) =>
                             {
                                 Object.DestroyImmediate(c, true);
                             });
                             AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(o));
                             EditorUtility.SetDirty(o);
                         }

                     });
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                return AssetDeleteResult.DidNotDelete;
            }

        }

        [OnEnvironmentInit]
        static class HierarchyOverride
        {
            private static bool _active;
            static HierarchyOverride()
            {
                Check();
            }

            private static void Check()
            {
                _active = EditorPrefs.GetBool("Hierarchy Extension");
                if (_active)
                    EditorEnv.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
                else
                    EditorEnv.hierarchyWindowItemOnGUI -= HierarchWindowOnGui;
                EditorApplication.RepaintHierarchyWindow();

            }

            [MenuItem(hierarchyOverridePath)]
            private static void Pan()
            {
                _active = !_active;
                if (_active)
                    EditorEnv.hierarchyWindowItemOnGUI += HierarchWindowOnGui;
                else
                    EditorEnv.hierarchyWindowItemOnGUI -= HierarchWindowOnGui;
                Menu.SetChecked(hierarchyOverridePath, _active);
                EditorPrefs.SetBool("Hierarchy Extension", _active);
                EditorApplication.RepaintHierarchyWindow();
            }

            private static void HierarchWindowOnGui(int instanceID, Rect selectionRect)
            {
                if (!_active) return;
                var obj = EditorUtility.InstanceIDToObject(instanceID);
                if (obj == null) return;
                var go = (GameObject)obj;
                GUILayout.BeginArea(selectionRect);
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        DrawRectIcon<MeshRenderer>(go);
                        DrawRectIcon<SkinnedMeshRenderer>(go);
                        DrawRectIcon<BoxCollider>(go);
                        DrawRectIcon<SphereCollider>(go);
                        DrawRectIcon<CapsuleCollider>(go);
                        DrawRectIcon<MeshCollider>(go);
                        DrawRectIcon<CharacterController>(go);
                        DrawRectIcon<Rigidbody>(go);
                        DrawRectIcon<Light>(go);
                        DrawRectIcon<Animator>(go);
                        DrawRectIcon<Animation>(go);
                        DrawRectIcon<Camera>(go);
                        DrawRectIcon<Projector>(go);
                        DrawRectIcon<ParticleSystem>(go);
                        DrawRectIcon<AudioSource>(go);
                        go.SetActive(EditorGUILayout.Toggle(go.activeSelf, GUILayout.Width(18)));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();



            }
            private static void DrawRectIcon<T>(GameObject go) where T : Component
            {
                if (go.GetComponent<T>() == null) return;
                var icon = EditorGUIUtility.ObjectContent(null, typeof(T)).image;
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
            }
        }


        private const string hierarchyOverridePath = "IFramework/Tool/HierarchyExtension";
        private const string copyAssetPathPath = "IFramework/Tool/Copy Path";
        private const string quitPath = "IFramework/Editor/Quit";
        private const string reOpenePath = "IFramework/Editor/ReOpen";

        private const string openDocPathPath = "IFramework/Folder/Open DocPath";
        private const string openStreamingPath = "IFramework/Folder/Open StreamingPath";
        private const string openDataPath = "IFramework/Folder/Open DataPath";
        private const string openConsoleLogPath = "IFramework/Folder/Open ConsoleLogPath";
        private const string openTemporaryCachePath = "IFramework/Folder/Open TemporaryCachePath";
        private const string findScriptPath = "CONTEXT/MonoBehaviour/IFramework.FindScript";



        [MenuItem(findScriptPath)]
        public static void FindScript(MenuCommand command)
        {
            Selection.activeObject = MonoScript.FromMonoBehaviour(command.context as MonoBehaviour);
        }

        [MenuItem(copyAssetPathPath)]
        public static void CopyAssetPath()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            GUIUtility.systemCopyBuffer = path;
        }
        [MenuItem(quitPath)]
        public static void Quit()
        {
            //  Environment.Exit(0);
            EditorApplication.Exit(0);
        }
        [MenuItem(reOpenePath)]
        public static void ReOpen()
        {
            EditorApplication.OpenProject(Application.dataPath.CombinePath("../"));
        }

        [MenuItem(openDocPathPath)]
        public static void OpenDoc()
        {
            OpenFloder(Application.persistentDataPath);
        }
        [MenuItem(openStreamingPath)]
        public static void OpenStreaming()
        {
            OpenFloder(Application.streamingAssetsPath);
        }
        [MenuItem(openDataPath)]
        public static void OpenDataPath()
        {
            OpenFloder(Application.dataPath);
        }
#if UNITY_2018
        [MenuItem(openConsoleLogPath)]
        public static void OpenConsoleLog()
        {
            OpenFloder(Application.consoleLogPath);
        }
#endif
        [MenuItem(openTemporaryCachePath)]
        public static void OpenTemporary()
        {
            OpenFloder(Application.temporaryCachePath);
        }





        public static void OpenFloder(string folder)
        {
            EditorUtility.OpenWithDefaultApp(folder);
        }
        public static EditorBuildSettingsScene[] ScenesInBuildSetting()
        {
            return EditorBuildSettings.scenes;
        }
        public static string[] GetScenesInBuildSetting()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            if (target == BuildTarget.Android)
            {
                return name + PlayerSettings.Android.bundleVersionCode + ".apk";
            }
            if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return name + PlayerSettings.Android.bundleVersionCode + ".exe";
            }
            if
#if UNITY_2017_3_OR_NEWER
            (target == BuildTarget.StandaloneOSX)
#else
            (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
#endif
            {
                return name + ".Game";
            }
            if (target == BuildTarget.iOS)
            {
                return "iOS";
            }
            return null;
            //if (target == BuildTarget.WebGL)
            //{
            //    return "/web";
            //}

        }
        public static GameObject CreatePrefab(GameObject source, string savePath)
        {
            GameObject goClone = GameObject.Instantiate(source);
            GameObject prefab;
#if UNITY_2018_1_OR_NEWER
            prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(goClone, savePath, InteractionMode.AutomatedAction);

#else
            prefab = PrefabUtility.CreatePrefab(savePath, goClone);
            prefab = PrefabUtility.ReplacePrefab(goClone, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
            AssetDatabase.ImportAsset(savePath);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            GameObject.DestroyImmediate(goClone);
            return prefab;
        }
    }

}
