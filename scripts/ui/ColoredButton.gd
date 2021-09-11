extends Control

export(Color) var normal
export(Color) var hovered
export(Color) var clicked

func _ready():
	connect("button_down", self, "button_down")
	connect("button_up", self, "button_up")
	connect("mouse_entered", self, "enter")
	connect("mouse_exited", self, "exit")

func button_down ():
	modulate = clicked

func button_up ():
	modulate = normal
	
func enter ():
	modulate = hovered
	
func exit ():
	modulate = normal
