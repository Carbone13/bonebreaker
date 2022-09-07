extends Control

signal picked

var selected:bool = false

func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT:
			if event.pressed:
				var mousePos = get_viewport().get_mouse_position()
				var rect:Rect2 = Rect2(rect_global_position, rect_size)
				if(rect.has_point(mousePos)):
					selected = !selected
				
				get_child(0).visible = selected
				if(selected):
					emit_signal("picked")
	if(selected):
		if event is InputEventMouseMotion:
			rect_global_position = event.position - rect_size / 4

func is_currently_moving ():
	return selected

func _ready():
	# put it on top
	var canvas_rid = get_canvas_item()
	
	VisualServer.canvas_item_set_draw_index(canvas_rid, 100)
	VisualServer.canvas_item_set_z_index(canvas_rid, 100)
	
	connect("mouse_entered", self, "mouse_enter")
	connect("mouse_exited", self, "mouse_leave")

func mouse_enter ():
	grab_focus()

func mouse_leave ():
	release_focus()
