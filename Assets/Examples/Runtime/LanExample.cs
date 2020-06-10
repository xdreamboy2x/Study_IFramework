/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework_Demo
{
    [RequireComponent(typeof(Game))]
    public class LanExample:MonoBehaviour
	{
        class TestLoader : ILanPairGroup
        {
            public List<LanPair> Load()
            {
                return new List<LanPair>()
                {
                    new LanPair()
                    {
                        lan= SystemLanguage.Chinese,
                        value="哈哈",
                        key="77"
                    },
                    new LanPair()
                    {
                        lan= SystemLanguage.English,
                        value="haha",
                        key="77"
                    }
                };
            }
        }
        [LanguageKey]
        public string key="77";
        LanguageModule.LanObserver observer;
        LanguageModule mou;
        private void Start()
        {
            mou = Framework.env1.modules.CreateModule<LanguageModule>();
            mou.Load(new TestLoader());
            observer= mou.CreatObserver(key, SystemLanguage.English).ObserveEvent((lan, val) => { Log.E(val); });
        }
        int index;
        private void Update()
        {
            index = ++index % 40;
            mou.lan = (SystemLanguage)index;
        }
        
    }
}
