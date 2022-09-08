# TOOD deconnection
# TODO handle player leave
# TODO handle host lobby leave

extends Node

const LOG_FILE_DIRECTORY = 'user://detailed_logs'
const DEFAULT_MAP = preload("res://scenes/maps/TrainingGround.tscn")

onready var map_holder = get_node("/root/Main Scene/Game")
onready var ui_layer = get_node("/root/Main Scene/UILayer")

var logging_enabled := true
var map
var players = {}


signal player_lobby_status_updated (session_id, selected)
signal game_started ()

func _ready():
	get_tree().connect("network_peer_connected", self, "_on_network_peer_connected")
	get_tree().connect("network_peer_disconnected", self, "_on_network_peer_disconnected")
	get_tree().connect("server_disconnected", self, "_on_server_disconnected")
	SyncManager.connect("sync_started", self, "_on_SyncManager_sync_started")
	SyncManager.connect("sync_stopped", self, "_on_SyncManager_sync_stopped")
	SyncManager.connect("sync_lost", self, "_on_SyncManager_sync_lost")
	SyncManager.connect("sync_regained", self, "_on_SyncManager_sync_regained")
	SyncManager.connect("sync_error", self, "_on_SyncManager_sync_error")

func update_my_lobby_status (selected:int):
	if get_tree().network_peer:
		rpc("set_player_lobby_status", OnlineMatch.get_my_session_id(), selected)
	else:
		set_player_lobby_status(OnlineMatch.get_my_session_id(), selected)

remotesync func set_player_lobby_status (session_id:String, selected:int):
	emit_signal("player_lobby_status_updated", session_id, selected)

# START THE GAME !
func start_game (_players:Dictionary):
	if get_tree().is_network_server():
		players = _players
		yield(get_tree().create_timer(1.0), "timeout")
		SyncManager.start()

func clear_map () -> void:
	if map:
		map_holder.remove_child(map)
		map.queue_free()
		map = null

func load_map () -> void:
	clear_map()
	
	var _map = DEFAULT_MAP.instance()
	map_holder.add_child(_map)
	map = _map	

func spawn_players() -> void:
	map.spawn_players(players)

func _on_network_peer_connected(peer_id: int):
	SyncManager.add_peer(peer_id)

func _on_network_peer_disconnected(peer_id: int):
	SyncManager.remove_peer(peer_id)

func _on_server_disconnected() -> void:
	_on_network_peer_disconnected(1)

func _on_SyncManager_sync_started() -> void:
	emit_signal("game_started")
	
	if not get_tree().is_network_server():
		players = ui_layer.get_screen("Lobby Screen").players_selection
	
	ui_layer.hide_all()
	ui_layer.show_message("")
	load_map()
	spawn_players()
	
	if logging_enabled:
		var dir = Directory.new()
		if not dir.dir_exists(LOG_FILE_DIRECTORY):
			dir.make_dir(LOG_FILE_DIRECTORY)
		
		var datetime = OS.get_datetime(true)
		var log_file_name = "%04d%02d%02d-%02d%02d%02d-peer-%d.log" % [
			datetime['year'],
			datetime['month'],
			datetime['day'],
			datetime['hour'],
			datetime['minute'],
			datetime['second'],
			get_tree().get_network_unique_id(),
		]
		
		SyncManager.start_logging(LOG_FILE_DIRECTORY + '/' + log_file_name)

func _on_SyncManager_sync_stopped() -> void:
	if logging_enabled:
		SyncManager.stop_logging()

func _on_SyncManager_sync_error(msg: String) -> void:
	SyncManager.clear_peers()
