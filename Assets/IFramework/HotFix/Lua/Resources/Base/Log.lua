local Log = {}

function Log.L(message)
    local info = debug.getinfo(2,"S")
    local source=info.source
    info = debug.getinfo(2,"l")
    local message="LUA :".. message .."\nLine: "..info.currentline.."\n at  "..source
    CS.IFramework.Log.L(message)
end
function Log.W(message)
    local info = debug.getinfo(2,"S")
    local source=info.source
    info = debug.getinfo(2,"l")
    local message="LUA :".. message.."\nLine: "..info.currentline.."\n at  "..source
    CS.IFramework.Log.W(message)
end
function Log.E(...)
    local info = debug.getinfo("S")
    local source=info.source
    info = debug.getinfo(2,"l")
    local message="LUA :"..message .."\nLine: "..info.currentline.."\n at  "..source
    CS.IFramework.Log.E(message)
end


function Log.LF(fmt,...)
    local info = debug.getinfo(2,"S")
    local source=info.source
    info = debug.getinfo(2,"l")
    local message="LUA :"..string.format(fmt,...).."\nLine: "..info.currentline.."\n at  "..source
    CS.IFramework.Log.L(message)
end
function Log.WF(fmt,...)
    local info = debug.getinfo(2,"S")
    local source=info.source
    info = debug.getinfo(2,"l")
    local message="LUA :"..string.format(fmt,...).."\nLine: "..info.currentline.."\n at  "..source
    CS.IFramework.Log.W(message)
end
function Log.EF(fmt,...)
    local info = debug.getinfo("S")
    local source=info.source
    info = debug.getinfo(2,"l")
    local message="LUA :"..string.format(fmt,...).."\nLine: "..info.currentline.."\n at  "..source
    CS.IFramework.Log.E(message)
end




setmetatable(Log,{
    __index=function (t,k)
        if k=="enable" then
            return CS.IFramework.Log.enable
        elseif k=="enable_L" then
            return CS.IFramework.Log.enable_L
        elseif k=="enable_W" then
            return CS.IFramework.Log.enable_W
        elseif k=="enable_E" then
            return CS.IFramework.Log.enable_E
        else
            error("No such Field in Log :"..tostring(k))
        end
    end
,
    __newindex=function(t,k,v)
        if k=="enable" then
            CS.IFramework.Log.enable =v
        elseif k=="enable_L" then
            CS.IFramework.Log.enable_L =v
        elseif k=="enable_W" then
            CS.IFramework.Log.enable_W =v
        elseif k=="enable_E" then
            CS.IFramework.Log.enable_E =v
        else
            error("No such Field in Log :"..tostring(k))
        end
    end
})

return Log