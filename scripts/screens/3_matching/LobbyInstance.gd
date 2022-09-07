extends Node

onready var info_label = $info
onready var count_label = $count

var _id:String

func join () -> void:
	OnlineMatch.join_match(Online.nakama_socket, _id)

func initialize (id:String, title:String, host:String, count:int) -> void:
	_id = id
	
	if(title == ""):
		title = "Unnamed Lobby"
	
	info_label.text = title + "\n" + "hosted by " + host
	count_label.text = str(count) + "/3"
