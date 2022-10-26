extends Node

const marston = preload("res://prefabs/characters/marston.tscn")
const musashi = preload("res://prefabs/characters/musashi.tscn")
const namka = preload("res://prefabs/characters/namka.tscn")
const Utils = preload("res://addons/godot-rollback-netcode/Utils.gd")

var players_holder

func spawn_players(players:Dictionary) -> void:
	players_holder = get_node("World/Players")
	
	for session_id in players.keys():
		var id = OnlineMatch.get_peer_id_by_session(session_id)
		
		var data = {
			"player_character":players[session_id],
			"start_transform":Vector2(50 + 50 * id, 90),
			"player_index":id,
			"peer_id":id,
			"player_name":OnlineMatch.get_player_by_peer_id(id).username
		}
		
		spawn_player(data)
	
	SyncManager._spawn_manager._alphabetize_children(players_holder)

func spawn_player(data:Dictionary) -> void:
	var player
	if(data["player_character"] == 1):
		player = marston.instance()
	if(data["player_character"] == 2):
		player = musashi.instance()
	if(data["player_character"] == 3):
		player = namka.instance()
	
	player.global_position = data["start_transform"]
	players_holder.add_child(player)
	Utils.try_call_interop_method(player, "_network_spawn", [data])
