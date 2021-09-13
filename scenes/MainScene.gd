extends Node2D

var marston = preload("res://prefabs/new/marston.tscn")

var spawned
var tick = 0

func _ready():
	if(Online.nakama_session == null):
		spawned = marston.instance()
		spawned.global_position = Vector2(70, 80)
		get_node("World/Players").add_child(spawned)
		
func _physics_process(delta):
	if(spawned):
		var inp = spawned._get_local_input()
		spawned._network_process(delta, inp, tick)
		tick += 1
