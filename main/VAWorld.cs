namespace vaudio_godot_openal;

[Tool]
public partial class VAWorld : Node
{
    // Temp
    public List<vaudio.Emitter> emitters = [];

    public vaudio.Emitter CreateEmitter(VAEmitter node, Action OnRaytracingComplete, Action<vaudio.Emitter> OnRaytracedByAnotherEmitter)
    {
        // RaytraceOnce emitters cast their rays once and are done — freeze their position at that
        // moment (e.g. a dodgeball impact SFX) instead of tracking the node forever afterward.
        var position = node.RaytraceOnce
            ? (vaudio.IPosition)ToVAudio(node.GlobalPosition)
            : new vaudio.FuncPosition(() => ToVAudio(node.GlobalPosition));

        var emitter = new vaudio.Emitter
        {
            Name = node.Name,
            Position = position,
            OnRaytracingComplete = OnRaytracingComplete,
            OnRaytracedByAnotherEmitter = OnRaytracedByAnotherEmitter,

            // Reverb
            ReverbRayCount = node.ReverbRayCount,
            ReverbBounceCount = node.ReverbBounceCount,
            ReverbEnergyCap = node.ReverbEnergyCap,
            MaxEchogramTime = node.MaxEchogramTime,
            EchogramGranularity = node.EchogramGranularity,
            AffectsGroupedEAX = node.AffectsGroupedEAX,
            AffectsEAXAfterRemoval = node.AffectsEAXAfterRemoval,
            HasRelativeReverb = node.HasRelativeReverb,
            RelativeReverbInnerThreshold = node.RelativeReverbInnerThreshold,
            RelativeReverbOuterThreshold = node.RelativeReverbOuterThreshold,

            // Muffling
            OcclusionRayCount = node.OcclusionRayCount,
            OcclusionBounceCount = node.OcclusionBounceCount,
            PermeationRayCount = node.PermeationRayCount,
            PermeationBounceCount = node.PermeationBounceCount,
            OcclusionEnergyCap = node.OcclusionEnergyCap,
            PermeationEnergyCap = node.PermeationEnergyCap,

            // Ambience
            AmbientOcclusionRayCount = node.AmbientOcclusionRayCount,
            AmbientOcclusionBounceCount = node.AmbientOcclusionBounceCount,
            AmbientPermeationRayCount = node.AmbientPermeationRayCount,
            AmbientPermeationBounceCount = node.AmbientPermeationBounceCount,
            AmbientOcclusionEnergyCap = node.AmbientOcclusionEnergyCap,
            AmbientPermeationEnergyCap = node.AmbientPermeationEnergyCap,

            // Visualisation
            VisualisationRayCount = node.VisualisationRayCount,
            VisualisationBounceCount = node.VisualisationBounceCount,
            VisualisationUpdateFrequency = node.VisualisationUpdateFrequency,

            // Debug rendering
            TrailColor = ToVAudio(node.TrailColor),
            OcclusionColor = ToVAudio(node.OcclusionColor),
            PermeationColor = ToVAudio(node.PermeationColor),
            AmbientPermeationColor = ToVAudio(node.AmbientPermeationColor),

            // Advanced
            Type = node.Type,
            RefreshRayCount = node.RefreshRayCount,
            RefreshDistanceThreshold = node.RefreshDistanceThreshold,
            ScatteringSeed = node.ScatteringSeed,
            ClampPosition = node.ClampPosition,
            ReservedEmitterTargets = node.ReservedEmitterTargets,
        };

        world.AddEmitter(emitter);

        if (node.IsMainListener)
        {
            if (listener == null)
            {
                listener = node;
            }
            else
            {
                LogWarning($"The {listener.Name} node has already been set as the IsMainListener node, but the {node.Name} node also has IsMainListener set to true. Only one node can be the main listener");
            }
        }
        else
        {
            if (listener == null)
            {
                LogWarning($"Emitters cannot be added before the main listener emitter is created. Ensure a VAEmitter node exists as a child node of VAWorld, with IsMainListener set to true");
            }
            else
            {
                listener.AddTarget(emitter);
            }
        }

        emitters.Add(emitter);
        return emitter;
    }

    public void RemoveEmitter(vaudio.Emitter emitter)
    {
        Debug.Assert(emitter != null);

        // Don't untarget/forget the emitter yet: if it's going into a pending-removal reverb tail,
        // it must keep being raytraced as a target of the listener until it's actually gone.
        var existingCallback = emitter.OnEmitterRemoved;
        emitter.OnEmitterRemoved = () =>
        {
            emitters.Remove(emitter);
            listener.RemoveTarget(emitter);
            existingCallback?.Invoke();
        };

        world.RemoveEmitter(emitter);
    }

    // Log to both - in case we're launched from vs2026 or from the Godot Editor
    public Action<string> Log = (message) =>
    {
        var prefixed = $"[vaudio-godot-openal] {message}";

        Console.WriteLine(prefixed);
        GD.Print(prefixed);
    };

    public Action<string> LogWarning = (message) =>
    {
        var prefixed = $"[vaudio-godot-openal] {message}";

        Console.WriteLine(prefixed);
        GD.PushWarning(prefixed);
    };

    public Action<string> LogError = (message) =>
    {
        var prefixed = $"[vaudio-godot-openal] {message}";

        Console.Error.WriteLine(prefixed);
        GD.PushError(prefixed);
    };

}
