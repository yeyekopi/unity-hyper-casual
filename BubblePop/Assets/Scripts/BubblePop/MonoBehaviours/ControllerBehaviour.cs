using System;
using DG.Tweening;
using Entitas;
using Entitas.VisualDebugging.Unity;
using UnityEngine;

public class ControllerBehaviour : MonoBehaviour {
    Controller _controller;
    
    void Awake() {
        DOTween.Init();
        ThreadUtils.SetMainThread(true);
        ThreadUtils.SetEntitasThread(true);
        ComponentLookup.InitializeAutomatic();
        var c = new Contexts();
#if UNITY_EDITOR
        c.contextObserver = new ContextObserver(c.state);
#endif
        _controller = new Controller(c);
    }

    void Start() => _controller.Initialize();
    void Update() => _controller.Execute();
}
