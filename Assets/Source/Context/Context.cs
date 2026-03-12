using System.Collections.Generic;
using UnityEngine;

public class Context : MonoBehaviour
{
    [SerializeField] private List<Installer> _installers;

    public void Initialize()
    {
        gameObject.name = "[Context]";

        foreach (var installer in _installers)
            installer.Install();

        DontDestroyOnLoad(gameObject);
    }
}
