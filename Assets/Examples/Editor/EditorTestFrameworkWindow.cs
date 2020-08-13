/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-16
 *Description:    Description
 *History:        2020-02-16--
*********************************************************************************/
using IFramework;
using IFramework.NodeAction;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace IFramework_Demo
{
    [EditorWindowCache]
	public class EditorTestFrameworkWindow:EditorWindow
	{
        private void OnEnable()
        {
            this.Sequence(EditorEnv.env)
                .Repeat((r) =>
                {
                    r.Event(() =>
                    {
                        color = Color.Lerp(color, 
                            new Color(Random.Range(0, 1f), 
                                        Random.Range(0, 1f), 
                                        Random.Range(0, 1f), 
                                        Random.Range(0.9f, 1f)),
                            (float)Framework.env0.deltaTime.TotalMilliseconds*2);
                        if (big)
                            rect = rect.Lerp(rect, new Rect(200, 200, 100, 100), (float)EditorEnv.env.deltaTime.TotalMilliseconds/2);
                        else
                            rect = rect.Lerp(rect, Rect.zero, (float)EditorEnv.env.deltaTime.TotalMilliseconds/2);
                        if (rect.size.x > 90)
                            big = false;
                        if (rect.size.x < 10)
                            big = true;
                    });
                },-1)
                .OnRecyle(()=> { Log.E("recy"); })
                .Run();
        }
        bool big;
        private Rect rect = new Rect(200, 200, 100, 100);
        private Color color;
        private void OnGUI()
        {
            GUI.color = color;
            GUI.Box(rect, "");
            Repaint();

        }
    }
}
