extends Node

func quit_lobby():
	OnlineMatch.leave()
	SceneManager.LoadScene("res://scenes/menu/Matchmaking Screen.tscn")
