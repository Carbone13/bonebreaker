extends "res://scripts/ui/Screen.gd"

export var colors = []

var selecter_default_position = Vector2(152, -10)

onready var status_container = $PLAYERS/HBoxContainer
onready var selecter = $PLAYERS/selecter
onready var start = $Start

var auto_start:bool

var local_status
var selected_char:int

var players_selection = {}

func _ready() -> void:
	var args = OS.get_cmdline_args()
	if("--auto-start-game" in args):
		auto_start = true
		
	clear_players()
	
	OnlineMatch.connect("player_joined", self, "_on_OnlineMatch_player_joined")
	OnlineMatch.connect("player_left", self, "_on_OnlineMatch_player_left")
	OnlineMatch.connect("player_status_changed", self, "_on_new_connection")
	
	selecter.connect("picked", self, "unselected_char")	
	Main.connect("player_lobby_status_updated", self, "_on_player_status_updated")

func _show_screen(info: Dictionary = {}) -> void:
	ui_layer.show_back_button()
	selected_char = 0
	
	var players: Dictionary = info.get("players", {})
	var match_id: String = info.get("match_id", 'unknown')
	
	selecter.rect_position = selecter_default_position
	clear_players()
	
	for session_id in players:
		add_player(session_id, players[session_id]['username'])
	
	if match_id:
		ui_layer.show_message("Match ID: " + match_id)
	
	if not get_tree().is_network_server():
		start.get_node("Label").text = "WAITING FOR START"

func _setup_screen (ui_layer:UILayer) -> void:
	._setup_screen(ui_layer)
	ui_layer.connect("back_button", self, "go_back")

func go_back () -> void:
	if visible:
		OnlineMatch.leave()
		get_tree().network_peer = null
		ui_layer.show_message("")
		ui_layer.show_screen("Matching Screen")

func clear_players() -> void:
	if local_status:
		local_status.selecter = null
		local_status = null
		
	for child in status_container.get_children():
		child.show_none(true)
		child.name = child.name + " EMPTY"
		child.visible = false

func get_next_card () -> Node:
	for child in status_container.get_children():
		if not child.visible:
			return child
	return null

func add_player(session_id: String, username: String) -> void:
	if not status_container.has_node(session_id):
		var status = get_next_card()

		if(session_id == OnlineMatch.my_session_id):
			local_status = status
			local_status.selecter = selecter

		status.visible = true
		status.name = session_id
		status.initialize(username)
		status.set_color(colors[colors.size() % (get_player_count())])

func remove_player(session_id:String) -> void:
	var status = status_container.get_node(session_id)
	if(status):
		status.visible = false

func get_player_count () -> int:
	var c = 0
	for child in status_container.get_children():
		if child.visible:
			c += 1
	return c

func is_everyone_ready () -> void:
	for v in players_selection.values():
		if v == 0:
			start.visible = false
			return

	start.visible = true
	if(auto_start and get_player_count() > 1):
		start_game()

func _process(_delta) -> void:
	if Input.is_action_just_pressed("start_game"):
		start_game()

func start_game() -> void:
	if get_tree().is_network_server():
		ui_layer.show_message("Starting...")
		Main.start_game(players_selection)
# Callbacks
func _on_OnlineMatch_player_joined(player) -> void:
	yield(get_tree(), "idle_frame")
	if OnlineMatch.match_id != "":
		add_player(player.session_id, player.username)

func _on_new_connection (peer_id, data) -> void:
	yield(get_tree(), "idle_frame")
	Main.update_my_lobby_status(selected_char)

func _on_OnlineMatch_player_left(player) -> void:
	remove_player(player.session_id)

func _on_player_status_updated (session_id, selected) -> void:
	if status_container.has_node(session_id):
		var status = status_container.get_node(session_id)
		if selected == 0:
			status.show_none()
		if selected == 1:
			status.show_marston()
		if selected == 2:
			status.show_musashi()
		if selected == 3:
			status.show_namka()
		
		players_selection[session_id] = selected
	
	is_everyone_ready()

func local_token_moving () -> bool:
	return (local_status and local_status.selecter.is_currently_moving())

# UI selection callbacks
func marston_hover_start ():
	if(local_token_moving()):
		local_status.show_marston()

func marston_hover_stop ():
	if(local_token_moving()):
		local_status.show_none()
		
func marston_click ():
	if(local_token_moving()):
		selected_char = 1
		Main.update_my_lobby_status(1)

func musashi_hover_start ():
	if(local_token_moving()):
		local_status.show_musashi()
	
func musashi_hover_stop ():
	if(local_token_moving()):
		local_status.show_none()

func musashi_click ():
	if(local_token_moving()):
		selected_char = 2
		Main.update_my_lobby_status(2)
		
func namka_hover_start ():
	if(local_token_moving()):
		local_status.show_namka()

func namka_hover_stop ():
	if(local_token_moving()):
		local_status.show_none()

func namka_click ():
	if(local_token_moving()):
		selected_char = 3
		Main.update_my_lobby_status(3)

func unselected_char ():
	selected_char = 0
	Main.update_my_lobby_status(0)
