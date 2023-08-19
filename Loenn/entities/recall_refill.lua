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
    return entity.twoDashes and "objects/refillTwo/idle00" or "objects/refill/idle00"
end


return recallRefill