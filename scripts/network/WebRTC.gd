extends "res://addons/network-sync-rollback/NetworkAdaptor.gd"

#onready var OnlineMatch = get_node('/root/OnlineMatch')
signal received_lobby_select
const DATA_CHANNEL_ID := 42

var data_channels := {}
var message_queue := {}

func _ready():
	if OnlineMatch:
		OnlineMatch.connect("webrtc_peer_added", self, '_on_OnlineMatch_webrtc_peer_added')
		OnlineMatch.connect("webrtc_peer_removed", self, '_on_OnlineMatch_webrtc_peer_removed')
		OnlineMatch.connect("disconnected", self, '_on_OnlineMatch_disconnected')

func _process(delta):
	poll()

func detach_network_adaptor(sync_manager) -> void:
	if OnlineMatch:
		OnlineMatch.disconnect("webrtc_peer_added", self, '_on_OnlineMatch_webrtc_peer_added')
		OnlineMatch.disconnect("webrtc_peer_removed", self, '_on_OnlineMatch_webrtc_peer_removed')
		OnlineMatch.disconnect("disconnected", self, '_on_OnlineMatch_disconnected')
	disconnect("start_received", sync_manager, "_remote_start")

func start_network_adaptor(sync_manager) -> void:
	message_queue.clear()

func stop_network_adaptor(sync_manager) -> void:
	message_queue.clear()

func _on_OnlineMatch_webrtc_peer_added(webrtc_peer: WebRTCPeerConnection, player: OnlineMatch.Player) -> void:
	var peer_id := player.peer_id
	
	if data_channels.has(peer_id):
		data_channels.erase(peer_id)
	
	var data_channel = webrtc_peer.create_data_channel('SyncManager', {
		negotiated = true,
		id = DATA_CHANNEL_ID,
		maxRetransmits = 0,
		ordered = false,
	})
	data_channel.write_mode = WebRTCDataChannel.WRITE_MODE_BINARY
	data_channels[peer_id] = data_channel

func _on_OnlineMatch_webrtc_peer_removed(webrtc_peer: WebRTCPeerConnection, player: OnlineMatch.Player) -> void:
	var peer_id := player.peer_id
	if data_channels.has(peer_id):
		data_channels[peer_id].close()
		data_channels.erase(peer_id)

func _on_OnlineMatch_disconnected() -> void:
	data_channels.clear()
	message_queue.clear()

func send_input_tick(peer_id: int, msg: PoolByteArray) -> void:
	if not data_channels.has(peer_id) or data_channels[peer_id].get_ready_state() != WebRTCDataChannel.STATE_OPEN:
		if not message_queue.has(peer_id):
			message_queue[peer_id] = []
		message_queue[peer_id].append(msg)
	else:
		data_channels[peer_id].put_packet(msg)
		
func send(peer_id: int, msg: PoolByteArray) -> void:
	if not data_channels.has(peer_id) or data_channels[peer_id].get_ready_state() != WebRTCDataChannel.STATE_OPEN:
		if not message_queue.has(peer_id):
			message_queue[peer_id] = []
		message_queue[peer_id].append(msg)
	else:
		data_channels[peer_id].put_packet(msg)

func poll() -> void:
	for peer_id in data_channels:
		var data_channel: WebRTCDataChannel = data_channels[peer_id]

		if data_channel.get_ready_state() != WebRTCDataChannel.STATE_OPEN:
			continue
		
		data_channel.poll()
		
		# Get all received messages.
		while data_channel.get_available_packet_count() > 0:
			var msg = data_channel.get_packet()
			var buffer := StreamPeerBuffer.new()
			buffer.put_data(msg)
			buffer.seek(0)
			
			var id = buffer.get_u8()
			
			if(id == 1):
				emit_signal("received_input_tick", peer_id, buffer.duplicate())
			if(id == 2):
				emit_signal("received_lobby_select", peer_id, buffer.duplicate())
			if(id == 3):
				emit_signal("start_received")
		
		# Send any queued messages.
		if message_queue.has(peer_id):
			var messages_to_send = message_queue[peer_id]
			if messages_to_send.size() > 0:
				for msg in messages_to_send:
					print("send")
					data_channel.put_packet(msg)
				messages_to_send.clear()
