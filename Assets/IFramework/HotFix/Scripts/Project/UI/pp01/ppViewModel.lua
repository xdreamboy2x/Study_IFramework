--*********************************************************************************
 --Author:         IFramework_Demo
 --Version:        0.0.1
 --UnityVersion:   2018.4.24f1
 --Date:           2020-07-16
 --Description:    Description
 --History:        2020-07-16--
--*********************************************************************************/
--super Fields 
--super Function 
----self:Subscribe( key,func )
----self:UnSubscribe(key,func)
----self:Invoke( key )



local ppViewModel=Class("ppViewModel",require("UI.ViewModel"))

--return ppViewModel's Fields By table
--Example return { myCount = 666 }
function ppViewModel:GetFieldTable()

end

function ppViewModel:OnDispose()

end

function ppViewModel:OnInitialize()

end

--View's  Event 
function ppViewModel:ListenViewEvent( code,... )

end

return ppViewModel
