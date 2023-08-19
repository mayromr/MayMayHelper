local utils = require("utils")

local recallRefill = {}

recallRefill.name = "MayMayHelper/RecallRefill"
recallRefill.depth = -100
recallRefill.placements = {
    {
        name = "recall_refill_one_dash",
        data = {
            twoDashes = false,
            oneUse = false,
            recallDelay = 2,
        },
    },
    {
        name = "recall_refill_two_dash",
        data = {
            twoDashes = true,
            oneUse = false,
            recallDelay = 2,
        },
    }
}

function recallRefill.texture(room, entity)
    return entity.twoDashes and "Maymayhelper/objects/RecallRefill-1/idle01" or "Maymayhelper/objects/RecallRefill-2/idle01"
end


return recallRefill