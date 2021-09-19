extends Node2D

var marston = preload("res://prefabs/characters/marston.tscn")
var musashi = preload("res://prefabs/characters/musashi.tscn")
var namka = preload("res://prefabs/characters/namka.tscn")

var player

func _network_spawn(data:Dictionary):
	if(data["player_character"] == 1):
		player = marston.instance()
	if(data["player_character"] == 2):
		player = musashi.instance()
	if(data["player_character"] == 3):
		player = namka.instance()
	
	player.global_position = data["start_transform"]
	get_parent().add_child(player)
	player._network_spawn(data)
	self.queue_free()
