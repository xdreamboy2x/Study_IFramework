local luaGroups = Class("luaGroups")
local VVMGroup =require("UI.VVMGroup")
function luaGroups:ctor()
	self.onDispose=function ()
		self:OnDispose()
	end
	self.onSubscibe=function ( panel )
		self:OnSubscibe(panel)
	end
	self.onUnSubscibe=function( panel )
		self:OnUnSubscibe(panel)
	end
	self.onFindPanel=function ( name )
		return self:OnFindPanel(name)
	end
	self.onInvokeListeners=function ( arg )
		self:OnInvokeListeners(arg)
	end
	self.groups={}
	self.FindGroup=function(name)
		return rawget(self.groups,name)
	end
end
--map: {name={ViewType=77,VMType=66},}
function luaGroups:SetMap(map)
	if map == nil then
		error("map could not be null ")
		return
	end
	self.CS_instance=IFramework.Lua.luaGroups()
	self.CS_instance:onDispose("+",self.onDispose)
	self.CS_instance:onSubscibe("+",self.onSubscibe)
	self.CS_instance:onUnSubscibe("+",self.onUnSubscibe)
	self.CS_instance:onFindPanel("+",self.onFindPanel)
	self.CS_instance:onInvokeListeners("+",self.onInvokeListeners)

	self.map=map
	return self.CS_instance
end

-- 以下方法私有

function luaGroups:OnDispose()

	for i, group in pairs(self.groups) do
		group.view:OnClear()
		group:Dispose()
	end

	self.CS_instance:onDispose("-",self.onDispose)
	self.CS_instance:onSubscibe("-",self.onSubscibe)
	self.CS_instance:onUnSubscibe("-",self.onUnSubscibe)
	self.CS_instance:onFindPanel("-",self.onFindPanel)
	self.CS_instance:onInvokeListeners("-",self.onInvokeListeners)
	self.groups=nil
	self.CS_instance=nil
	self.map=nil
end

function luaGroups:OnSubscibe(panel)
	local vvmType = rawget(self.map,panel.name)
	if(vvmType==nil) then
		error("not find vvm type with name : "..panel.name)
		return
	end
	if rawget(self.groups,panel.name) ~=nil then
		error("same name with panel  "..panel.name)
		return
	end

	local message=Delegate()
	local viewModel=vvmType.VMType(message)
	local view=vvmType.ViewType(message,viewModel,panel)
	local vvmGroup=VVMGroup(panel,view,viewModel,message)
	rawset(self.groups,panel.name,vvmGroup)
	view:OnLoad()
end
function luaGroups:OnUnSubscibe(panel)
	local group= rawget(self.groups ,panel.name)
	if group ~=nil then
		group.view:OnClear()
		group:Dispose()
	end
end


function luaGroups:OnFindPanel(name)
	local group=self.FindGroup(name)
	if group ~= nil then
		return group.panel
	end
	return nil
end
function luaGroups:OnInvokeListeners(arg)
	if arg.pressPanel ~=nil then
		self.FindGroup(arg.pressPanel.name).view:OnPress(arg)
	end
	if arg.popPanel ~=nil then
		self.FindGroup(arg.popPanel.name).view:OnPop(arg)
	end
	if arg.curPanel  ~=nil then
		self.FindGroup(arg.curPanel .name).view:OnTop(arg)
	end
end



return luaGroups
