extends "res://scripts/ui/Screen.gd"

func _show_screen(_info:Dictionary = {}):
	ui_layer.show_back_button()

func _setup_screen (_ui_layer: UILayer):
	._setup_screen(_ui_layer)
	
	ui_layer.connect("back_button", self, "go_back")

func go_back ():
	if visible:
		ui_layer.show_screen("Title Screen")
