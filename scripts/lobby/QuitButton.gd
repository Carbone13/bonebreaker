extends Node

func quit_lobby():
	OnlineMatch.leave()
	get_tree().network_peer = null
	SceneManager.LoadScene("res://scenes/menu/Matchmaking Screen.tscn")
