extends "res://scripts/ui/Screen.gd"

func _show_screen(_info:Dictionary = {}):
	if visible:
		ui_layer.show_back_button()
