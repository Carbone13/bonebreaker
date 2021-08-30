extends Node

onready var match_id:LineEdit = $"lobby id/ID"

var is_hosting:bool = false

func _ready():
	match_id.text = str(OnlineMatch.match_id)

func _notification(what):
	if what == MainLoop.NOTIFICATION_WM_QUIT_REQUEST:
		get_tree().quit()
	if what == MainLoop.NOTIFICATION_CRASH:
		get_tree().quit()


func unregister_lobby() -> void:
	pass
