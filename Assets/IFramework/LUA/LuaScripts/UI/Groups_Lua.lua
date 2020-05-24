local Groups_lua = Class("Groups_lua")

--map: {name={ViewType=77,VMType=66},}
function Groups_lua:ctor(map)
   self.CS_instance=CS.IFramework.UI.Groups_lua()
   self.CS_instance:onDispose("+",function ()
						   		self:OnDispose()
						   	end )
   self.CS_instance:onSubscibe("+",function ( panel )
						   			self:OnSubscibe(panel)
						   		end )
   self.CS_instance:onUnSubscibe("+",function( panel )
					   				self:OnUnSubscibe(panel)
					   			end )
   self.CS_instance:onFindPanel("+",function ( name )
   									return self:OnFindPanel(name)
   								end )
   self.CS_instance:onInvokeListeners("+",function ( arg )
   										self:OnInvokeListeners(arg)
   									end )
	if map == nil then
		error("map could not be null ")
		return
	end
   	self.map=map
	self.groups={}
	self.FindGroup=function(name)
		return rawget(self.groups,name)
	end
end

function Groups_lua:OnDispose()

	for i, group in pairs(self.groups) do
		group:Dispose()
	end
	self.groups=nil
	self.CS_instance=nil
	self.map=nil
	
end

function Groups_lua:OnSubscibe(panel)
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
	local vvmGroup=VVMGroup(panel,viewModel,view,message)
	rawset(self.groups,panel.name,vvmGroup)
	view:OnLoad()
end
function Groups_lua:OnUnSubscibe(panel)
	local group= rawget(self.groups ,panel.name)
	if group ~=nil then
		group.view:OnClear()
		group:Dispose()
	end
end


function Groups_lua:OnFindPanel(name)
	local group=self.FindGroup(name)
	if group ~= nil then
		return group.panel
	end
	return nil
end


function Groups_lua:OnInvokeListeners(arg)
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


return Groups_lua
