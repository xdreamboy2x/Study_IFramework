Log.L("Start Game Logic ")

local _group=require("UI.LuaGroups")()
local _uimodule=  Game.unityModules.UI

_uimodule:SetGroups(_group:SetMap(require("Custom.UI.UIMap_MVVM")))
