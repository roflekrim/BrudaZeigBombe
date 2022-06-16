using System;
using BrudaZeigBombe.Configuration;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace BrudaZeigBombe.Managers;

internal class CameraManager : IInitializable, IDisposable
{
    private readonly PluginConfig _config;
    private readonly Camera _mainCamera;

    [Inject]
    public CameraManager(PluginConfig config, Camera mainCamera)
    {
        _config = config;
        _mainCamera = mainCamera;
    }

    public void Initialize()
    {
        if (!_config.HmdOnly || _mainCamera == null) return;
        _mainCamera.cullingMask |= 1 << 24;
    }

    public void Dispose()
    {
        _mainCamera.cullingMask &= ~(1 << 24);
    }
}