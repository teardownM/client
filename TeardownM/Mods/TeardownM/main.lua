local bDrawMouse = false

function tick()
	if InputPressed('f1') then
		bDrawMouse = not bDrawMouse
	end
end

function draw()
	if bDrawMouse then UiMakeInteractive() end
end

-----------------------------------------
------------- UI Functions --------------
-----------------------------------------
function AddToast(eType, sMessage, iDuration)
	--[[ Adds a message to the screen, called from C# ]]--
end

-----------------------------------------
--------------- Callbacks ---------------
-----------------------------------------
function OnConnecting()
	-- TDM_SetRichPresence(ERichPresence.Connecting)
end

function OnConnect()
	-- TDM_SetRichPresence(ERichPresence.Connected)
end

function OnDisconnect()
	-- TDM_SetRichPresence(ERichPresence.MainMenu)
end

function OnSpawn(iClientID)

end

function OnDeath(iClientID, iKillerID, sWeapon)

end

function OnMessage(iClientID, sMessage)

end

function OnShoot(iClientID, sWeapon, iPosX, iPosY)

end