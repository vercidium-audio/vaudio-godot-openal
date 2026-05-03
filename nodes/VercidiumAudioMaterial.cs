namespace vaudio_godot_openal;

/// <summary>
/// Custom acoustic material resource for the Vercidium Audio plugin.
/// Must be defined as a child Node of a VercidiumAudio node.
/// Can be created in the Godot editor and assigned to collision shapes.
/// </summary>
[Tool]
[GlobalClass]
public partial class VercidiumAudioMaterial : Node
{
    private int _materialType = 1000;
    private string _materialName = "CustomMaterial";

    VercidiumAudio vercidiumAudio;
    vaudio.MaterialProperties vaudioMaterial;

    public override void _EnterTree()
    {
        if (Engine.IsEditorHint())
            return;

        vercidiumAudio = this.GetVercidiumAudioParent();

        // Prevent duplicates
        if (vercidiumAudio.customMaterials.TryGetValue(MaterialType, out VercidiumAudioMaterial value))
        {
            VercidiumAudio.LogError($"The VercidiumAudioMaterial node {Name} has the same material ID ({MaterialType}) as the VercidiumAudioMaterial node {value.Name}. Please change this to another ID");

            return;
        }

        // Create the vaudio material
        vaudioMaterial = new vaudio.MaterialProperties(
            AbsorptionLF,
            AbsorptionHF,
            Scattering,
            TransmissionLF,
            TransmissionHF,
            PlaneTransmissionLF,
            PlaneTransmissionHF
        );

        vercidiumAudio.context.AddMaterial((vaudio.MaterialType)MaterialType, vaudioMaterial, GetDebugColor());
        vercidiumAudio.customMaterials[MaterialType] = this;
    }

    bool firstSet = true;

    /// <summary>
    /// Unique material ID. Must be >= 1000 to avoid conflicts with built-in materials
    /// </summary>
    [Export(PropertyHint.Range, "1000,9999,1,or_greater,no_slider")]
    public int MaterialType
    {
        get => _materialType;
        set
        {
            if (!firstSet && !Engine.IsEditorHint())
            {
                VercidiumAudio.LogWarning($"Cannot change the type of VercidiumAudioMaterial nodes at runtime. Node: {Name}");
                return;
            }

            _materialType = value;
            UpdateConfigurationWarnings();

            firstSet = false;
        }
    }

    /// <summary>
    /// Material name for debugging and identification
    /// </summary>
    [Export(PropertyHint.None, "")]
    public string MaterialName
    {
        get => _materialName;
        set
        {
            _materialName = value;
            UpdateConfigurationWarnings();
        }
    }

    float _AbsorptionLF = 0.02f;
    float _AbsorptionHF = 0.1f;
    float _Scattering = 0.1f;
    float _TransmissionLF = 50;
    float _TransmissionHF = 100f;
    float _PlaneTransmissionLF = 0.1f;
    float _PlaneTransmissionHF = 0.25f;
    Color _DebugColor = new(1, 0, 1);

    /// <summary>
    /// Low-frequency absorption coefficient (0.0 to 1.0)
    /// </summary>
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float AbsorptionLF
    { 
        get => _AbsorptionLF;
        set
        {
            // Prevent redundant sets
            if (value == _AbsorptionLF)
                return;

            _AbsorptionLF = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.AbsorptionLF = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// High-frequency absorption coefficient (0.0 to 1.0)
    /// </summary>
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float AbsorptionHF
    {
        get => _AbsorptionHF;
        set
        {
            // Prevent redundant sets
            if (value == _AbsorptionHF)
                return;

            _AbsorptionHF = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.AbsorptionHF = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// Scattering coefficient (0.0 to 1.0)
    /// </summary>
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float Scattering
    {
        get => _Scattering;
        set
        {
            // Prevent redundant sets
            if (value == _Scattering)
                return;

            _Scattering = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.Scattering = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// Low-frequency transmission in dB/m (0.0 or greater)
    /// </summary>
    [Export(PropertyHint.Range, "0.0,10000.0")]
    public float TransmissionLF
    {
        get => _TransmissionLF;
        set
        {
            // Prevent redundant sets
            if (value == _TransmissionLF)
                return;

            _TransmissionLF = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.TransmissionLF = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// High-frequency transmission in dB/m (0.0 or greater)
    /// </summary>
    [Export(PropertyHint.Range, "0.0,10000.0")]
    public float TransmissionHF
    {
        get => _TransmissionHF;
        set
        {
            // Prevent redundant sets
            if (value == _TransmissionHF)
                return;

            _TransmissionHF = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.TransmissionHF = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// Percentage of low-frequency energy lost when a ray passes through a flat primitive
    /// </summary>
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float PlaneTransmissionLF
    {
        get => _PlaneTransmissionLF;
        set
        {
            // Prevent redundant sets
            if (value == _PlaneTransmissionLF)
                return;

            _PlaneTransmissionLF = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.PlaneTransmissionLF = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// Percentage of high-frequency energy lost when a ray passes through a flat primitive
    /// </summary>
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float PlaneTransmissionHF
    {
        get => _PlaneTransmissionHF;
        set
        {
            // Prevent redundant sets
            if (value == _PlaneTransmissionHF)
                return;

            _PlaneTransmissionHF = value;

            if (vercidiumAudio != null)
            {
                vaudioMaterial.PlaneTransmissionHF = value;
                vercidiumAudio.context.MaterialsDirty = true;
            }
        }
    }

    /// <summary>
    /// Debug color for the VAudio debug renderer
    /// </summary>
    [Export]
    public Color DebugColor
    { 
        get => _DebugColor;        
        set
        {
            _DebugColor = value;

            vercidiumAudio?.context.SetMaterialColor((vaudio.MaterialType)MaterialType, GetDebugColor());
        }
    }

    /// <summary>
    /// Gets the debug color as a vaudio.Color
    /// </summary>
    public vaudio.Color GetDebugColor() => new(DebugColor.R, DebugColor.G, DebugColor.B, 1.0f);

    /// <summary>
    /// Validates the material configuration and returns warnings
    /// </summary>
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (MaterialType < 1000)
        {
            warnings.Add($"Material ID must be >= 1000 (current: {MaterialType}). IDs 0-999 are reserved for built-in materials.");
        }

        if (string.IsNullOrWhiteSpace(MaterialName))
        {
            warnings.Add("Material Name should not be empty.");
        }

        return [.. warnings];
    }
}
