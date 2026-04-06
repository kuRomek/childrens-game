using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Scene, SerializeField] private int _sceneId;
    [SerializeField] private List<MeshRenderer> _highlightingElements;

    public void ToggleHighlight(bool isActive)
    {
        foreach (var element in _highlightingElements)
            element.renderingLayerMask = Utils.RenderLayers.Default + (isActive ? Utils.RenderLayers.Outline2 : 0);
    }

    public void Interact()
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton<RunStart>();
        SceneManager.LoadScene(_sceneId);
    }
}
