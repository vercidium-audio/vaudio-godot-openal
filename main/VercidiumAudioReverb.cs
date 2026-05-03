namespace vaudio_godot_openal;

public partial class VercidiumAudio : Node
{
    public ALReverbEffect GetReverbEffect(vaudio.Emitter emitter)
    {
        if (emitter.AffectsGroupedEAX && emitter.GroupedEAXIndex >= 0)
        {
            if (emitter.GroupedEAXIndex >= groupedReverbEffects.Count)
            {
                LogWarning($"Emitter {emitter.Name} has a grouped EAX index of {emitter.GroupedEAXIndex} but only {groupedReverbEffects.Count} EAX presets are available.");
                return listenerReverbEffect;
            }

            return groupedReverbEffects[emitter.GroupedEAXIndex];
        }

        return listenerReverbEffect;
    }

    public ALReverbEffect GetReverbEffect(VercidiumAudioEmitter emitter)
    {
        if (emitter.AffectsGroupedEAX && emitter.GroupedEAXIndex >= 0)
        {
            if (emitter.GroupedEAXIndex >= groupedReverbEffects.Count)
            {
                LogWarning($"Emitter {emitter.Name} has a grouped EAX index of {emitter.GroupedEAXIndex} but only {groupedReverbEffects.Count} EAX presets are available.");
                return listenerReverbEffect;
            }

            return groupedReverbEffects[emitter.GroupedEAXIndex];
        }

        return listenerReverbEffect;
    }

    void OnReverbUpdated()
    {
        // Update ambient gain (if reverb enabled)
        if (listener.ProcessedReverb != null)
        {
            var ambientClarityLF = listener.ProcessedReverb.OutsidePercent;
            var ambientClarityHF = listener.ProcessedReverb.OutsidePercent;
            
            // * 2 because half go into the terrain
            ambientClarityLF += listener.AmbientPermeationGainLF;
            ambientClarityHF += listener.AmbientPermeationGainHF;

            ambientClarityLF = MathF.Min(1, ambientClarityLF);
            ambientClarityHF = MathF.Min(1, ambientClarityHF);

            if (GodotOpenALEnabled)
            {
                ambientFilter ??= new(ambientClarityLF, ambientClarityHF);
                ambientFilter.SetGain(ambientClarityLF, ambientClarityHF);
            }
        }

        // Apply raytraced EAX results to ALReverbEffects
        if (listener.EAX != null)
            CopyReverb(listener.EAX, listenerReverbEffect, false);

        for (int i = 0; i < context.GroupedEAX.Count; i++)
        {
            if (groupedReverbEffects.Count <= i)
                groupedReverbEffects.Add(new());

            CopyReverb(context.GroupedEAX[i], groupedReverbEffects[i], true);

            groupedReverbEffects[i].Update();
        }
    }

    void CopyReverb(vaudio.EAXReverbResults eax, ALReverbEffect effect, bool isGroupedEAX)
    {
        effect.gain = 1f;

        // Density causes static when updating in real time
        //  See OpenAL Soft GitHub issue: https://github.com/kcat/openal-soft/issues/1229
        effect.density = 0.5f;//eax.Density;

        effect.diffusion = eax.Diffusion;
        effect.gainLF = eax.GainLF;
        effect.gainHF = eax.GainHF;
        effect.decayTime = eax.DecayTime;
        effect.decayLFRatio = eax.DecayLFRatio;
        effect.decayHFRatio = eax.DecayHFRatio;
        effect.reflectionsDelay = eax.ReflectionsDelay;
        effect.reflectionsGain = eax.ReflectionsGain;
        effect.lateReverbGain = eax.LateReverbGain;
        effect.lateReverbDelay = eax.LateReverbDelay;
        effect.echoTime = eax.EchoTime;
        effect.echoDepth = eax.EchoDepth;
        effect.modulationTime = eax.ModulationTime;
        effect.modulationDepth = eax.ModulationDepth;
        effect.airAbsorptionGainHF = eax.AirAbsorptionGainHF;
        effect.hfReference = eax.HFReference;
        effect.lfReference = eax.LFReference;
        effect.roomRolloffFactor = eax.RoomRolloffFactor;
        effect.decayHFLimit = eax.DecayHFLimit;

        if (isGroupedEAX && eax.Pan.TryGetValue(listener.emitter, out var pan))
        {
            // Convert to openal
            pan = vaudio.RaytracingContext.CalculateListenerRelativePan(pan, listener.Pitch, listener.Yaw);

            effect.effectSlotGain = eax.EffectSlotGain[listener.emitter];
            effect.effectSlotGain = Math.Max(0, effect.effectSlotGain);
            effect.effectSlotGain = Math.Min(1, effect.effectSlotGain);

            // TODO - separate pan for late reverb and reflections
            effect.lateReverbPan[0] = pan.X;
            effect.lateReverbPan[1] = pan.Y;
            effect.lateReverbPan[2] = pan.Z;

            effect.reflectionsPan[0] = pan.X;
            effect.reflectionsPan[1] = pan.Y;
            effect.reflectionsPan[2] = pan.Z;
        }

        effect.dirty = true;
        effect.Update();
    }
}
