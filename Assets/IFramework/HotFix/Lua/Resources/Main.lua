--锁住 _G
local function Lock_G()
		setmetatable(_G, {
					    -- 控制新建全局变量
					    __newindex = function(_, k)
					        error("attempt to add a new value to global,key: " .. k, 2)
					    end,

					    -- 控制访问全局变量
					    -- __index = function(_, k)
					    --     error("attempt to index a global value,key: "..k,2)
					    -- end
						})
end

-- 全局函数
-- 用于声明全局变量
local function Define(name, value) rawset(_G, name, value) end
-- 是否定义全局变量
local function IsDefine( name ) return rawget(_G, name) ~= nil end

-- C# 调用方法
    -- Using("UnityEngine.KeyCode")
    -- print(KeyCode.Space)
local function Using( classname ,variant)
	local function Get( name )
		return load("return "..name)()
	end 
	if not variant then
		local a,b,c=string.find(classname,"[^%s]+%.([^%s]+)")
		if c then 
			variant =c
		else 
			variant=classname
		end
	end

   local _target = rawget(_G, variant)
   if _target == nil then
   	  _target =Get("CS."..classname)
   	  rawset(_G,variant,_target)
   end
   return _target
end



Using("UnityEngine")
Using("IFramework")
Using("IFramework.Launcher")

local updateEvent
local disposeEvent
local onFixUpdate
local onLateUpdate
local onApplicationFocus
local onApplicationPause

local function BindToUpdate(object,method)
	updateEvent:Subscribe(object,method)
end
local function BindToDispose(object,method)
	disposeEvent:Subscribe(object,method)
end
local function BindToFixUpdate(object,method)
	onFixUpdate:Subscribe(object,method)
end
local function BindToLateUpdate(object,method)
	onLateUpdate:Subscribe(object,method)
end
local function BindToOnApplicationFocus(object,method)
	onApplicationFocus:Subscribe(object,method)
end
local function BindToOnApplicationPause(object,method)
	onApplicationPause:Subscribe(object,method)
end

local function UnBindToDispose(object,method)
	disposeEvent:UnSubscribe(object,method)
end
local function UnBindToUpdate(object,method)
	updateEvent:UnSubscribe(object,method)
end
local function UnBindToFixUpdate(object,method)
	onFixUpdate:UnSubscribe(object,method)
end
local function UnBindToLateUpdate(object,method)
	onLateUpdate:UnSubscribe(object,method)
end
local function UnBindToOnApplicationFocus(object,method)
	onApplicationFocus:UnSubscribe(object,method)
end
local function UnBindToOnApplicationPause(object,method)
	onApplicationPause:UnSubscribe(object,method)
end



function Awake()

	Define("Log",require("Base.Log"))
	Define("Class",require ("Base.Class"))

	Lock_G()

	Define("Using",Using)
	Define("IsDefine",IsDefine)
	Define("Define",Define)

	Define("BindToUpdate",BindToUpdate)
	Define("BindToDispose",BindToDispose)
    Define("BindToLateUpdate",BindToLateUpdate)
	Define("BindToFixUpdate",BindToFixUpdate)
    Define("BindToOnApplicationFocus",BindToOnApplicationFocus)
	Define("BindToOnApplicationPause",BindToOnApplicationPause)

	Define("UnBindToUpdate",UnBindToUpdate)
	Define("UnBindToDispose",UnBindToDispose)
	Define("UnBindToLateUpdate",UnBindToLateUpdate)
	Define("UnBindToFixUpdate",UnBindToFixUpdate)
	Define("UnBindToOnApplicationFocus",UnBindToOnApplicationFocus)
	Define("UnBindToOnApplicationPause",UnBindToOnApplicationPause)
	
	Define("MathUtil",require("Base.MathUtil"))
	Define("TableUtil",require("Base.TableUtil"))
	Define("Convert",require("Base.Convert"))
	Define("IOUtil",require("Base.IOUtil"))
	Define("StringUtil",require("Base.StringUtil"))
	Define("Util",require("Base.Util"))
	
	Define("Delegate",require("Base.Classes.Delegate"))
	Define("ObservableValue",require("Base.Classes.ObservableValue"))
	Define("ObservableObject",require("Base.Classes.ObservableObject"))
	Define("Game",Launcher.instance.game)



	updateEvent=Delegate()
	disposeEvent=Delegate()
	onLateUpdate=Delegate()
    onFixUpdate=Delegate()
    onApplicationFocus=Delegate()
    onApplicationPause=Delegate()



	IFramework.Framework.BindEnvUpdate(Update,Launcher.env)
    Launcher.BindFixedUpdate(FixUpdate)
    Launcher.BindLateUpdate(LateUpdate)
    Launcher.BindOnApplicationFocus(OnApplicationFocus)
    Launcher.BindOnApplicationPause(OnApplicationPause)

	require("FixCsharp")
	require("GameLogic")
end




function Update()
	updateEvent:Invoke()
end
function FixUpdate()
    onFixUpdate:Invoke()
end
function LateUpdate()
     onLateUpdate:Invoke()
end
function OnApplicationFocus(foucus)
     onApplicationFocus:Invoke(foucus)
end
function OnApplicationPause(pause)
     onApplicationPause:Invoke(pause)
end









function OnDispose()
	updateEvent:Dispose()
	onLateUpdate:Dispose()
    onFixUpdate:Dispose()
    onApplicationFocus:Dispose()
    onApplicationPause:Dispose()



	disposeEvent:Invoke()
	disposeEvent:Dispose()




    Launcher.UnBindFixedUpdate(FixUpdate)
    Launcher.UnBindLateUpdate(LateUpdate)
    Launcher.UnBindOnApplicationFocus(OnApplicationFocus)
    Launcher.UnBindOnApplicationPause(OnApplicationPause)
	IFramework.Framework.UnBindEnvUpdate(Update,Launcher.env)
end

