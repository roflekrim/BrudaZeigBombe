using System;
using System.Collections.Generic;
using System.Linq;
using BrudaZeigBombe.Configuration;
using IPA.Utilities;
using SiraUtil.Affinity;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace BrudaZeigBombe.Affinity_Patches;

internal class BombPatch : IAffinity, IDisposable
{
    private static readonly int ShaderPropColorID = Shader.PropertyToID("_Color");
    private static readonly int ShaderPropAlphaID = Shader.PropertyToID("_Alpha");
    private static readonly int ShaderPropBloomID = Shader.PropertyToID("_Bloom");
    private static readonly int ShaderPropZTestID = Shader.PropertyToID("_ZTest");
    private static readonly int ShaderPropFresnelModeID = Shader.PropertyToID("_FresnelMode");
    private static readonly int ShaderPropFresnelBiasID = Shader.PropertyToID("_FresnelBias");
    private static readonly int ShaderPropFresnelScaleID = Shader.PropertyToID("_FresnelScale");
    private static readonly int ShaderPropFresnelPowerID = Shader.PropertyToID("_FresnelPower");
    private static readonly int ShaderPropCullModeID = Shader.PropertyToID("_CullMode");
    
    private static readonly FieldAccessor<BombNoteController, CuttableBySaber>.Accessor BombCuttable =
        FieldAccessor<BombNoteController, CuttableBySaber>.GetAccessor("_cuttableBySaber");
    
    private readonly PluginConfig _config;
    private readonly Material _material;
    private List<GameObject> _highlights;

    [Inject]
    public BombPatch(PluginConfig config, [Inject(Id = "BZB_Material")] Material material)
    {
        _config = config;
        _material = material;
        _highlights = new List<GameObject>();

        // there is a very good chance that there is a better way to do this
        _material.SetColor(ShaderPropColorID, _config.Color);
        _material.SetFloat(ShaderPropAlphaID, _config.Opacity);
        _material.SetFloat(ShaderPropBloomID, 14);
        _material.SetInt(ShaderPropZTestID, (int) UnityEngine.Rendering.CompareFunction.Always);
        _material.SetInt(ShaderPropFresnelModeID, 1);
        _material.SetFloat(ShaderPropFresnelBiasID, 0.1f);
        _material.SetFloat(ShaderPropFresnelScaleID, 2f);
        _material.SetFloat(ShaderPropFresnelPowerID, 3.5f);
        _material.SetInt(ShaderPropCullModeID, (int) UnityEngine.Rendering.CullMode.Back);
        _material.renderQueue = _config.AlwaysOnTop ? 2005 : 2003;
        
        _config.PropertyChanged.AddListener(ConfigChanged);
    }

    public void Dispose()
    {
        _config.PropertyChanged.RemoveListener(ConfigChanged);
    }

    private void ConfigChanged()
    {
        _material.SetColor(ShaderPropColorID, _config.Color);
        _material.SetFloat(ShaderPropAlphaID, _config.Opacity);
        _material.renderQueue = _config.AlwaysOnTop ? 2005 : 2003;
        
        _highlights = _highlights.Where(x => x != null).ToList();
        foreach (var gameObject in _highlights)
        {
            if (!_config.Enabled)
            {
                Object.Destroy(gameObject);
                return;
            }
            
            gameObject.layer = _config.HmdOnly ? 24 : 8;

            if (gameObject.transform.parent.gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
                meshRenderer.sharedMaterial.renderQueue = _config.AlwaysOnTop ? 2005 : 2003;
        }
        
        if (!_config.Enabled)
            _highlights.Clear();
    }
    
    [AffinityPostfix]
    [AffinityPatch(typeof(BombNoteController), nameof(BombNoteController.Init))]
    // ReSharper disable once InconsistentNaming
    internal void Postfix_Bomb(BombNoteController __instance)
    {
        if (__instance.noteTransform.Find("BZB_Highlight") || !_config.Enabled) return;
            
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (go is null) return;
        if (go.TryGetComponent<SphereCollider>(out var sphereCollider))
            Object.Destroy(sphereCollider);

        var diameter = BombCuttable(ref __instance).radius * 2;
        go.layer = _config.HmdOnly ? 24 : 8;
        go.name = "BZB_Highlight";
        go.transform.parent = __instance.noteTransform;
        go.transform.localScale = new Vector3(diameter, diameter, diameter);
        go.transform.localPosition = Vector3.zero;

        if (go.TryGetComponent<MeshRenderer>(out var component))
            component.sharedMaterial = _material;
        
        _highlights.Add(go);

        if (!_config.AlwaysOnTop) return;
        if (__instance.noteTransform.gameObject.TryGetComponent<MeshRenderer>(out var meshRenderer))
            meshRenderer.sharedMaterial.renderQueue = 2005;
    }
}