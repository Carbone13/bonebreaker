extends Node

export(Color) var disabled_button_color

onready var ucaret_username:Label = $USER_CARET/username
onready var ucaret_level:Label = $USER_CARET/level
onready var ucaret_logout:Control = $USER_CARET/logout

onready var but_multiplayer:Button = $PLAY_MODES/multiplayer
onready var but_local:Button = $PLAY_MODES/local
onready var but_settings:Button = $PLAY_MODES/settings
onready var but_credits:Button = $PLAY_MODES/credits
onready var but_quit:Button = $PLAY_MODES/quit

func _ready():
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
	SceneManager.call("LoadScene", "res://scenes/menu/Connection Screen.tscn")
	
func quit () -> void:
	get_tree().quit()
	
func credit () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Credit Screen.tscn")

func settings () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Settings Screen.tscn")

