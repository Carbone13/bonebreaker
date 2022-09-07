extends "res://scripts/ui/Screen.gd"

onready var create_private:Button = $CREATE_GAME/public/CheckBox
onready var create_title:LineEdit = $CREATE_GAME/name/LineEdit
onready var lobby_holder:Control = $JOIN_GAME/ScrollContainer/VBoxContainer

var lobby_instance_prefab = preload("res://prefabs/screens/lobby/lobby_instance.tscn")

var awaiting_operation:bool = false

func _ready(): 
	OnlineMatch.connect("match_created", self, "_on_OnlineMatch_created")
	OnlineMatch.connect("match_joined", self, "_on_OnlineMatch_joined")

func _setup_screen (_ui_layer: UILayer):
	._setup_screen(_ui_layer)
	
	ui_layer.connect("back_button", self, "go_back")

func _show_screen (_info:Dictionary = {}):
	refresh_lobby()
	ui_layer.show_back_button()
	
func go_back ():
	if visible:
		if not awaiting_operation:
			ui_layer.show_screen("Title Screen")
	
func create_lobby() -> void:
	awaiting_operation = true
	OnlineMatch.create_match(Online.nakama_socket, !create_private.pressed, create_title.text)
	
func refresh_lobby() -> void:
	for n in lobby_holder.get_children():
		lobby_holder.remove_child(n)
		n.queue_free()
	
	var data:NakamaAPI.ApiMatchList = yield(Online.get_nakama_client().list_matches_async(Online.nakama_session, 0, 4, 20, false, null, null), "completed")
	
	if(data):
		for m in data._get_matches():
			var info = yield(Online.nakama_socket.rpc_async("get_lobby_infos", m.match_id), "completed")
			var json_result = JSON.parse(info.payload)
			if(json_result.error == OK):
				var payload = json_result.result
				var lobby_instance = lobby_instance_prefab.instance()

				lobby_holder.add_child(lobby_instance)
				lobby_instance.initialize(m.match_id, payload["title"], payload["host"], m.size)

func _on_OnlineMatch_created(match_id: String):
	awaiting_operation = false
	ui_layer.show_screen("Lobby Screen", { match_id = match_id})

func _on_OnlineMatch_joined(match_id: String):
	awaiting_operation = false
	ui_layer.show_screen("Lobby Screen", { match_id = match_id})
