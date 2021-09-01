extends Node

export var colors = []
onready var player_card_holder = $PLAYERS/HBoxContainer
onready var match_id:LineEdit = $"lobby id/ID"
onready var player_card_prefab = load("res://prefabs/player_card.tscn")


var cards = {}
var local_card
var selected_character = -1

func _ready():
	# spawn our card if we are the host
	if(Online.nakama_session):
		var card = player_card_prefab.instance()
		card.call("set_username", Online.nakama_session.username + " (you)")
		card.call("show_none")
		card.call("set_color", colors[0])
		player_card_holder.add_child(card)
		card.set_as_local()
		
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
	
	if(characterID == 1):
		cards[peerID].show_marston()
	if(characterID == 2):
		cards[peerID].show_musashi()
	if(characterID == 3):
		cards[peerID].show_namka()

func player_joined (_webrtc_peer: WebRTCPeerConnection, player: OnlineMatch.Player):
	# if its us
	if(player.username == Online.nakama_session.username):
		local_card.call("set_color", colors[3 % player.peer_id])
		cards[player.peer_id] = local_card
		cards.erase(-1)
		return
	
	print("spawning card for peer " + str(player.peer_id))
	var card = player_card_prefab.instance()
	
	card.remove_child(card.get_node("token"))
	
	card.call("set_username", player.username)
	card.call("show_none")
	card.call("set_color", colors[3 % player.peer_id])
	player_card_holder.add_child(card)
	
	cards[player.peer_id] = card
	
	get_node("PLAYERS/one").visible = cards.keys().size() == 1
	get_node("PLAYERS/two").visible = cards.keys().size() == 2
	get_node("PLAYERS/three").visible = cards.keys().size() == 3
	
	if(selected_character != -1):
		var buffer := StreamPeerBuffer.new()
		buffer.put_8(2)
		buffer.put_8(selected_character)
		var channel:WebRTCDataChannel = SyncManager.network_adaptor.get("data_channels")[player.peer_id]
		channel.put_packet(buffer.data_array)

func player_left (_webrtc_peer: WebRTCPeerConnection, player: OnlineMatch.Player):
	if(cards.has(player.peer_id)):
		player_card_holder.remove_child(cards[player.peer_id])
		cards[player.peer_id].queue_free()
		cards.erase(player.peer_id)
	
	get_node("PLAYERS/one").visible = cards.keys().size() == 1
	get_node("PLAYERS/two").visible = cards.keys().size() == 2
	get_node("PLAYERS/three").visible = cards.keys().size() == 3
	
func send_select_packet (id):
	var buffer := StreamPeerBuffer.new()
	buffer.resize(16)
	buffer.put_8(2)
	buffer.put_8(id)
	selected_character = id
	for peer_id in SyncManager.network_adaptor.data_channels:
		SyncManager.network_adaptor.call("send_input_tick", peer_id, buffer.data_array)

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
