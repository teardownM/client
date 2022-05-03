function tick()
    -- 159.89.18.92:7350
    if InputPressed('l') then
        LogGeneral('Connecting to server...')
        TDM_ConnectToServer("127.0.0.1", 7350)
	elseif InputPressed('k') then
		LogGeneral('Disconnecting from server...')
		TDM_DisconnectFromServer()
	elseif InputPressed('j') then
		LogGeneral('Getting players...')
    end
end

local InfoBoxSize = {
	240, 210
}

--------------- This is for debugging -----------------
-- The game crashes often so for now I'm overriding them
function TDM_GetServerName()
    return "Server Name"
end

function TDM_GetServerMap()
    return "Awesome Map"
end

function TDM_GetPlayerCount()
    return 1
end
-------------------------------------------------------

function draw()
	UiPush()
		UiFont("regular.ttf", 22)

		UiTranslate(10, UiHeight() - InfoBoxSize[2] - 10)

		UiPush()
			UiColor(0, 0, 0, 0.55)
			UiImageBox('ui/hud/infobox.png', InfoBoxSize[1], InfoBoxSize[2], 6, 6)
			UiColor(1, 1, 1, 1)
			UiTranslate(10, UiFontHeight())

			UiText('-- Keybindings --')
			UiTranslate(0, UiFontHeight())
			UiText('Join Server: L')
			UiTranslate(0, UiFontHeight())
			UiText('Disconnect: K')
			UiTranslate(0, UiFontHeight())
			UiText('List Players: J')

			UiTranslate(0, UiFontHeight())
			UiTranslate(0, UiFontHeight())
			UiText('-- Server Info --')

			UiTranslate(0, UiFontHeight())
			UiText('Server Name: ' .. TDM_GetServerName())
			UiTranslate(0, UiFontHeight())
			UiText('Server Map: ' .. TDM_GetServerMap())
			UiTranslate(0, UiFontHeight())
			UiText('Player Count: ' .. TDM_GetPlayerCount())
		UiPop()
    UiPop()
end