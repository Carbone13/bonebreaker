extends Node

var GameScene = preload("res://scenes/Main.tscn")

var game_started := false
var game_over := false
var players_alive := {}
var players_setup := {}

signal game_started ()
signal player_dead (player_id)
signal game_over (player_id)

func _ready() -> void:
	get_tree().connect("network_peer_connected", self, "tt")
	OnlineMatch.connect("error", self, "_on_OnlineMatch_error")
	OnlineMatch.connect("disconnected", self, "_on_OnlineMatch_disconnected")
	OnlineMatch.connect("player_status_changed", self, "_on_OnlineMatch_player_status_changed")
	OnlineMatch.connect("player_left", self, "_on_OnlineMatch_player_left")
	SyncManager.connect("sync_error", self, "_on_SyncManager_sync_error")

func tt (va):
	print("np")

func game_start(players: Dictionary, selected: Dictionary, immediate: bool = false) -> void:
	if not immediate:
		rpc("_do_game_setup", players, selected)
	else:
		_do_game_setup(players, selected)
		
remotesync func _do_game_setup(players: Dictionary, selected: Dictionary, immediate: bool = false) -> void:
	get_tree().set_pause(true)
	
	if game_started:
		game_stop()
	
	game_started = true
	game_over = false
	players_alive = players
	
	reload_map()
	
	var player_index := 1
	for peer_id in players:
		var spawn_data := {
			peer_id = peer_id,
			player_index = player_index,
			player_name = players[peer_id],
			player_character = selected[peer_id],
			start_transform = Vector2(50 + (50 * (player_index)), 90),
		}
		var other_player = SyncManager.spawn(str(peer_id), get_node("/root/Training Grounds/World/Players"), load("res://prefabs/spawner_holder.tscn"), spawn_data, false, "Player")
		player_index += 1
	
	if not immediate:
		# Tell the host that we've finished setup.
		rpc_id(1, "_finished_game_setup", get_tree().get_network_unique_id())
	else:
		_do_game_start()

# Records when each player has finished setup so we know when all players are ready.
mastersync func _finished_game_setup(player_id: int) -> void:
	players_setup[player_id] = players_alive[player_id]
	if players_setup.size() == players_alive.size():
		# Once all clients have finished setup, tell them to start the game.
		rpc("_do_game_start")
		SyncManager.start()

# Actually start the game on this client.
remotesync func _do_game_start() -> void:
	emit_signal("game_started")
	get_tree().set_pause(false)

func reload_map() -> void:
	SceneManager.LoadScene("res://scenes/Main.tscn")

func kill_player(player_id) -> void:
	var player_node = get_node("/root/Training Grounds/World/Players").get_node("Player " + str(player_id))
	if player_node:
		if player_node.has_method("die"):
			player_node.die()
		else:
			# If there is no die method, we do the most important things it
			# would have done.
			player_node.queue_free()
			_on_player_dead(player_id)

func game_stop() -> void:	
	game_started = false
	players_setup.clear()
	players_alive.clear()
	
	for child in get_node("/root/Training Grounds/World/Players").get_children():
		get_node("/root/Training Grounds/World/Players").remove_child(child)
		child.queue_free()

func _save_state() -> Dictionary:
	return {
		players_alive = players_alive,
		game_over = game_over,
	}

func _load_state(state: Dictionary) -> void:
	players_alive = state['players_alive']
	game_over = state['game_over']


func _on_SyncManager_scene_spawned(name: String, spawned_node: Node, scene: PackedScene, data: Dictionary) -> void:
	pass

func _on_player_dead(player_id) -> void:
	emit_signal("player_dead", player_id)
	
	players_alive.erase(player_id)
	if not game_over and players_alive.size() == 1:
		game_over = true
		var player_keys = players_alive.keys()
		emit_signal("game_over", player_keys[0])
		
func _on_OnlineMatch_player_status_changed(player, status) -> void:
	if status == OnlineMatch.PlayerStatus.CONNECTED:
		if player.peer_id != get_tree().get_network_unique_id():
			SyncManager.add_peer(player.peer_id)

func _on_OnlineMatch_player_left(player) -> void:
	print(player.username + " has left")
	if(!SyncManager.started):
		return
	
	kill_player(player.peer_id)
	
	SyncManager.remove_peer(player.peer_id)
	
	players_alive.erase(player.peer_id)

func _on_OnlineMatch_error(message: String):
	if message != '':
		print(message)
	SyncManager.stop()
	SyncManager.clear_peers()

func _on_OnlineMatch_disconnected():
	#_on_OnlineMatch_error("Disconnected from host")
	if(!SyncManager.started):
		return
		
	_on_OnlineMatch_error('')

func _on_SyncManager_sync_error(msg: String) -> void:
	print("Fatal sync error: " + msg)
	var peer = get_tree().network_peer
	if peer:
		peer.close_connection()
	SyncManager.clear_peers()
