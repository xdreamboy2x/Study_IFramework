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
using IFramework.Moudles.Message;
using UnityEngine;
namespace IFramework_Demo
{

    public class ObserverExample:MonoBehaviour,IPublisher
	{
        public interface IPub : IPublisher { }
        public class Pub:IPub
        { }
        public class Listenner : IObserver
        {
            public Listenner()
            {
                Framework.MessageMoudle.Subscribe<Pub>(this);

                //Framework.MessageMoudle.Subscribe<ObserverExample>(this);
            }
            public void Listen(IPublisher publisher, Type eventType, int code, IEventArgs args, params object[] param)
            {
                Log.L(Framework.MessageMoudle.name);
                Log.L(string.Format("Recieve code {0} from type {1}", code,eventType));
            }
        }
        private void Awake()
        {
            Framework.MessageMoudle = MessageMoudle.CreatInstance<MessageMoudle>();
            Framework.Init();
            Framework.MessageMoudle.BindFramework();
            Listenner listenner = new Listenner();
          Debug.Log(  Framework.MessageMoudle.Publish<IPub>( 100, null));

            Framework.MessageMoudle.DelayPublish(this, 100, null);
        }
        private void Update()
        {
            //Framework.Update();


        }
    }
}
