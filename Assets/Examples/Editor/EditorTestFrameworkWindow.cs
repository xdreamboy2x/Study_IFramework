/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-16
 *Description:    Description
 *History:        2020-02-16--
*********************************************************************************/
using IFramework;
using IFramework.Modules.NodeAction;
using UnityEditor;
using UnityEngine;
namespace IFramework_Demo
{
    [EditorWindowCache]
	public class EditorTestFrameworkWindow:EditorWindow
	{
        private void OnEnable()
        {
            this.Sequence(0)
                .Repeat((r) =>
                {
                    r.Event(() =>
                    {
                        color = Color.Lerp(color, 
                            new Color(Random.Range(0.5f, 1f), 
                                        Random.Range(0.5f, 1f), 
                                        Random.Range(0.5f, 1f), 
                                        Random.Range(0.9f, 1f)),
                            (float)Framework.env0.deltaTime.TotalMilliseconds);
                        if (big)
                            rect = rect.Lerp(rect, new Rect(100, 100, 100, 100), (float)Framework.env0.deltaTime.TotalMilliseconds);
                        else
                            rect = rect.Lerp(rect, Rect.zero, (float)Framework.env0.deltaTime.TotalMilliseconds);
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
        private Rect rect = new Rect(100, 100, 100, 100);
        private Color color;
        private void OnGUI()
        {
            GUI.color = color;
            GUI.Box(rect, "");
            Repaint();
        }
    }
}
