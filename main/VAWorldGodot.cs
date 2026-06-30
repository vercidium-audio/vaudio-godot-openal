using godot_openal;

namespace vaudio_godot_openal;

public partial class VAWorld : Node
{
    public bool Initialised => world != null;

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
            return;

        // Cache the scene root since we access it often
        SceneRoot = GetTree().CurrentScene as Node3D;

        world = new()
        {
            LogCallback = Log,
            Position = new(Position.X, Position.Y, Position.Z),
            Size = new(Size.X, Size.Y, Size.Z),
            Epsilon = Epsilon,
            WorldIsIndoors = WorldIsIndoors,

            // Reverb
            MaximumGroupedEAXCount = MaximumGroupedEAXCount,
            OnReverbUpdated = OnReverbUpdated,

            // Air absorption
            MetersPerUnit = MetersPerUnit,
            InverseSpeedOfSound = 1.0f /    SpeedOfSound,
            ReferenceFrequencyLF = ReferenceFrequencyLF,
            ReferenceFrequencyHF = ReferenceFrequencyHF,

            // Emitters 
            EmittersOutsideTheWorldAreMuffled = EmittersOutsideTheWorldAreMuffled,

            // Threading
            MaximumConcurrencyLevel = MaximumConcurrencyLevel,
            WorkItemCount = WorkItemCount,

            RenderingEnabled = RenderingEnabled,
        };

        world.AirAbsorption.Humidity = Humidity;
        world.AirAbsorption.Temperature = Temperature;
        world.AirAbsorption.Pressure = Pressure;

        // Create reverb effects
        OnDeviceRecreated();

        if (!GodotOpenALEnabled)
        {
            LogWarning("The godot-openal addon is not found. For best audio quality, ensure godot-openal is enabled and the ALManager autoload is enabled in Project Settings > Globals");
        }

        // Register for device destroyed/recreated callbacks to clean up and recreate reverb effects
        RegisterDeviceRecreatedCallback(OnDeviceRecreated);
        RegisterDeviceDestroyedCallback(OnDeviceDestroyed);

        // Wait a frame for the scene to be fully loaded
        CallDeferred(nameof(InitializeScene));
    }

    void OnDeviceRecreated()
    {
        // Recreate the reverb effects after the device is recreated
        listenerReverbEffect = new();
    }

    void OnDeviceDestroyed()
    {
        // Delete all reverb effects - they contain OpenAL resources that are now invalid
        ambientFilter?.Delete();
        ambientFilter = null;

        listenerReverbEffect?.Dispose();
        listenerReverbEffect = null;

        foreach (var effect in groupedReverbEffects)
            effect.Dispose();

        groupedReverbEffects.Clear();
    }

    void InitializeScene()
    {
        foreach (var child in SceneRoot.GetChildren())
            AddPrimitive(child, vaudio.MaterialType.Air, true);

        // Listen for scene tree changes
        GetTree().NodeAdded += OnNodeAdded;
        GetTree().NodeRemoved += OnNodeRemoved;
    }

    public override void _ExitTree()
    {
        if (Engine.IsEditorHint())
            return;

        ALManager.instance.UnregisterDeviceDestroyedCallback(OnDeviceDestroyed);
        ALManager.instance.UnregisterDeviceRecreatedCallback(OnDeviceRecreated);

        GetTree().NodeAdded -= OnNodeAdded;
        GetTree().NodeRemoved -= OnNodeRemoved;

        // Remove vercidium_audio_* metadata fields from all nodes in the scene
        RemovePrimitive(SceneRoot, true);

        world?.Dispose();
    }

    // This fires for the new parent node AND each of its child nodes separately
    //  Parent node is invoked first
    void OnNodeAdded(Node node) => AddPrimitive(node, vaudio.MaterialType.Air, false);

    // This fires for the new parent node AND each of its child nodes separately
    //  Child nodes are invoked first
    void OnNodeRemoved(Node node) => RemovePrimitive(node, false);

    bool NoListenerErrorLogged;

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        if (listener == null)
        {
            if (!NoListenerErrorLogged)
            {
                LogError($"Failed to update node {Name} because there is no main listener. Ensure a VAEmitter exists with `IsMainListener` set to true");
                NoListenerErrorLogged = true;
            }

            return;
        }

        listener.emitter.PermeationColor = new vaudio.Color(255, 255, 0);

        // Sync the AL listener to our main listener
        if (GodotOpenALEnabled)
        {
            ALManager.instance.ListenerPosition = listener.GlobalPosition;
            ALManager.instance.ListenerPitch = listener.Pitch;
            ALManager.instance.ListenerYaw = listener.Yaw;
        }

        // Render the debug window from the perspective of the main listener
        world.CameraPosition = ToVAudio(listener.GlobalPosition);
        world.CameraPitch = listener.Pitch;
        world.CameraYaw = listener.Yaw;
        world.FieldOfView = float.DegreesToRadians(90);

        world.Update();
    }

}
