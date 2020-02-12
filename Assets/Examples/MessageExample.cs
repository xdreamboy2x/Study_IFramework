/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-16
 *Description:    Description
 *History:        2019-12-16--
*********************************************************************************/
using System;
using IFramework;
using IFramework.Modules.Message;
using UnityEngine;
namespace IFramework_Demo
{
    [RequireComponent(typeof(APP))]
    public class MessageExample:MonoBehaviour,IPublisher
	{
        public interface IPub : IPublisher { }
        public class Pub:IPub
        { }
        public class Listenner : IObserver
        {
            public Listenner()
            {
                Framework.env1.modules.Message.Subscribe<Pub>(this);
                Framework.env1.modules.Message.Subscribe<MessageExample>(this);
            }
            public void Listen(IPublisher publisher, Type eventType, int code, IEventArgs args, params object[] param)
            {
                Log.L(string.Format("Recieve code {0} from type {1}", code,eventType));
            }
        }
        MessageModule Message { get { return Framework.env1.modules.Message; } set { Framework.env1.modules.Message = value; } }
        private void Start()
        {
            Message = Framework.env1.modules.CreateModule<MessageModule>();
            Listenner listenner = new Listenner();

            Debug.Log(Message.Publish<IPub>( 100, null));

        }
       
    }
}
