

using UnityEngine;

namespace IFramework.Hotfix.Lua
{
    public class AsyncTest : MonoBehaviour
    {

        void Start()
        {
            XLuaEnv.DoString("require 'async_test'");
        }

    }
}

