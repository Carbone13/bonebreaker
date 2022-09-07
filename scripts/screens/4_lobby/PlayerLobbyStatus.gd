extends Panel

signal character_unselected

onready var marston_sprite:Control = $Characters/Marston
onready var musashi_sprite:Control = $Characters/Musashi
onready var namka_sprite:Control = $Characters/Namka
onready var username_label:Label = $username
onready var tween:Tween = $Tween

var selecter
var current_sprite:Control

func initialize (username):
	if(name == OnlineMatch.my_session_id):
		if(selecter):
			selecter.visible = true
	
	username_label.text = username

func set_color (color:Color):
	var style = get_stylebox("panel").duplicate()
	style.bg_color = color
	add_stylebox_override("panel", style)
	
	if selecter:
		selecter.self_modulate = color

func show_none (immediate:bool = false):
	if(current_sprite):
		tween.interpolate_property(current_sprite, "rect_position", 
		current_sprite.rect_position, Vector2(-50, current_sprite.rect_position.y), (0.0 if immediate else 0.16), Tween.TRANS_CUBIC, Tween.EASE_OUT)
		tween.start()
	
	current_sprite = null

func show_sprite (sprite:Control, from:Vector2, to:Vector2) -> void:
	if(sprite == current_sprite):
		return
		
	if(current_sprite):
		current_sprite.visible = false
	
	sprite.visible = true
	tween.interpolate_property(sprite, "rect_position",
	from, to,
	0.13, Tween.TRANS_CUBIC, Tween.EASE_OUT)
	tween.start()
	
	current_sprite = sprite

func show_marston ():
	show_sprite(marston_sprite, Vector2(88, 7), Vector2(32, 7))

func show_musashi ():
	show_sprite(musashi_sprite, Vector2(75, -4), Vector2(14, -4))

func show_namka ():
	show_sprite(namka_sprite, Vector2(75, -8), Vector2(20, -8))

func _on_selecter_picked():
	emit_signal("character_unselected")
