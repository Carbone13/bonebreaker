extends Node


var is_hosting:bool = false

func _notification(what):
	if what == MainLoop.NOTIFICATION_WM_QUIT_REQUEST:
		get_tree().quit()
	if what == MainLoop.NOTIFICATION_CRASH:
		get_tree().quit()


func unregister_lobby() -> void:
	pass
