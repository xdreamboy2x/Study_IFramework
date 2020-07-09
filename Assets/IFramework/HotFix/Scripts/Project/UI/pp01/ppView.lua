--*********************************************************************************
 --Author:         IFramework_Demo
 --Version:        0.0.1
 --UnityVersion:   2018.4.24f1
 --Date:           2020-07-16
 --Description:    Description
 --History:        2020-07-16--
--*********************************************************************************/

--super Fields 
----self.message : publish Event
----self.context : ViewModel
----self.panel :  UIpanel From C#
--super Function 
----self:PublishViewEvent(code,...)


local ppView=Class("ppView",require("UI.UIView"))

--Bind ViewModel Fields
function ppView:BindProperty()

end

function ppView:Dispose()

end

function ppView:OnLoad()

end

function ppView:OnTop( arg )

end

function ppView:OnPress( arg )

end

function ppView:OnPop( arg )

end

function ppView:OnClear()

end

return ppView
