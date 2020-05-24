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








function Awake()

	Define("Using",Using)
	Define("IsDefine",IsDefine)
	Define("Define",Define)

	Define("TableUtil",require("TableUtil"))
	Define("Convert",require("Convert"))
	Define("IOUtil",require("IOUtil"))
	Define("StringUtil",require("StringUtil"))
	Define("MathUtil",require("MathUtil"))
	Define("Util",require("Util"))


	Define("Class",require ("Class"))
	Define("Delegate",require("Classes.Delgate"))
	Define("ObservableValue",require("Classes.ObservableValue"))
	Define("ObservableObject",require("Classes.ObservableObject"))




     print(IsDefine("Util5"))
	 Lock_G()


	Define("Groups_lua",require("Groups_lua"))
    group=	Groups_lua()
CS.IFramework.Framework.env1.modules:FindModule(typeof(CS.IFramework.UI.UIModule)):SetGroups(group.CS_instance)

end

function Update()


end

function OnDispose()
	print("OnDispose")
end