extends Node

export var colors = []
onready var player_card_holder:HBoxContainer = $PLAYERS/HBoxContainer
onready var match_id:LineEdit = $"lobby id/ID"
onready var player_card_prefab = load("res://prefabs/player_card.tscn")

var offset = 0
var cards = {}
var local_card
var selected_character = -1

func _ready():
	# spawn our card if we are the host
	if(Online.nakama_session):
		var card = player_card_holder.get_child(0)
		
		card.call("set_username", Online.nakama_session.username + " (you)")
		card.call("show_none")
		card.call("set_color", colors[0])
		
		card.connect("unselect_character", self, "unselect_local")

		
		card.set_as_local()
		card.visible = true
		
		local_card = card
		cards[-1] = local_card
		
		match_id.text = str(OnlineMatch.match_id)
	
	OnlineMatch.connect("webrtc_peer_added", self, 'player_joined')
	OnlineMatch.connect("webrtc_peer_removed", self, 'player_left')
	SyncManager.network_adaptor.connect("received_lobby_select", self, "received_character_update") 

func _notification(what):
	if what == MainLoop.NOTIFICATION_WM_QUIT_REQUEST:
		get_tree().quit()
	if what == MainLoop.NOTIFICATION_CRASH:
		get_tree().quit()

func received_character_update (peerID, buffer):
	var _int = buffer.get_u8()
	var characterID = buffer.get_u8()

	if(characterID == 0):
		cards[peerID].show_none()
	if(characterID == 1):
		cards[peerID].show_marston()
	if(characterID == 2):
		cards[peerID].show_musashi()
	if(characterID == 3):
		cards[peerID].show_namka()

func player_joined (_webrtc_peer: WebRTCPeerConnection, player: OnlineMatch.Player):
	# if its us
	if(player.username == Online.nakama_session.username):
		print("correcting from red to peer: " + str(player.peer_id))
		var col_id = 3 % player.peer_id
		local_card.call("set_color", colors[col_id])
		cards[player.peer_id] = local_card
		cards.erase(-1)
		return
	print("adding peer " + str(player.peer_id))
	var _child
	for child in player_card_holder.get_children():
		if(!child.visible):
			_child = child
			break
	
	var card = _child
	
	card.remove_child(card.get_node("token"))
	
	card.call("set_username", player.username)
	card.call("show_none")
	
	var col_id = 3 % player.peer_id
	card.call("set_color", colors[col_id])
	
	var token_position = local_card.get_node("token").rect_global_position
		
	card.visible = true
	player_card_holder.queue_sort()
	local_card.get_node("token").call_deferred("set_pos", token_position)

	cards[player.peer_id] = card

	if(selected_character != -1):
		var buffer := StreamPeerBuffer.new()
		buffer.resize(16)
		buffer.put_8(2)
		buffer.put_8(selected_character)
		for peer_id in SyncManager.network_adaptor.data_channels:
			SyncManager.network_adaptor.call("send_input_tick", peer_id, buffer.data_array)

func player_left (_webrtc_peer: WebRTCPeerConnection, player: OnlineMatch.Player):
	# we need to save our token position just before
	offset += 1
	var token_position = local_card.get_node("token").rect_global_position
	
	if(cards.has(player.peer_id)):
		player_card_holder.remove_child(cards[player.peer_id])
		cards[player.peer_id].queue_free()
		cards.erase(player.peer_id)
	
	player_card_holder.queue_sort()
	
	local_card.get_node("token").call_deferred("set_pos", token_position)

func send_select_packet (id):
	var buffer := StreamPeerBuffer.new()
	buffer.resize(16)
	buffer.put_8(2)
	buffer.put_8(id)
	selected_character = id
	for peer_id in SyncManager.network_adaptor.data_channels:
		SyncManager.network_adaptor.call("send_input_tick", peer_id, buffer.data_array)

func unselect_local ():
	send_select_packet(0)

func is_everyone_ready ():
	for card in cards:
		if(!card.confirmed):
			return false
	return true

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
