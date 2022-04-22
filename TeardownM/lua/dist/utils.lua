local _tl_compat; if (tonumber((_VERSION or ''):match('[%d.]*$')) or 0) < 5.3 then local p, m = pcall(require, 'compat53.module'); if p then _tl_compat = m end end; local ipairs = _tl_compat and _tl_compat.ipairs or ipairs; local math = _tl_compat and _tl_compat.math or math; local string = _tl_compat and _tl_compat.string or string; local table = _tl_compat and _tl_compat.table or table


function explode(data)
   local d = ","
   local t = {}
   local ll = 0

   if (#data == 1) then
      return nil
   end

   while true do
      local l = string.find(data, d, ll, true)
      if l ~= nil then
         table.insert(t, string.sub(data, ll, l - 1))
         ll = l + 1
      else
         table.insert(t, string.sub(data, ll))
         break
      end
   end

   return t
end



function round(num, numDecimalPlaces)
   local mult = 10 ^ (numDecimalPlaces or 0)
   return math.floor(num * mult + 0.5) / mult
end


function stringToBool(bool)
   local stringtoboolean = {
      ["true"] = true,
      ["false"] = false,
   }

   return stringtoboolean[bool]
end

function arrayToString(array)
   local stringArray = ""

   for index, value in ipairs(array) do


      if (type(value) == "boolean") then
         value = (value and 'true' or 'false')
      end

      if (index == #array) then
         stringArray = stringArray .. value
      else
         stringArray = stringArray .. value .. ","
      end
   end

   return stringArray
end
