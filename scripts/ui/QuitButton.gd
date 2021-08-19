extends Node

func go_back() -> void:
	SceneManager.call("LoadScene", "res://scenes/menu/Title Screen.tscn")


func quit():
	SceneManager.call("LoadScene", "res://scenes/menu/Title Screen.tscn")
