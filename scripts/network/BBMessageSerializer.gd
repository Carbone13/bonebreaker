extends "res://addons/godot-rollback-netcode/MessageSerializer.gd"

const input_path_mapping := {
	'$': 0,
	'/root/Main Scene/Game/Training Grounds/World/Players/Player 1': 1,
	'/root/Main Scene/Game/Training Grounds/World/Players/Player 2': 2,
	'/root/Main Scene/Game/Training Grounds/World/Players/Player 3': 3,
}

func serialize_input(all_input: Dictionary) -> PoolByteArray:
	var buffer := StreamPeerBuffer.new()
	buffer.resize(32)
	
	buffer.put_u8(all_input.size())
	for path in all_input:
		var mapped_path = input_path_mapping[path]
		buffer.put_u8(mapped_path)
		if mapped_path == 0:
			buffer.put_u32(all_input[path])
			continue
		
		var input = all_input[path]
		buffer.put_float(input["x"])
		buffer.put_float(input["y"])
		buffer.put_u8(input["b"])
	
	buffer.resize(buffer.get_position())
	return buffer.data_array

func unserialize_input(serialized: PoolByteArray) -> Dictionary:
	var buffer := StreamPeerBuffer.new()
	buffer.put_data(serialized)
	buffer.seek(0)
	
	var all_input := {}
	
	var path_count = buffer.get_u8()
	for path_index in range(path_count):
		var mapped_path = buffer.get_u8()
		if mapped_path == 0:
			all_input['$'] = buffer.get_u32()
			continue
		
		var input := {}
		input["x"] = buffer.get_float()
		input["y"] = buffer.get_float()
		input["b"] = buffer.get_u8()
		
		var path = '/root/Main Scene/Game/Training Grounds/World/Players/Player ' + str(mapped_path)
		all_input[path] = input
	
	return all_input
