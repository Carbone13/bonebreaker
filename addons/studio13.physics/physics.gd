tool
extends EditorPlugin


func _enter_tree():
	pass
	#add_custom_type("Actor2D", "Actor2D", preload("res://scripts/physics/Actor2D.cs"), preload("icons/CharacterBody2D.svg"))
	#add_custom_type("CollisionBox", "Node2D", preload("res://scripts/physics/CollisionBox.cs"), preload("icons/CollisionShape2D.svg"))


func _exit_tree():
	pass
	#remove_custom_type("Actor2D")
	#remove_custom_type("CollisionBox")
