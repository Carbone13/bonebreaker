extends Node

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

onready var description_label:Label = $DESCRIPTION

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
	
func multiplayer () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Matchmaking Screen.tscn")

func local () -> void:
	SceneManager.call("LoadScene", "res://scenes/Main.tscn")

func quit () -> void:
	get_tree().quit()
	
func credit () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Credit Screen.tscn")

func settings () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Settings Screen.tscn")

func hover_lose () -> void:
	description_label.visible = false

func multi_hovered () -> void:
	description_label.visible = true
	if(Online.nakama_session):
		description_label.text = multi_descr
	else:
		description_label.text = "only available when connected !"

func local_hovered () -> void:
	description_label.visible = true
	description_label.text = local_descr

func settings_hovered () -> void:
	description_label.visible = true
	description_label.text = settings_descr

func credits_hovered () -> void:
	description_label.visible = true
	description_label.text = credits_descr

func quit_hovered () -> void:
	description_label.visible = true
	description_label.text = quit_descr

func offline () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Main.tscn")


