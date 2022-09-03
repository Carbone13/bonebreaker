extends Node

# TODO use the sync manager and RPCs !!!!!

export var colors = []
onready var player_card_holder:HBoxContainer = $PLAYERS/HBoxContainer
onready var match_id:LineEdit = $"lobby id/ID"
onready var player_card_prefab = load("res://prefabs/player_card.tscn")

var can_start:bool = false
var offset = 0
var cards = {}
var local_card:Control
var selected_character = -1

var players_character = {}

func _ready():
	match_id.text = str(OnlineMatch.match_id)

	OnlineMatch.connect("player_joined", self, "_on_OnlineMatch_player_joined")
	OnlineMatch.connect("player_left", self, "_on_OnlineMatch_player_left")

	WebRtc.connect("received_lobby_select", self, "received_character_update") 


func _process(delta):
	if(Input.is_action_just_pressed("lobby_start") && can_start):
		start_game()

func start_game ():
	if(cards.has(-1)):
		cards[1] = local_card
		cards.erase(-1)
	var usernames = OnlineMatch.get_player_names_by_peer_id()
	var selected = players_character
	Main.game_start(usernames, selected)
	
func spawn_card(username, color, session_id) -> Node:
	var token_pos
	var was_spawned = false
	if(local_card):
		was_spawned = true
		token_pos = local_card.get_node("token").rect_global_position
	
	var _child
	for child in player_card_holder.get_children():
		if(!child.visible):
			_child = child
			break
	_child.visible = true
	_child.call("set_username", username)
	_child.call("show_none")
	_child.call("set_color", color)
	cards[session_id] = _child
	
	if(session_id == OnlineMatch.get_my_session_id()):
		local_card = _child
		local_card.connect("unselect_character", self, "unselect_local")
	else:
		_child.get_node("token").visible = false
	
	if(local_card && was_spawned):
		local_card.get_node("token").call_deferred("set", "rect_global_position", token_pos)
	
	return _child

func add_player(session_id: String, id, username: String) -> void:
	if (!cards.has(session_id)):
		spawn_card(username, colors[3 % id], session_id)
		
func remove_player (session_id:String) -> void:
	if(cards.has(session_id)):
		var token_pos
		if(local_card):
			token_pos = local_card.get_node("token").rect_global_position
	
		cards[session_id].visible = false
		cards.erase(session_id)
		
		if(local_card):
			local_card.get_node("token").call_deferred("set", "rect_global_position", token_pos)

func received_character_update (peerID, buffer):
	var _int = buffer.get_u8()
	var characterID = buffer.get_u8()
	var sessionid = OnlineMatch.get_player_by_peer_id(peerID).session_id
	
	if(characterID == 0):
		cards[sessionid].show_none()
		hide_start()
	if(characterID == 1):
		cards[sessionid].show_marston()
		cards[sessionid].confirm()
		try_show_start()
	if(characterID == 2):
		cards[sessionid].show_musashi()
		cards[sessionid].confirm()
		try_show_start()
	if(characterID == 3):
		cards[sessionid].show_namka()
		cards[sessionid].confirm()
		try_show_start()
	
	players_character[peerID] = characterID

func hide_start ():
	get_node("Start").visible = false
	can_start = false
	
func try_show_start ():
	if(is_everyone_ready() && OnlineMatch.players.size() > 0):
		get_node("Start").visible = true
		if(!OnlineMatch.host):
			get_node("Start/Label").text = "WAITING FOR THE HOST TO START !"
		else:
			can_start = true
	else:
		hide_start()

func send_select_packet (id):
	try_show_start()
	var buffer := StreamPeerBuffer.new()
	buffer.resize(16)
	buffer.put_8(2)
	buffer.put_8(id)
	selected_character = id
	for peer_id in WebRtc.data_channels:
		WebRtc.call("send", peer_id, buffer.data_array)

func unselect_local ():
	send_select_packet(0)

func is_everyone_ready ():
	for card in cards.values():
		if(!card.confirmed):
			return false
	return true

func _on_OnlineMatch_player_joined(player) -> void:
	add_player(player.session_id, player.peer_id, player.username)
	
	try_show_start()
	
	if(selected_character != -1):
		var buffer := StreamPeerBuffer.new()
		buffer.resize(16)
		buffer.put_8(2)
		buffer.put_8(selected_character)

		WebRtc.call("send", player.peer_id, buffer.data_array)

func _on_OnlineMatch_player_left(player) -> void:
	remove_player(player.session_id)
	
	try_show_start()

func marston_hover_start ():
	if(local_card.moving_token()):
		local_card.show_marston()
	
func marston_hover_stop ():
	if(local_card.moving_token()):
		local_card.show_none()

func marston_click ():
	if(local_card.moving_token()):
		local_card.confirm()
		send_select_packet(1)
		players_character[get_tree().get_network_unique_id()] = 1

func musashi_hover_start ():
	if(local_card.moving_token()):
		local_card.show_musashi()
	
func musashi_hover_stop ():
	if(local_card.moving_token()):
		local_card.show_none()

func musashi_click ():
	if(local_card.moving_token()):
		local_card.confirm()
		send_select_packet(2)
		players_character[get_tree().get_network_unique_id()] = 2
		
func namka_hover_start ():
	if(local_card.moving_token()):
		local_card.show_namka()
	
func namka_hover_stop ():
	if(local_card.moving_token()):
		local_card.show_none()

func namka_click ():
	if(local_card.moving_token()):
		local_card.confirm()
		send_select_packet(3)
		players_character[get_tree().get_network_unique_id()] = 3
