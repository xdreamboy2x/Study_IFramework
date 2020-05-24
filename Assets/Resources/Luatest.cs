using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IFramework;
using IFramework.Lua;
using System;
using XLua;


public class Luatest : MonoBehaviour,IXLuaLoader
{
    public TextAsset luaScript;

    public TextAsset group;

    LuaTable scriptEnv;
    public byte[] load(ref string filepath)
    {
        if (filepath== "Groups_lua")
       return group.bytes;
        return null;
    }

    private void LuaDispose()
    {

        var dis = scriptEnv.Get<Action>("OnDispose");
        dis.Invoke();
        dis = null;

    }
   
    private void Start()
    {
        XLuaEnv.AddLoader(this);
        XLuaEnv.onDispose += LuaDispose;
         scriptEnv = XLuaEnv.GetTable(luaScript, "LuaTestScript");
        scriptEnv.Set("this", this);
        var  awake= scriptEnv.Get<Action>("Awake");
         awake.Invoke();
        awake = null;
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }


   
}