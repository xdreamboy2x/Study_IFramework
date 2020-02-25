/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;

namespace IFramework_Demo
{
    public class SubArg : IEventArgs { }
    public class AddArg : IEventArgs { }

    public class Panel01View : TUIView_MVVM<Panel01ViewModel, Panel01>
	{
		protected override void BindProperty()
		{
			base.BindProperty();
            handler.BindProperty(() =>
            {
                Tpanel.Count_Text.text = Tcontext.count.ToString();

            });
			//ToDo
		}

		protected override void OnClear()
		{
		}

		protected override void OnLoad()
		{
            this.Tpanel.BTn_ADD.onClick.AddListener(() =>
            {
                this.message.Publish(this, 0, new AddArg());
            });
            this.Tpanel.BTn_SUB.onClick.AddListener(() =>
            {
                this.message.Publish(this, 0, new SubArg());

            });
        }

		protected override void OnPop(UIEventArgs arg)
		{
            Hide();
        }

		protected override void OnPress(UIEventArgs arg)
		{
            Hide();
		}

		protected override void OnTop(UIEventArgs arg)
		{
            Show();
		}

	}
}
