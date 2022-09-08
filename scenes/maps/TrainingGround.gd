extends "res://scripts/map/Map.gd"

var spawned
var tick = 0

func _ready():
	players_holder = get_node("World/Players")
	# local game, for testing purposes
	if(Online.nakama_session == null):
		spawned = marston.instance()
		spawned.global_position = Vector2(70, 80)
		spawned.playerControlled = true
		spawned.focused = true
		get_node("World/Players").add_child(spawned)
		
func _physics_process(delta):
	if(spawned):
		var inp = spawned._get_local_input()
		spawned._network_process(delta, inp, tick)
		spawned.get_node("Animator")._network_process(delta, inp, tick)
		tick += 1
