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





Using("IFramework")
Using("IFramework.Game")


function Awake()
	IFramework.Lua.XLuaEnv.AddLoader(IFramework.Lua.AssetBundleLoader())
	Define("Using",Using)
	Define("IsDefine",IsDefine)
	Define("Define",Define)
	
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




     print(IsDefine("Util5"))
	 Lock_G()

	Groups_lua=	require("UI.luaGroups")


	local function Test()
		local Panel01View=Class("Panel01View",require("UI.UIView"))
		function Panel01View:BindProperty()
           -- print(self.context)
			self.context:Subscribe("count",function()
				--print("binde")
				--print(self.context.count)
				self.panel.Count_Text.text=tostring(self.context.count) 
			end )

		end

		function Panel01View:Dispose()

		end


		function Panel01View:OnLoad(  )
			self.panel.BTn_ADD.onClick:AddListener(function ()
				self:PublishViewEvent("+")
			end)
			self.panel.BTn_SUB.onClick:AddListener(function ()
				self:PublishViewEvent("-")
			end)
		end
		function Panel01View:OnTop( arg )
			self:Show()
		end
		function Panel01View:OnPress( arg )
			self:Hide()
		end
		function Panel01View:OnPop( arg )
			self:Hide()
		end
		function Panel01View:OnClear(  )
			print("Clear")
			self.panel.BTn_ADD.onClick:RemoveAllListeners()
			self.panel.BTn_SUB.onClick:RemoveAllListeners()

		end
		return Panel01View
	end
	local function Test2()
		local Panel01ViewModel=Class("Panel01ViewModel",require("UI.ViewModel"))
		function Panel01ViewModel:OnDispose()

		end
		function Panel01ViewModel:GetFieldTable()
         	return {count=100}
		end
		function Panel01ViewModel:OnInitialize()
			--print(self.count)
		   --self.count=100
		end
		function Panel01ViewModel:ListenViewEvent( code,... )

			if code == "-" then
				self.count=self.count-1
				print(code=="-")

			else

				self.count=self.count+1
			end
			
		end
		return Panel01ViewModel
	end
	
    group=	Groups_lua()

	Panel01View=Test()
	Panel01ViewModel=Test2()
	--map: {name={ViewType=77,VMType=66},}

	map={Panel01={ViewType=Panel01View,VMType=Panel01ViewModel}}
    Game.modules:FindModule(typeof(IFramework.UI.UIModule)):SetGroups(group:SetMap(map))

end





function Update()


end

function OnDispose()
	print("OnDispose")
end