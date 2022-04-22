require("utils")








function vehicleManagerTick()
   local vehicleHandle = GetPlayerVehicle()


   if vehicleHandle ~= 0 then
      SetBool("playerInVehicle", true)
      SetInt("vehicleHandle", vehicleHandle)

      local vehicleTransform = GetVehicleTransform(vehicleHandle)

      SetString("vehicleMovement", arrayToString({ vehicleHandle, InputValue("up"), InputValue("down"), InputValue("left"), InputValue("right"), InputValue("handbrake") }))
      SetString("vehicleTransform", arrayToString({ round(vehicleTransform.pos[1], 3), round(vehicleTransform.pos[2], 3), round(vehicleTransform.pos[3], 3), round(vehicleTransform.rot[1], 3), round(vehicleTransform.rot[2], 3), round(vehicleTransform.rot[3], 3), round(vehicleTransform.rot[4], 3) }))
   else
      SetBool("playerInVehicle", false)
      SetInt("vehicleHandle", 0)
   end

   local DriveInput = {}






   local I_vehicleHandle = tonumber(GetString("I_vehicleHandle"))
   local I_vehicleDriveInput = explode(GetString("I_vehicleDriveInput"))
   local I_vehiclePos = explode(GetString("I_vehiclePos"))
   local I_vehicleQuat = explode(GetString("I_vehicleQuat"))

   if (#I_vehicleDriveInput ~= 1) then
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

      DriveVehicle(I_vehicleHandle, tonumber(I_vehicleDriveInput[1]), tonumber(I_vehicleDriveInput[2]), stringToBool(I_vehicleDriveInput[3]))

      SetBodyTransform(vehicleBody, Transform(Vec(newVehiclePos.x, newVehiclePos.y, newVehiclePos.z), Quat(
      newVehicleQuat.x, newVehicleQuat.y, newVehicleQuat.z, newVehicleQuat.w)))
   end
end
