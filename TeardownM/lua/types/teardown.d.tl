
--[[
Teardown uses Lua version 5.1 as scripting language.
The Lua 5.1 reference manual can be found here. @https://www.lua.org/manual/5.1/

Created with HypnoTox's Teardown API Stub Generator, available at https://github.com/hypnotox/teardown-api-stub-generator
]]

--[[ Classes ]]

global type VectorType = {number, number, number}
global type QuaternionType = {number, number, number, number}

global record TransformType
    pos: VectorType
    rot: QuaternionType
end

--[[ Functions ]]

---@see @https://www.teardowngame.com/modding/api.html#GetIntParam
global GetIntParam: function(name: string, default: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetFloatParam
global GetFloatParam: function(name: string, default: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetBoolParam
global GetBoolParam: function(name: string, default: boolean): boolean

---@see @https://www.teardowngame.com/modding/api.html#GetStringParam
global GetStringParam: function(name: string, default: string): string

---@see @https://www.teardowngame.com/modding/api.html#GetVersion
global GetVersion: function(): string

---@see @https://www.teardowngame.com/modding/api.html#HasVersion
global HasVersion: function(version: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#GetTime
global GetTime: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetTimeStep
global GetTimeStep: function(): number

---@see @https://www.teardowngame.com/modding/api.html#InputLastPressedKey
global InputLastPressedKey: function(): string

---@see @https://www.teardowngame.com/modding/api.html#InputPressed
global InputPressed: function(input: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#InputReleased
global InputReleased: function(input: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#InputDown
global InputDown: function(input: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#InputValue
global InputValue: function(input: string): number

---@see @https://www.teardowngame.com/modding/api.html#SetValue
global SetValue: function(variable: string, value: number, transition: string, time: number)

---@see @https://www.teardowngame.com/modding/api.html#PauseMenuButton
global PauseMenuButton: function(title: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#StartLevel
global StartLevel: function(mission: string, path: string, layers: string, passThrough: boolean)

---@see @https://www.teardowngame.com/modding/api.html#SetPaused
global SetPaused: function(paused: boolean)

---@see @https://www.teardowngame.com/modding/api.html#Restart
global Restart: function()

---@see @https://www.teardowngame.com/modding/api.html#Menu
global Menu: function()

---@see @https://www.teardowngame.com/modding/api.html#ClearKey
global ClearKey: function(key: string)

---@see @https://www.teardowngame.com/modding/api.html#ListKeys
global ListKeys: function(parent: string): {number}

---@see @https://www.teardowngame.com/modding/api.html#HasKey
global HasKey: function(key: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#SetInt
global SetInt: function(key: string, value: number)

---@see @https://www.teardowngame.com/modding/api.html#GetInt
global GetInt: function(key: string): number

---@see @https://www.teardowngame.com/modding/api.html#SetFloat
global SetFloat: function(key: string, value: number)

---@see @https://www.teardowngame.com/modding/api.html#GetFloat
global GetFloat: function(key: string): number

---@see @https://www.teardowngame.com/modding/api.html#SetBool
global SetBool: function(key: string, value: boolean)

---@see @https://www.teardowngame.com/modding/api.html#GetBool
global GetBool: function(key: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#SetString
global SetString: function(key: string, value: string)

---@see @https://www.teardowngame.com/modding/api.html#GetString
global GetString: function(key: string): string

---@see @https://www.teardowngame.com/modding/api.html#Vec
global Vec: function(x: number, y: number, z: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecCopy
global VecCopy: function(org: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecLength
global VecLength: function(vec: VectorType): number

---@see @https://www.teardowngame.com/modding/api.html#VecNormalize
global VecNormalize: function(vec: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecScale
global VecScale: function(vec: VectorType, scale: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecAdd
global VecAdd: function(a: VectorType, b: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecSub
global VecSub: function(a: VectorType, b: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecDot
global VecDot: function(a: VectorType, b: VectorType): number

---@see @https://www.teardowngame.com/modding/api.html#VecCross
global VecCross: function(a: VectorType, b: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#VecLerp
global VecLerp: function(a: VectorType, b: VectorType, t: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#Quat
global Quat: function(x: number, y: number, z: number, w: number): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#QuatCopy
global QuatCopy: function(org: QuaternionType): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#QuatAxisAngle
global QuatAxisAngle: function(axis: QuaternionType, angle: number): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#QuatEuler
global QuatEuler: function(x: number, y: number, z: number): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#GetQuatEuler
global GetQuatEuler: function(quat: QuaternionType): number, number, number

---@see @https://www.teardowngame.com/modding/api.html#QuatLookAt
global QuatLookAt: function(eye: VectorType, target: VectorType): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#QuatSlerp
global QuatSlerp: function(a: QuaternionType, b: QuaternionType, t: number): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#QuatRotateQuat
global QuatRotateQuat: function(a: QuaternionType, b: QuaternionType): QuaternionType

---@see @https://www.teardowngame.com/modding/api.html#QuatRotateVec
global QuatRotateVec: function(a: QuaternionType, vec: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#Transform
global Transform: function(pos: VectorType, rot: QuaternionType): TransformType

---@see @https://www.teardowngame.com/modding/api.html#TransformCopy
global TransformCopy: function(org: TransformType): TransformType

---@see @https://www.teardowngame.com/modding/api.html#TransformToParentTransform
global TransformToParentTransform: function(parent: TransformType, child: TransformType): TransformType

---@see @https://www.teardowngame.com/modding/api.html#TransformToLocalTransform
global TransformToLocalTransform: function(parent: TransformType, child: TransformType): TransformType

---@see @https://www.teardowngame.com/modding/api.html#TransformToParentVec
global TransformToParentVec: function(t: TransformType, v: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#TransformToLocalVec
global TransformToLocalVec: function(t: TransformType, v: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#TransformToParentPoint
global TransformToParentPoint: function(t: TransformType, p: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#TransformToLocalPoint
global TransformToLocalPoint: function(t: TransformType, p: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#SetTag
global SetTag: function(handle: number, tag: string, value: string)

---@see @https://www.teardowngame.com/modding/api.html#RemoveTag
global RemoveTag: function(handle: number, tag: string)

---@see @https://www.teardowngame.com/modding/api.html#HasTag
global HasTag: function(handle: number, tag: string): boolean

---@see @https://www.teardowngame.com/modding/api.html#GetTagValue
global GetTagValue: function(handle: number, tag: string): string

---@see @https://www.teardowngame.com/modding/api.html#GetDescription
global GetDescription: function(handle: number): string

---@see @https://www.teardowngame.com/modding/api.html#SetDescription
global SetDescription: function(handle: number, description: string)

---@see @https://www.teardowngame.com/modding/api.html#Delete
global Delete: function(handle: number)

---@see @https://www.teardowngame.com/modding/api.html#IsHandleValid
global IsHandleValid: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#FindBody
global FindBody: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindBodies
global FindBodies: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetBodyTransform
global GetBodyTransform: function(handle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#SetBodyTransform
global SetBodyTransform: function(handle: number, transform: TransformType)

---@see @https://www.teardowngame.com/modding/api.html#GetBodyMass
global GetBodyMass: function(handle: number): number

---@see @https://www.teardowngame.com/modding/api.html#IsBodyDynamic
global IsBodyDynamic: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#SetBodyDynamic
global SetBodyDynamic: function(handle: number, dynamic: boolean)

---@see @https://www.teardowngame.com/modding/api.html#SetBodyVelocity
global SetBodyVelocity: function(handle: number, velocity: VectorType)

---@see @https://www.teardowngame.com/modding/api.html#GetBodyVelocity
global GetBodyVelocity: function(handle: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#GetBodyVelocityAtPos
global GetBodyVelocityAtPos: function(handle: number, pos: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#SetBodyAngularVelocity
global SetBodyAngularVelocity: function(handle: number, angVel: VectorType)

---@see @https://www.teardowngame.com/modding/api.html#GetBodyAngularVelocity
global GetBodyAngularVelocity: function(handle: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#IsBodyActive
global IsBodyActive: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#SetBodyActive
global SetBodyActive: function(handle: number, active: boolean)

---@see @https://www.teardowngame.com/modding/api.html#ApplyBodyImpulse
global ApplyBodyImpulse: function(handle: number, position: VectorType, impulse: VectorType)

---@see @https://www.teardowngame.com/modding/api.html#GetBodyShapes
global GetBodyShapes: function(handle: number): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetBodyVehicle
global GetBodyVehicle: function(body: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetBodyBounds
global GetBodyBounds: function(handle: number): VectorType, VectorType

---@see @https://www.teardowngame.com/modding/api.html#GetBodyCenterOfMass
global GetBodyCenterOfMass: function(handle: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#IsBodyVisible
global IsBodyVisible: function(handle: number, maxDist: number, rejectTransparent: boolean): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsBodyBroken
global IsBodyBroken: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsBodyJointedToStatic
global IsBodyJointedToStatic: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#DrawBodyOutline
global DrawBodyOutline: function(handle: number, r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#DrawBodyHighlight
global DrawBodyHighlight: function(handle: number, amount: number)

---@see @https://www.teardowngame.com/modding/api.html#GetBodyClosestPoint
global GetBodyClosestPoint: function(body: number, origin: VectorType): boolean, VectorType, VectorType, number

---@see @https://www.teardowngame.com/modding/api.html#ConstrainVelocity
global ConstrainVelocity: function(bodyA: number, bodyB: number, point: VectorType, dir: VectorType, relVel: number, min: number, max: number)

---@see @https://www.teardowngame.com/modding/api.html#ConstrainAngularVelocity
global ConstrainAngularVelocity: function(bodyA: number, bodyB: number, dir: VectorType, relAngVel: number, min: number, max: number)

---@see @https://www.teardowngame.com/modding/api.html#ConstrainPosition
global ConstrainPosition: function(bodyA: number, bodyB: number, pointA: VectorType, pointB: VectorType, maxVel: number, maxImpulse: number)

---@see @https://www.teardowngame.com/modding/api.html#ConstrainOrientation
global ConstrainOrientation: function(bodyA: number, bodyB: number, quatA: VectorType, quatB: VectorType, maxAngVel: number, maxAngImpulse: number)

---@see @https://www.teardowngame.com/modding/api.html#FindShape
global FindShape: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindShapes
global FindShapes: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetShapeLocalTransform
global GetShapeLocalTransform: function(handle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#SetShapeLocalTransform
global SetShapeLocalTransform: function(handle: number, transform: TransformType)

---@see @https://www.teardowngame.com/modding/api.html#GetShapeWorldTransform
global GetShapeWorldTransform: function(handle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#GetShapeBody
global GetShapeBody: function(handle: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetShapeJoints
global GetShapeJoints: function(shape: number): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetShapeLights
global GetShapeLights: function(shape: number): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetShapeBounds
global GetShapeBounds: function(handle: number): VectorType, VectorType

---@see @https://www.teardowngame.com/modding/api.html#SetShapeEmissiveScale
global SetShapeEmissiveScale: function(handle: number, scale: number)

---@see @https://www.teardowngame.com/modding/api.html#GetShapeMaterialAtPosition
global GetShapeMaterialAtPosition: function(handle: number, pos: VectorType): string, number, number, number, number

---@see @https://www.teardowngame.com/modding/api.html#GetShapeMaterialAtIndex
global GetShapeMaterialAtIndex: function(handle: number, x: number, y: number, z: number): string, number, number, number, number

---@see @https://www.teardowngame.com/modding/api.html#GetShapeSize
global GetShapeSize: function(handle: number): number, number, number, number

---@see @https://www.teardowngame.com/modding/api.html#GetShapeVoxelCount
global GetShapeVoxelCount: function(handle: number): number

---@see @https://www.teardowngame.com/modding/api.html#IsShapeVisible
global IsShapeVisible: function(handle: number, maxDist: number, rejectTransparent: boolean): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsShapeBroken
global IsShapeBroken: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#DrawShapeOutline
global DrawShapeOutline: function(handle: number, r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#DrawShapeHighlight
global DrawShapeHighlight: function(handle: number, amount: number)

---@see @https://www.teardowngame.com/modding/api.html#SetShapeCollisionFilter
global SetShapeCollisionFilter: function(handle: number, layer: number, mask: number)

---@see @https://www.teardowngame.com/modding/api.html#GetShapeClosestPoint
global GetShapeClosestPoint: function(shape: number, origin: VectorType): boolean, VectorType, VectorType

---@see @https://www.teardowngame.com/modding/api.html#FindLocation
global FindLocation: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindLocations
global FindLocations: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetLocationTransform
global GetLocationTransform: function(handle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#FindJoint
global FindJoint: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindJoints
global FindJoints: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#IsJointBroken
global IsJointBroken: function(joint: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#GetJointType
global GetJointType: function(joint: number): string

---@see @https://www.teardowngame.com/modding/api.html#GetJointOtherShape
global GetJointOtherShape: function(joint: number, shape: number): number

---@see @https://www.teardowngame.com/modding/api.html#SetJointMotor
global SetJointMotor: function(joint: number, velocity: number, strength: number)

---@see @https://www.teardowngame.com/modding/api.html#SetJointMotorTarget
global SetJointMotorTarget: function(joint: number, target: number, maxVel: number, strength: number)

---@see @https://www.teardowngame.com/modding/api.html#GetJointLimits
global GetJointLimits: function(joint: number): number, number

---@see @https://www.teardowngame.com/modding/api.html#GetJointMovement
global GetJointMovement: function(joint: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetJointedBodies
global GetJointedBodies: function(body: number): {number}

---@see @https://www.teardowngame.com/modding/api.html#DetachJointFromShape
global DetachJointFromShape: function(joint: number, shape: number)

---@see @https://www.teardowngame.com/modding/api.html#FindLight
global FindLight: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindLights
global FindLights: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#SetLightEnabled
global SetLightEnabled: function(handle: number, enabled: boolean)

---@see @https://www.teardowngame.com/modding/api.html#SetLightColor
global SetLightColor: function(handle: number, r: number, g: number, b: number)

---@see @https://www.teardowngame.com/modding/api.html#SetLightIntensity
global SetLightIntensity: function(handle: number, intensity: number)

---@see @https://www.teardowngame.com/modding/api.html#GetLightTransform
global GetLightTransform: function(handle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#GetLightShape
global GetLightShape: function(handle: number): number

---@see @https://www.teardowngame.com/modding/api.html#IsLightActive
global IsLightActive: function(handle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsPointAffectedByLight
global IsPointAffectedByLight: function(handle: number, point: VectorType): boolean

---@see @https://www.teardowngame.com/modding/api.html#FindTrigger
global FindTrigger: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindTriggers
global FindTriggers: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetTriggerTransform
global GetTriggerTransform: function(handle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#SetTriggerTransform
global SetTriggerTransform: function(handle: number, transform: TransformType)

---@see @https://www.teardowngame.com/modding/api.html#GetTriggerBounds
global GetTriggerBounds: function(handle: number): VectorType, VectorType

---@see @https://www.teardowngame.com/modding/api.html#IsBodyInTrigger
global IsBodyInTrigger: function(trigger: number, body: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsVehicleInTrigger
global IsVehicleInTrigger: function(trigger: number, vehicle: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsShapeInTrigger
global IsShapeInTrigger: function(trigger: number, shape: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsPointInTrigger
global IsPointInTrigger: function(trigger: number, point: VectorType): boolean

---@see @https://www.teardowngame.com/modding/api.html#IsTriggerEmpty
global IsTriggerEmpty: function(handle: number, demolision: boolean): boolean, VectorType

---@see @https://www.teardowngame.com/modding/api.html#GetTriggerDistance
global GetTriggerDistance: function(trigger: number, point: VectorType): number

---@see @https://www.teardowngame.com/modding/api.html#GetTriggerClosestPoint
global GetTriggerClosestPoint: function(trigger: number, point: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#FindScreen
global FindScreen: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindScreens
global FindScreens: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#SetScreenEnabled
global SetScreenEnabled: function(screen: number, enabled: boolean)

---@see @https://www.teardowngame.com/modding/api.html#IsScreenEnabled
global IsScreenEnabled: function(screen: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#GetScreenShape
global GetScreenShape: function(screen: number): number

---@see @https://www.teardowngame.com/modding/api.html#FindVehicle
global FindVehicle: function(tag: string, global: boolean): number

---@see @https://www.teardowngame.com/modding/api.html#FindVehicles
global FindVehicles: function(tag: string, global: boolean): {number}

---@see @https://www.teardowngame.com/modding/api.html#GetVehicleTransform
global GetVehicleTransform: function(vehicle: number): TransformType

---@see @https://www.teardowngame.com/modding/api.html#GetVehicleBody
global GetVehicleBody: function(vehicle: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetVehicleHealth
global GetVehicleHealth: function(vehicle: number): number

---@see @https://www.teardowngame.com/modding/api.html#GetVehicleDriverPos
global GetVehicleDriverPos: function(vehicle: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#DriveVehicle
global DriveVehicle: function(vehicle: number, drive: number, steering: number, handbrake: boolean)

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerPos
global GetPlayerPos: function(): VectorType

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerTransform
global GetPlayerTransform: function(includePitch: boolean): TransformType

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerTransform
global SetPlayerTransform: function(transform: TransformType, includePitch: boolean)

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerGroundVelocity
global SetPlayerGroundVelocity: function(vel: VectorType)

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerCameraTransform
global GetPlayerCameraTransform: function(): VectorType

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerSpawnTransform
global SetPlayerSpawnTransform: function(transform: TransformType)

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerVelocity
global GetPlayerVelocity: function(): VectorType

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerVehicle
global SetPlayerVehicle: function(vehicle: number)

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerVelocity
global SetPlayerVelocity: function(velocity: VectorType)

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerVehicle
global GetPlayerVehicle: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerGrabShape
global GetPlayerGrabShape: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerGrabBody
global GetPlayerGrabBody: function(): number

---@see @https://www.teardowngame.com/modding/api.html#ReleasePlayerGrab
global ReleasePlayerGrab: function()

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerPickShape
global GetPlayerPickShape: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerPickBody
global GetPlayerPickBody: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerInteractShape
global GetPlayerInteractShape: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerInteractBody
global GetPlayerInteractBody: function(): number

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerScreen
global SetPlayerScreen: function(handle: number)

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerScreen
global GetPlayerScreen: function(): number

---@see @https://www.teardowngame.com/modding/api.html#SetPlayerHealth
global SetPlayerHealth: function(health: number)

---@see @https://www.teardowngame.com/modding/api.html#GetPlayerHealth
global GetPlayerHealth: function(): number

---@see @https://www.teardowngame.com/modding/api.html#RespawnPlayer
global RespawnPlayer: function()

---@see @https://www.teardowngame.com/modding/api.html#RegisterTool
global RegisterTool: function(id: string, name: string, file: string, group: number)

---@see @https://www.teardowngame.com/modding/api.html#GetToolBody
global GetToolBody: function(): number

---@see @https://www.teardowngame.com/modding/api.html#SetToolTransform
global SetToolTransform: function(transform: TransformType, sway: number)

---@see @https://www.teardowngame.com/modding/api.html#LoadSound
global LoadSound: function(path: string, nominalDistance: number): number

---@see @https://www.teardowngame.com/modding/api.html#LoadLoop
global LoadLoop: function(path: string, nominalDistance: number): number

---@see @https://www.teardowngame.com/modding/api.html#PlaySound
global PlaySound: function(handle: number, pos: VectorType, volume: number)

---@see @https://www.teardowngame.com/modding/api.html#PlayLoop
global PlayLoop: function(handle: number, pos: VectorType, volume: number)

---@see @https://www.teardowngame.com/modding/api.html#PlayMusic
global PlayMusic: function(path: string)

---@see @https://www.teardowngame.com/modding/api.html#StopMusic
global StopMusic: function()

---@see @https://www.teardowngame.com/modding/api.html#LoadSprite
global LoadSprite: function(path: string): number

---@see @https://www.teardowngame.com/modding/api.html#DrawSprite
global DrawSprite: function(handle: number, transform: TransformType, width: number, height: number, r: number, g: number, b: number, a: number, depthTest: boolean, additive: boolean)

---@see @https://www.teardowngame.com/modding/api.html#QueryRequire
global QueryRequire: function(layers: string)

---@see @https://www.teardowngame.com/modding/api.html#QueryRejectVehicle
global QueryRejectVehicle: function(vehicle: number)

---@see @https://www.teardowngame.com/modding/api.html#QueryRejectBody
global QueryRejectBody: function(body: number)

---@see @https://www.teardowngame.com/modding/api.html#QueryRejectShape
global QueryRejectShape: function(shape: number)

---@see @https://www.teardowngame.com/modding/api.html#QueryRaycast
global QueryRaycast: function(origin: VectorType, direction: VectorType, maxDist: number, radius: number, rejectTransparent: boolean): boolean, number, table, number

---@see @https://www.teardowngame.com/modding/api.html#QueryClosestPoint
global QueryClosestPoint: function(origin: VectorType, maxDist: number): boolean, VectorType, VectorType, number

---@see @https://www.teardowngame.com/modding/api.html#QueryAabbShapes
global QueryAabbShapes: function(min: VectorType, max: VectorType): {number}

---@see @https://www.teardowngame.com/modding/api.html#QueryAabbBodies
global QueryAabbBodies: function(min: VectorType, max: VectorType): {number}

---@see @https://www.teardowngame.com/modding/api.html#QueryPath
global QueryPath: function(startValue: VectorType, endValue: VectorType, maxDist: number, targetRadius: number)

---@see @https://www.teardowngame.com/modding/api.html#AbortPath
global AbortPath: function()

---@see @https://www.teardowngame.com/modding/api.html#GetPathState
global GetPathState: function(): string

---@see @https://www.teardowngame.com/modding/api.html#GetPathLength
global GetPathLength: function(): number

---@see @https://www.teardowngame.com/modding/api.html#GetPathPoint
global GetPathPoint: function(dist: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#GetLastSound
global GetLastSound: function(): number, VectorType

---@see @https://www.teardowngame.com/modding/api.html#IsPointInWater
global IsPointInWater: function(point: VectorType): boolean, number

---@see @https://www.teardowngame.com/modding/api.html#GetWindVelocity
global GetWindVelocity: function(point: VectorType): VectorType

---@see @https://www.teardowngame.com/modding/api.html#ParticleReset
global ParticleReset: function()

---@see @https://www.teardowngame.com/modding/api.html#ParticleType
global ParticleType: function(type: string)

---@see @https://www.teardowngame.com/modding/api.html#ParticleTile
global ParticleTile: function(type: {integer})

---@see @https://www.teardowngame.com/modding/api.html#ParticleColor
global ParticleColor: function(r0: number, g0: number, b0: number, r1: number, g1: number, b1: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleRadius
global ParticleRadius: function(r0: number, r1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleAlpha
global ParticleAlpha: function(a0: number, a1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleGravity
global ParticleGravity: function(g0: number, g1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleDrag
global ParticleDrag: function(d0: number, d1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleEmissive
global ParticleEmissive: function(d0: number, d1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleRotation
global ParticleRotation: function(r0: number, r1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleStretch
global ParticleStretch: function(s0: number, s1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleSticky
global ParticleSticky: function(s0: number, s1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleCollide
global ParticleCollide: function(c0: number, c1: number, interpolation: string, fadein: number, fadeout: number)

---@see @https://www.teardowngame.com/modding/api.html#ParticleFlags
global ParticleFlags: function(bitmask: number)

---@see @https://www.teardowngame.com/modding/api.html#SpawnParticle
global SpawnParticle: function(pos: VectorType, velocity: VectorType, lifetime: number)

---@see @https://www.teardowngame.com/modding/api.html#Shoot
global Shoot: function(origin: VectorType, direction: VectorType, type: number)

---@see @https://www.teardowngame.com/modding/api.html#MakeHole
global MakeHole: function(position: VectorType, r0: number, r1: number, r2: number, silent: boolean)

---@see @https://www.teardowngame.com/modding/api.html#Explosion
global Explosion: function(pos: VectorType, size: number)

---@see @https://www.teardowngame.com/modding/api.html#SpawnFire
global SpawnFire: function(pos: VectorType)

---@see @https://www.teardowngame.com/modding/api.html#GetFireCount
global GetFireCount: function(): number

---@see @https://www.teardowngame.com/modding/api.html#QueryClosestFire
global QueryClosestFire: function(origin: VectorType, maxDist: number): boolean, VectorType

---@see @https://www.teardowngame.com/modding/api.html#QueryAabbFireCount
global QueryAabbFireCount: function(min: VectorType, max: VectorType): number

---@see @https://www.teardowngame.com/modding/api.html#RemoveAabbFires
global RemoveAabbFires: function(min: VectorType, max: VectorType): number

---@see @https://www.teardowngame.com/modding/api.html#GetCameraTransform
global GetCameraTransform: function(): TransformType

---@see @https://www.teardowngame.com/modding/api.html#SetCameraTransform
global SetCameraTransform: function(transform: TransformType, fov: number)

---@see @https://www.teardowngame.com/modding/api.html#SetCameraFov
global SetCameraFov: function(degrees: number)

---@see @https://www.teardowngame.com/modding/api.html#SetCameraDof
global SetCameraDof: function(distance: number, amount: number)

---@see @https://www.teardowngame.com/modding/api.html#PointLight
global PointLight: function(pos: VectorType, r: number, g: number, b: number, intensity: number)

---@see @https://www.teardowngame.com/modding/api.html#SetTimeScale
global SetTimeScale: function(scale: number)

---@see @https://www.teardowngame.com/modding/api.html#SetEnvironmentDefault
global SetEnvironmentDefault: function()

---@see @https://www.teardowngame.com/modding/api.html#SetEnvironmentProperty
global SetEnvironmentProperty: function(name: string, value0: any, value1: any, value2: any, value3: any)

---@see @https://www.teardowngame.com/modding/api.html#GetEnvironmentProperty
global GetEnvironmentProperty: function(name: string): any, any, any, any, any

---@see @https://www.teardowngame.com/modding/api.html#SetPostProcessingDefault
global SetPostProcessingDefault: function()

---@see @https://www.teardowngame.com/modding/api.html#SetPostProcessingProperty
global SetPostProcessingProperty: function(name: string, value0: number, value1: number, value2: number)

---@see @https://www.teardowngame.com/modding/api.html#GetPostProcessingProperty
global GetPostProcessingProperty: function(name: string): number, number, number

---@see @https://www.teardowngame.com/modding/api.html#DrawLine
global DrawLine: function(p0: VectorType, p1: VectorType, r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#DebugLine
global DebugLine: function(p0: VectorType, p1: VectorType, r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#DebugCross
global DebugCross: function(p0: VectorType, r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#DebugWatch
global DebugWatch: function(name: string, value: string)

---@see @https://www.teardowngame.com/modding/api.html#DebugPrint
global DebugPrint: function(message: string)

---@see @https://www.teardowngame.com/modding/api.html#UiMakeInteractive
global UiMakeInteractive: function()

---@see @https://www.teardowngame.com/modding/api.html#UiPush
global UiPush: function()

---@see @https://www.teardowngame.com/modding/api.html#UiPop
global UiPop: function()

---@see @https://www.teardowngame.com/modding/api.html#UiWidth
global UiWidth: function(): number

---@see @https://www.teardowngame.com/modding/api.html#UiHeight
global UiHeight: function(): number

---@see @https://www.teardowngame.com/modding/api.html#UiCenter
global UiCenter: function(): number

---@see @https://www.teardowngame.com/modding/api.html#UiMiddle
global UiMiddle: function(): number

---@see @https://www.teardowngame.com/modding/api.html#UiColor
global UiColor: function(r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#UiColorFilter
global UiColorFilter: function(r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#UiTranslate
global UiTranslate: function(x: number, y: number)

---@see @https://www.teardowngame.com/modding/api.html#UiRotate
global UiRotate: function(angle: number)

---@see @https://www.teardowngame.com/modding/api.html#UiScale
global UiScale: function(x: number, y: number)

---@see @https://www.teardowngame.com/modding/api.html#UiWindow
global UiWindow: function(width: number, height: number, clip: boolean)

---@see @https://www.teardowngame.com/modding/api.html#UiSafeMargins
global UiSafeMargins: function(): number, number, number, number

---@see @https://www.teardowngame.com/modding/api.html#UiAlign
global UiAlign: function(alignment: string)

---@see @https://www.teardowngame.com/modding/api.html#UiModalBegin
global UiModalBegin: function()

---@see @https://www.teardowngame.com/modding/api.html#UiModalEnd
global UiModalEnd: function()

---@see @https://www.teardowngame.com/modding/api.html#UiDisableInput
global UiDisableInput: function()

---@see @https://www.teardowngame.com/modding/api.html#UiEnableInput
global UiEnableInput: function()

---@see @https://www.teardowngame.com/modding/api.html#UiReceivesInput
global UiReceivesInput: function(): boolean

---@see @https://www.teardowngame.com/modding/api.html#UiGetMousePos
global UiGetMousePos: function(): number, number

---@see @https://www.teardowngame.com/modding/api.html#UiIsMouseInRect
global UiIsMouseInRect: function(w: number, h: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#UiWorldToPixel
global UiWorldToPixel: function(point: VectorType): number, number, number

---@see @https://www.teardowngame.com/modding/api.html#UiPixelToWorld
global UiPixelToWorld: function(x: number, y: number): VectorType

---@see @https://www.teardowngame.com/modding/api.html#UiBlur
global UiBlur: function(amount: number)

---@see @https://www.teardowngame.com/modding/api.html#UiFont
global UiFont: function(path: string, size: number)

---@see @https://www.teardowngame.com/modding/api.html#UiFontHeight
global UiFontHeight: function(): number

---@see @https://www.teardowngame.com/modding/api.html#UiText
global UiText: function(text: string, move: boolean): number, number

---@see @https://www.teardowngame.com/modding/api.html#UiGetTextSize
global UiGetTextSize: function(text: string): number, number

---@see @https://www.teardowngame.com/modding/api.html#UiWordWrap
global UiWordWrap: function(width: number)

---@see @https://www.teardowngame.com/modding/api.html#UiTextOutline
global UiTextOutline: function(r: number, g: number, b: number, a: number, thickness: number)

---@see @https://www.teardowngame.com/modding/api.html#UiTextShadow
global UiTextShadow: function(r: number, g: number, b: number, a: number, distance: number, blur: number)

---@see @https://www.teardowngame.com/modding/api.html#UiRect
global UiRect: function(w: number, h: number)

---@see @https://www.teardowngame.com/modding/api.html#UiImage
global UiImage: function(path: string): number, number

---@see @https://www.teardowngame.com/modding/api.html#UiGetImageSize
global UiGetImageSize: function(path: string): number, number

---@see @https://www.teardowngame.com/modding/api.html#UiImageBox
global UiImageBox: function(path: string, width: number, height: number, borderWidth: number, borderHeight: number)

---@see @https://www.teardowngame.com/modding/api.html#UiSound
global UiSound: function(path: string, volume: number, pitch: number, pan: number)

---@see @https://www.teardowngame.com/modding/api.html#UiSoundLoop
global UiSoundLoop: function(path: string, volume: number)

---@see @https://www.teardowngame.com/modding/api.html#UiMute
global UiMute: function(amount: number, music: boolean)

---@see @https://www.teardowngame.com/modding/api.html#UiButtonImageBox
global UiButtonImageBox: function(path: string, borderWidth: number, borderHeight: number, r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#UiButtonHoverColor
global UiButtonHoverColor: function(r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#UiButtonPressColor
global UiButtonPressColor: function(r: number, g: number, b: number, a: number)

---@see @https://www.teardowngame.com/modding/api.html#UiButtonPressDist
global UiButtonPressDist: function(dist: number)

---@see @https://www.teardowngame.com/modding/api.html#UiTextButton
global UiTextButton: function(text: string, w: number, h: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#UiImageButton
global UiImageButton: function(path: number, w: number, h: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#UiBlankButton
global UiBlankButton: function(w: number, h: number): boolean

---@see @https://www.teardowngame.com/modding/api.html#UiSlider
global UiSlider: function(path: number, axis: string, current: number, min: number, max: number): number, boolean

---@see @https://www.teardowngame.com/modding/api.html#UiGetScreen
global UiGetScreen: function(): number

