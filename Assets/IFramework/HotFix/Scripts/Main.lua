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


local function Log(fmt,...)
	local info = debug.getinfo(2,"S")
    local source=info.source
	info = debug.getinfo(2,"l")
	print(string.format(fmt,...),"\nLine: "..info.currentline.."\n at  "..source)
end
local function LogError(fmt,...)
	local info = debug.getinfo(2,"S")
	local source=info.source
	info = debug.getinfo(2,"l")
	error(string.format(fmt,...).."\nLine: "..info.currentline.."\n at  "..source)
end


Using("UnityEngine")
Using("IFramework")
Using("IFramework.Game")


function Awake()
	Define("Using",Using)
	Define("IsDefine",IsDefine)
	Define("Define",Define)
	Define("Log",Log)
	Define("LogError",LogError)
	
	Define("MathUtil",require("Base.MathUtil"))

	Define("TableUtil",require("Base.TableUtil"))
	Define("Convert",require("Base.Convert"))
	Define("IOUtil",require("Base.IOUtil"))
	Define("StringUtil",require("Base.StringUtil"))
	Define("Util",require("Base.Util"))
	
	Define("Class",require ("Base.Class"))
	Define("Delegate",require("Base.Classes.Delegate"))
	Define("ObservableValue",require("Base.Classes.ObservableValue"))
	Define("ObservableObject",require("Base.Classes.ObservableObject"))
	
	Lock_G()
	IFramework.Framework.BindEnvUpdate(Update,Game.env)
	require("Custom.FixCsharp")
	require("Custom.GameLogic")

end





function Update()

	Log("Update")

end

function OnDispose()
	Log("OnDispose")
	IFramework.Framework.UnBindEnvUpdate(Update,Game.env)

end

