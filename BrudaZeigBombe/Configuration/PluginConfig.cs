using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using UnityEngine;
using UnityEngine.Events;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BrudaZeigBombe.Configuration;

internal class PluginConfig
{
    public virtual bool Enabled { get; set; } = true;
    public virtual Color Color { get; set; } = new(0.28f, 1.0f, 0.4f);
    public virtual float Opacity { get; set; } = 1.0f;
    public virtual bool AlwaysOnTop { get; set; } = false;
    public virtual bool HmdOnly { get; set; } = false;
    
    public virtual void Changed() => PropertyChanged.Invoke();

    [Ignore]
    public readonly UnityEvent PropertyChanged = new();
}