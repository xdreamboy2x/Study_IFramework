local Convert={}
function Convert.ToNumber(value, base)
    local val = MathUtil.tonumber(value, base)
    if val==nil then
        return false
        end
    return true,val
end

function Convert.ToInt(value)
	local bo,val = Convert.ToNumber(value)
	if bo==true then
		return bo , math.Round(val)
		end
    return bo
end


-- 检查并尝试转换为布尔值，除了 nil 和 false，其他任何值都会返回 true
function Convert.ToBool(value)
    return (value ~= nil and value ~= false)
end

return Convert