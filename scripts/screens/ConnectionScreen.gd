extends Control

const CREDENTIALS_FILENAME = 'user://credentials.json'

onready var dialog:AcceptDialog = $error_box

onready var _login:Control = $LOGIN_PANEL
onready var _register:Control = $REGISTER_PANEL

onready var connecting:Control = $LOGIN_PANEL/loading
onready var registering:Control = $REGISTER_PANEL/loading

onready var login_email_field:LineEdit = $LOGIN_PANEL/email/LineEdit
onready var login_password_field:LineEdit = $LOGIN_PANEL/password/LineEdit
onready var register_email_field:LineEdit = $REGISTER_PANEL/email/LineEdit
onready var register_username_field:LineEdit = $REGISTER_PANEL/username/LineEdit
onready var register_password_field:LineEdit = $REGISTER_PANEL/password/LineEdit
onready var register_save_credentials:CheckBox = $REGISTER_PANEL/remember/CheckBox
onready var login_save_credentials:CheckBox = $LOGIN_PANEL/remember/CheckBox

onready var offline_button:Button = $Offline

var email:String = ''
var password:String = ''
var username:String = ''

var loginSide:bool = true

signal Connected

func _ready () -> void:
	try_load_credentials()
	var args = OS.get_cmdline_args()
	if("--auto-connect" in args):
		login()

func log_nakama_error (var errorcode:NakamaException) -> void:
	dialog.visible = true
	dialog.dialog_text = errorcode.message
	pass
	
func log_error_message (var message:String) -> void:
	dialog.visible = true
	dialog.dialog_text = message

func try_load_credentials() -> void:
	var file = File.new()
	if file.file_exists(CREDENTIALS_FILENAME):
		file.open(CREDENTIALS_FILENAME, File.READ)
		var result := JSON.parse(file.get_as_text())
		if result.result is Dictionary:
			email = result.result['email']
			password = result.result['password']
			login_email_field.text = email
			login_password_field.text = password
		file.close()

func save_credentials() -> void:
	var file = File.new()
	file.open(CREDENTIALS_FILENAME, File.WRITE)
	var credentials = {
		email = email,
		password = password,
	}
	file.store_line(JSON.print(credentials))
	file.close()

func query_credentials() -> void:
	if(loginSide):
		email = login_email_field.text.strip_edges()
		password = login_password_field.text.strip_edges()
	else:
		email = register_email_field.text.strip_edges()
		password = register_password_field.text.strip_edges()
		username = register_username_field.text.strip_edges()

func login () -> void:
	if(!loginSide):
		return
	
	query_credentials()
	var save:bool = login_save_credentials.pressed
	
	if email == '':
		log_error_message(tr("NO_EMAIL"))
		return
	if password == '':
		log_error_message(tr("NO_PASSWORD"))
		return
	if password.length() < 8:
		log_error_message(tr("PASSWORD_TOO_SHORT"))
		return
	connecting.visible = true
	var nakama_session:NakamaSession = yield(Online.nakama_client.authenticate_email_async(email, password, null, false), "completed")
	connecting.visible = false
	if(nakama_session.is_exception()):
		log_nakama_error(nakama_session.exception)
		return
	else:
		if(save):
			save_credentials()
		Online.nakama_session = nakama_session
		Online.connect_nakama_socket()
		emit_signal("Connected")
		SceneManager.call("LoadScene", "res://scenes/menu/Title Screen.tscn")

func register () -> void:
	if(loginSide):
		return
		
	query_credentials()
	var save:bool = register_save_credentials.pressed
	
	if username == '':
		log_error_message(tr("INVALID_USERNAME"))
		return
	if email == '':
		log_error_message(tr("NO_EMAIL"))
		return
	if password == '':
		log_error_message(tr("NO_PASSWORD"))
		return
	if password.length() < 8:
		log_error_message(tr("PASSWORD_TOO_SHORT"))
		return
		
	registering.visible = true
	var nakama_session = yield(Online.nakama_client.authenticate_email_async(email, password, username, true), "completed")
	registering.visible = false
	if(nakama_session.is_exception()):
		log_nakama_error(nakama_session.exception)
		return
	else:
		if(save):
			save_credentials()
		Online.nakama_session = nakama_session
		Online.connect_nakama_socket()
		emit_signal("Connected")
		SceneManager.call("LoadScene", "res://scenes/menu/Title Screen.tscn")

func go_offline () -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Title Screen.tscn")

func switch_to_login_side () -> void:
	loginSide = true
	_login.visible = true
	_register.visible = false
	offline_button.focus_neighbour_left = $LOGIN_PANEL/LOGIN.get_path()
	$LOGIN_PANEL/LOGIN.grab_focus()

func switch_to_register_side () -> void:
	loginSide = false
	_login.visible = false
	_register.visible = true
	offline_button.focus_neighbour_left = $REGISTER_PANEL/REGISTER.get_path()
	$REGISTER_PANEL/REGISTER.grab_focus()
