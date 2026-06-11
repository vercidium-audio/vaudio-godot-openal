@tool
extends EditorPlugin

const VercidiumAudio = preload("res://addons/vaudio-godot-openal/main/VercidiumAudio.cs")
const VercidiumAudioEmitter = preload("res://addons/vaudio-godot-openal/nodes/VercidiumAudioEmitter.cs")
const VercidiumAudioMaterial = preload("res://addons/vaudio-godot-openal/nodes/VercidiumAudioMaterial.cs")

const VercidiumAudioSource = preload("res://addons/vaudio-godot-openal/nodes/VercidiumAudioSource.cs")
const VercidiumAudioSourceRelative = preload("res://addons/vaudio-godot-openal/nodes/VercidiumAudioSourceRelative.cs")
const VercidiumAudioSourceAmbient = preload("res://addons/vaudio-godot-openal/nodes/VercidiumAudioSourceAmbient.cs")

const CSPROJ_INSERT = """    <!-- Add vaudio.dll to your project -->
    <ItemGroup>
        <Reference Include="vaudio">
            <HintPath>path\\to\\your\\vaudio.dll</HintPath>
        </Reference>
    </ItemGroup>"""

func _enter_tree():
	var icon = preload("res://addons/vaudio-godot-openal/icons/vercidium.svg")
	var iconAL = preload("res://addons/vaudio-godot-openal/icons/vercidium_al.svg")

	add_custom_type("VercidiumAudio", "Node", VercidiumAudio, icon)
	add_custom_type("VercidiumAudioEmitter", "Node3D", VercidiumAudioEmitter, icon)
	add_custom_type("VercidiumAudioMaterial", "Node3D", VercidiumAudioMaterial, icon)

	add_custom_type("VercidiumAudioSource", "Node3D", VercidiumAudioSource, iconAL)
	add_custom_type("VercidiumAudioSourceRelative", "Node", VercidiumAudioSourceRelative, iconAL)
	add_custom_type("VercidiumAudioSourceAmbient", "Node", VercidiumAudioSourceAmbient, iconAL)

	_setup_project()

	if not ProjectSettings.settings_changed.is_connected(_on_settings_changed):
		ProjectSettings.settings_changed.connect(_on_settings_changed)

	print("[vaudio-godot-openal] Vercidium Audio (vaudio) plugin enabled")

func _exit_tree():
	remove_custom_type("VercidiumAudio")
	remove_custom_type("VercidiumAudioEmitter")
	remove_custom_type("VercidiumAudioMaterial")

	remove_custom_type("VercidiumAudioSource")
	remove_custom_type("VercidiumAudioSourceRelative")
	remove_custom_type("VercidiumAudioSourceAmbient")

	if ProjectSettings.settings_changed.is_connected(_on_settings_changed):
		ProjectSettings.settings_changed.disconnect(_on_settings_changed)

	print("Vercidium Audio (vaudio-godot-openal) plugin disabled")

var _setup_done := false

func _on_settings_changed():
	if not _setup_done:
		_setup_project()

func _setup_project():
	var project_name = ProjectSettings.get_setting("application/config/name")
	var csproj_path = "res://%s.csproj" % project_name

	if not FileAccess.file_exists(csproj_path):
		push_error("[vaudio-godot-openal] No C# solution found. Please create a C# solution (Project → Tools → C# → Create C# Solution) and then re-enable this plugin")
		return

	var file = FileAccess.open(csproj_path, FileAccess.READ)
	if not file:
		return

	var content = file.get_as_text()
	file.close()

	_setup_done = true

	if "vaudio.dll" in content:
		if "path\\to\\your\\vaudio.dll" in content:
			push_error("[vaudio-godot-openal] csproj is invalid (%s) - please replace 'path\\to\\your\\vaudio.dll' with your actual vaudio.dll path, then disable and enable the VercidiumAudio plugin" % ProjectSettings.globalize_path(csproj_path))
		else:
			print("[vaudio-godot-openal] csproj configured correctly")
		return

	var insert_pos = content.rfind("</Project>")
	if insert_pos == -1:
		push_error("[vaudio-godot-openal] Could not find a </Project> tag in the .csproj file")
		return

	var new_content = content.substr(0, insert_pos) + "\n" + CSPROJ_INSERT + "\n" + content.substr(insert_pos)

	file = FileAccess.open(csproj_path, FileAccess.WRITE)
	if file:
		file.store_string(new_content)
		file.close()
		push_warning("[vaudio-godot-openal] Added vaudio references to ", ProjectSettings.globalize_path(csproj_path), " - please replace 'path\\to\\your\\vaudio.dll' with your actual vaudio.dll path, then disable and enable the VercidiumAudio plugin")

func _enable_plugin():
	if not DirAccess.dir_exists_absolute("res://addons/godot-openal"):
		push_error("[vaudio-godot-openal] The 'godot-openal' plugin is required. Clone it from https://github.com/vercidium-audio/godot-openal and enable it first.")
		get_editor_interface().set_plugin_enabled("vaudio-godot-openal", false)