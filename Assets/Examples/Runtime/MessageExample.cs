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
    public class MessageExample:MonoBehaviour,IMessagePublisher
	{
        public interface IPub : IMessagePublisher { }
        public class Pub:IPub
        { }
        public class Listenner : IMessageListener
        {
            public Listenner()
            {
                Framework.env1.modules.Message.Subscribe<IPub>(Listen);
                Framework.env1.modules.Message.UnSubscribe<IPub>(Listen);

                // Framework.env1.modules.Message.Subscribe<MessageExample>(this);
            }
            public void Listen( Type eventType, int code, IEventArgs args, params object[] param)
            {
                Log.L(string.Format("Recieve code {0} from type {1}", code,eventType)); 
            }
        }
        MessageModule Message;
        private void Start()
        {
            Message = MessageModule.CreatInstance<MessageModule>("","");
            Listenner listenner = new Listenner();

            Debug.Log(Framework.Version);

            Debug.Log(Message.Publish<Pub>( 100, null));

        }
       
    }
}
