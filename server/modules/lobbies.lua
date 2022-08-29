local nk = require("nakama")
local lobbies = {}

local function update_lobby_infos (context, payload)
    local lobby_info = nk.json_decode(payload)
    lobbies[lobby_info.matchID] = payload;
end

local function get_lobby_infos (context, payload)
    local id = string.format(payload)
    return lobbies[id]
end

nk.register_rpc(update_lobby_infos, "update_lobby_infos")
nk.register_rpc(get_lobby_infos, "get_lobby_infos")