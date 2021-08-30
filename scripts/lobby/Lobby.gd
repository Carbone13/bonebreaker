extends Node

var id:String

func join ():
	OnlineMatch.join_match(Online.nakama_socket, id)
