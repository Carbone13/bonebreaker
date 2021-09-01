extends Control

signal clicked

func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT:
			if event.pressed:
				var mousePos = get_viewport().get_mouse_position()
				var rect:Rect2 = Rect2(rect_global_position, rect_size)
				if(rect.has_point(mousePos)):
					emit_signal("clicked")
