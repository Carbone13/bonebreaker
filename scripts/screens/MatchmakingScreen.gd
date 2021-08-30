extends Node

onready var create_private:Button = $CREATE_GAME/public/CheckBox
onready var create_title:LineEdit = $CREATE_GAME/name/LineEdit
onready var lobby_holder:Control = $JOIN_GAME/ScrollContainer/VBoxContainer
const lobby_control_prefab:String = "res://prefabs/lobby.tscn"

func _ready(): 
	refresh_lobby()
	OnlineMatch.connect("match_created", self, "_on_OnlineMatch_created")
	OnlineMatch.connect("match_joined", self, "_on_OnlineMatch_joined")

func create_lobby() -> void:
	OnlineMatch.create_match(Online.nakama_socket, !create_private.pressed, create_title.text)
	
func refresh_lobby() -> void:
	for n in lobby_holder.get_children():
		lobby_holder.remove_child(n)
		n.queue_free()
	
	var data:NakamaAPI.ApiMatchList = yield(Online.get_nakama_client().list_matches_async(Online.nakama_session, 0, 4, 20, false), "completed")
	if data.is_exception():
		emit_signal("error", "Could not fetch available lobbies")
		return null
	else:
		if(data):
			for m in data._get_matches():
				var lobby_info = load(lobby_control_prefab).instance()
			
				var info = yield(Online.nakama_socket.rpc_async("get_lobby_infos", m.match_id), "completed")
				var json_result = JSON.parse(info.payload)
				if(json_result.error == OK):
					var payload = json_result.result
					if(payload["result"] == 0):
						return
					
					lobby_info.set("id", m.match_id)
					if(payload["title"] == ""):
						payload["title"] = "Unnamed Lobby"
					lobby_info.get_node("info").text = payload["title"] + "\n" + "hosted by "+ payload["host"]
					lobby_info.get_node("count").text = str(m.size) + "/4"
					lobby_holder.add_child(lobby_info)
	
func _on_OnlineMatch_created(_match_id: String):
	SceneManager.call("LoadScene", "res://scenes/menu/Lobby Screen.tscn")
	
func _on_OnlineMatch_joined(_match_id: String):
	SceneManager.call("LoadScene", "res://scenes/menu/Lobby Screen.tscn")
