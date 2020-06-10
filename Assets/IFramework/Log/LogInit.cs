/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    [OnEnvironmentInit]
    static class LogInit
    {
        public const string StoName = "LogSetting";
        static LogInit()
        {
            Log.loger = new UnityLoger();
            LogSetting setting = Resources.Load<LogSetting>(StoName);
            if (setting == null) return;
            Log.enable_L = setting.enable_L;
            Log.enable_W = setting.enable_W;
            Log.enable_E = setting.enable_E;
            Log.lev_L = setting.lev_L;
            Log.lev_W = setting.lev_W;
            Log.enable = setting.enable;
            Log.lev_E = setting.lev_E;
        }
    }

}