-- leave space here

-- explode(seperator, string)
function explode<Type>(data: string): Type
   local d = ","
   local t = {}
   local ll = 0

   if(#data == 1) then
      return nil
   end

   while true do
      local l = string.find(data, d, ll, true) -- find the next d in the string
      if l ~= nil then -- if "not not" found then..
         table.insert(t, string.sub(data, ll, l-1)) -- Save it in our array.
         ll = l + 1 -- save just after where we found it for searching next time.
      else
         table.insert(t, string.sub(data, ll)) -- Save what's left in our array.
         break -- Break at end, as it should be, according to the lua manual.
      end
   end
   
   return t as Type
end

-- Given a number and how many decial places, round that number
-- either up or down.
function round(num: number, numDecimalPlaces: number): number
    local mult = 10 ^ (numDecimalPlaces or 0)
    return math.floor(num * mult + 0.5) / mult
end

-- Converts a string boolean to a real boolean
function stringToBool(bool: string): boolean | nil
    local stringtoboolean: {string:boolean} = {
        ["true"] = true,
        ["false"] = false
    }

    return stringtoboolean[bool]
end

function arrayToString<K,V>(array: {string | boolean | number}): string
   local stringArray = ""

   for index, value in ipairs(array) do

      -- Converts boolean to string boolean
      if (type(value) == "boolean") then
         value = (value and 'true' or 'false')
      end

      if (index == #array) then
         stringArray = stringArray .. value as string
      else
         stringArray = stringArray .. value as string .. ","
      end
   end

   return stringArray 
end
