#include "utils.lua"

-- TODO: How do you get this function into a seperate file?
brake = 0
drive = 0
steer = 0

-- Runs on every tick and does the following:
--   Checks whether the local player is in a vehicle, if so, set varibles in the registry
--   that get read in the Sledge C# client.
--
--  Then, get the incoming data from Sledge and perform the vehicle transform update here.
--  The reason we do this with Lua rather than in Sledge is that currently we don't have access
--  to Lua functions in C# such as DriveVehicle().  
function VehicleManagerTick()
    local vehicleHandle = GetPlayerVehicle()

    if vehicleHandle ~= 0 then
        SetBool("playerInVehicle", true)
        SetInt("vehicleHandle", vehicleHandle)

        local vehicleTransform = GetVehicleTransform(vehicleHandle)

        SetString("vehicleMovement",
            vehicleHandle .. "," .. InputValue("up") .. "," .. InputValue("down") .. "," .. InputValue("left") .. "," ..
                InputValue("right") .. "," .. InputValue("handbrake"))

        SetString("vehicleTransform",
            round(vehicleTransform.pos[1], 3) .. ',' .. round(vehicleTransform.pos[2], 3) .. ',' ..
                round(vehicleTransform.pos[3], 3) .. ',' .. round(vehicleTransform.rot[1], 3) .. ',' ..
                round(vehicleTransform.rot[2], 3) .. ',' .. round(vehicleTransform.rot[3], 3) .. ',' ..
                round(vehicleTransform.rot[4], 3))

    else
        SetBool("playerInVehicle", false)
        SetInt("vehicleHandle", 0)
    end

    -- I = incoming data from C#
    local I_vehicleHandle = tonumber(GetString("I_vehicleHandle"))
    local I_vehicleDriveInput = explode(",", GetString("I_vehicleDriveInput"))
    local I_vehiclePos = explode(",", GetString("I_vehiclePos"))
    local I_vehicleQuat = explode(",", GetString("I_vehicleQuat"))

    if (table.getn(I_vehicleDriveInput) ~= 1) then
        local vehicleBody = GetVehicleBody(I_vehicleHandle)
        local vehicleBodyTransform = GetBodyTransform(vehicleBody)

        local newVehiclePos = {}
        newVehiclePos['x'] = tonumber(I_vehiclePos[1])
        newVehiclePos['y'] = vehicleBodyTransform.pos[2]
        newVehiclePos['z'] = tonumber(I_vehiclePos[3])

        local newVehicleQuat = {}
        newVehicleQuat['x'] = tonumber(I_vehicleQuat[1])
        newVehicleQuat['y'] = tonumber(I_vehicleQuat[2])
        newVehicleQuat['z'] = tonumber(I_vehicleQuat[3])
        newVehicleQuat['w'] = tonumber(I_vehicleQuat[4])

        DriveVehicle(I_vehicleHandle, 0, tonumber(I_vehicleDriveInput[3]), stringtoboolean[I_vehicleDriveInput[4]])

        SetBodyTransform(vehicleBody, Transform(Vec(newVehiclePos.x, newVehiclePos.y, newVehiclePos.z), Quat(
            newVehicleQuat.x, newVehicleQuat.y, newVehicleQuat.z, newVehicleQuat.w)))
    end
end

----------------------------

function init()
end



function tick(dt)
    VehicleManagerTick()

    --  if InputDown("up") or InputDown("down") or InputDown('left') or InputDown("right") or InputDown("jump") then
    --      local t = GetPlayerTransform()

    --      DebugPrint(dump(t.pos[1]))
    --      DebugPrint(dump(t.pos[2]))
    --      DebugPrint(dump(t.pos[3]))
    --  end

end

function update(dt)
end

function draw(dt)
end

