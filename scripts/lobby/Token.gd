extends Control

signal unselect

var local:bool = false
var selected:bool = false

func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == BUTTON_LEFT:
			if event.pressed:
				var mousePos = get_viewport().get_mouse_position()
				var rect:Rect2 = Rect2(rect_global_position, rect_size)
				if(rect.has_point(mousePos)):
					selected = !selected
					
				if(selected):
					emit_signal("unselect")
	if(selected):
		if event is InputEventMouseMotion:
			rect_global_position = event.position - rect_size / 4

func set_pos (pos):
	rect_global_position = pos

func _ready():
	var canvas_rid = get_canvas_item()
	# You may need to adjust these values
	VisualServer.canvas_item_set_draw_index(canvas_rid, 100)
	VisualServer.canvas_item_set_z_index(canvas_rid, 100)
	
	connect("focus_entered", self, "focus_enter")
	connect("focus_exited", self, "focus_exit")
	connect("mouse_entered", self, "mouse_enter")
	connect("mouse_exited", self, "mouse_leave")

func focus_enter ():
	pass

func focus_exit ():
	pass

func mouse_enter ():
	if(local):
		grab_focus()

func mouse_leave ():
	if(local):
		release_focus()
