local bDrawMouse = false

function tick()
	if InputPressed('f1') then
		bDrawMouse = not bDrawMouse
	end
end

-----------------------------------------
------------- UI Functions --------------
-----------------------------------------
function AddToast(eType, sMessage, iDuration)
	-- This function will add a toast to the screen (a toast is a small message that appears on the screen for a certain amount of time)
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






































-- local InfoBoxSize = {
-- 	240, 210
-- }

function draw()
	if bDrawMouse then
		UiMakeInteractive()
	end

	-- UiPush()
	-- 	UiFont("regular.ttf", 22)

	-- 	UiTranslate(10, UiHeight() - InfoBoxSize[2] - 10)

	-- 	UiPush()
	-- 		UiColor(0, 0, 0, 0.55)
	-- 		UiImageBox('ui/hud/infobox.png', InfoBoxSize[1], InfoBoxSize[2], 6, 6)
	-- 		UiColor(1, 1, 1, 1)
	-- 		UiTranslate(10, UiFontHeight())

	-- 		UiText('-- Keybindings --')
	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('Join Server: L')
	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('Disconnect: K')
	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('List Players: J')

	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('-- Server Info --')

	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('Server Name: ' .. TDM_GetServerName())
	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('Server Map: ' .. TDM_GetServerMap())
	-- 		UiTranslate(0, UiFontHeight())
	-- 		UiText('Player Count: ' .. TDM_GetPlayerCount())
	-- 	UiPop()
    -- UiPop()
end

function OnConnecting(serverInfo)

end

function OnConnect(serverInfo)
	
end