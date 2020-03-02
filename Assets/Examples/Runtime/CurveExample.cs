/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-03-03
 *Description:    Description
 *History:        2020-03-03--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFramework;

namespace IFramework_Demo
{
	public class CurveExample : MonoBehaviour
	{
        private ValueCurve c;

        private void Start()
        {
            c = new ValueCurve(new List<Point2>()
                {
                    new  Point2(0,-10),
                    new  Point2(0.3f,0.6f),

                    new Point2(0.5f,0.7f),
                    new Point2(0.7f,0.9f),
                    new Point2(1,1)
                });
        }
        private void Update()
        {
            for (int i = 0; i < c.count - 1; i++)
            {
                Vector2 v = new Vector2(c.GetStep(i).x, c.GetStep(i).y);
                Vector2 v2 = new Vector2(c.GetStep(i + 1).x, c.GetStep(i + 1).y);

                Debug.DrawLine(v, v2);

            }
        }
    }
}
