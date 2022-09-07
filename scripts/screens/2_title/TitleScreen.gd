extends "res://scripts/ui/Screen.gd"

const multi_descr:String = "connect to your friends thanks to the internet, and beat their ass :)"
const local_descr:String = "no internet ? no problem ! grab your brother and another controller !"
const settings_descr:String = "man, these default settings sucks, lets change them"
const credits_descr:String = "wanna know about the delicious person who made this game with <3 ?"
const quit_descr:String = "no, stay here :("

export(Color) var disabled_button_color

onready var ucaret_username:Label = $USER_CARET/username
onready var ucaret_level:Label = $USER_CARET/level
onready var ucaret_logout:Control = $USER_CARET/logout

onready var but_multiplayer:Button = $PLAY_MODES/multiplayer
onready var but_local:Button = $PLAY_MODES/local
onready var but_settings:Button = $PLAY_MODES/settings
onready var but_credits:Button = $PLAY_MODES/credits
onready var but_quit:Button = $PLAY_MODES/quit


func _show_screen(_info:Dictionary = {}):
	setup_user_caret()
	setup_buttons()

func setup_user_caret () -> void:
	if(Online.nakama_session):
		ucaret_username.text = Online.nakama_session.username
		ucaret_level.text = "lv: 0"
	else:
		ucaret_username.text = "Offline!"
		ucaret_level.text = ""

func setup_buttons () -> void:
	if(!Online.nakama_session):
		but_multiplayer.disabled = true
		var bg_stylebox = but_multiplayer.get_node("bg").get_stylebox("panel").duplicate()
		bg_stylebox.bg_color = disabled_button_color
		but_multiplayer.get_node("bg").add_stylebox_override("panel", bg_stylebox)

func logout () -> void:
	Online.nakama_session = null
	ui_layer.hide_message()
	ui_layer.show_screen("Connection Screen")

func multiplayer () -> void:
	ui_layer.show_screen("Matching Screen")

func local () -> void:
	pass

func quit () -> void:
	get_tree().quit()
	
func credit () -> void:
	ui_layer.show_screen("Credits Screen")
	
func settings () -> void:
	ui_layer.show_screen("Settings Screen")

func hover_lose():
	ui_layer.hide_message()

func multi_hovered () -> void:
	if(Online.nakama_session):
		ui_layer.show_message(multi_descr)
	else:
		ui_layer.show_message("only available when connected !")

func local_hovered () -> void:
	ui_layer.show_message(local_descr)

func settings_hovered () -> void:
	ui_layer.show_message(settings_descr)

func credits_hovered () -> void:
	ui_layer.show_message(credits_descr)

func quit_hovered () -> void:
	ui_layer.show_message(quit_descr)
