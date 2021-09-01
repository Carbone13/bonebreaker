extends Panel

onready var marston_sprite:TextureRect = $Characters/Marston
onready var musashi_sprite:TextureRect = $Characters/Musashi
onready var namka_sprite:TextureRect = $Characters/Namka

var current_sprite:Control
var local:bool = false

func set_color (color:Color):
	#left side
	var left_style:StyleBoxFlat = get_node("left_side").get_stylebox("panel").duplicate()
	left_style.bg_color = color
	get_node("left_side").add_stylebox_override("panel", left_style)
	# token
	if(get_node_or_null("token")):
		get_node("token").modulate = color
	# username
	var username_style:StyleBoxFlat = get_node("username/dash").get_stylebox("panel").duplicate()
	username_style.bg_color = color
	get_node("username/dash").add_stylebox_override("panel", username_style)
	# show area
	get_node("Characters/Show Area").color = color
	
func confirm ():
	pass

func moving_token ():
	return get_node("token").get("selected") as bool

func set_as_local ():
	local = true
	get_node("token").set("local", true)

func set_username (username:String):
	get_node("username").set("text", username)

func show_none ():
	var tween:Tween = get_node("Tween")
	if(current_sprite):
		tween.interpolate_property(current_sprite, "rect_position",
		current_sprite.rect_position, Vector2(-35, current_sprite.rect_position.y), 0.1, Tween.TRANS_CUBIC, Tween.EASE_OUT)
	tween.call_deferred("start")
	
	current_sprite = null

func show_marston ():
	marston_sprite.visible = true
	
	var tween:Tween = get_node("Tween")
	tween.interpolate_property(marston_sprite, "rect_position",
	Vector2(80, 13), Vector2(27, 13), 0.1, Tween.TRANS_CUBIC, Tween.EASE_OUT)
	tween.start()
	
	if(current_sprite):
		current_sprite.visible = false
	current_sprite = marston_sprite
	marston_sprite.visible = true
	
func show_musashi ():
	musashi_sprite.visible = true
	
	var tween:Tween = get_node("Tween")
	tween.interpolate_property(musashi_sprite, "rect_position",
	Vector2(80, 11), Vector2(23, 11), 0.1, Tween.TRANS_CUBIC, Tween.EASE_OUT)
	tween.start()

	if(current_sprite):
		current_sprite.visible = false
	current_sprite = musashi_sprite
	musashi_sprite.visible = true
	
func show_namka ():
	namka_sprite.visible = true
	
	var tween:Tween = get_node("Tween")
	tween.interpolate_property(namka_sprite, "rect_position",
	Vector2(80, 7), Vector2(27, 7), 0.1, Tween.TRANS_CUBIC, Tween.EASE_OUT)
	tween.start()
	
	if(current_sprite):
		current_sprite.visible = false
	current_sprite = namka_sprite
	namka_sprite.visible = true
