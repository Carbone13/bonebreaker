extends Node

signal player_lobby_status_updated (session_id, selected)

func update_my_lobby_status (selected:int):
	rpc("set_player_lobby_status", OnlineMatch.get_my_session_id(), selected)

remotesync func set_player_lobby_status (session_id:String, selected:int):
	emit_signal("player_lobby_status_updated", session_id, selected)
