// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 12:56
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using DOVector3 = UnityEngine.Vector3;
using DOQuaternion = UnityEngine.Quaternion;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine.Audio;
using Object = UnityEngine.Object;
using UnityEngine.UI;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    public class DOMoveCore : TweenerCore<Vector3, Vector3, VectorOptions> {
        public static event Action<Transform, DOMoveCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOAspectCore : TweenerCore<float, float, FloatOptions> { 
        public static event Action<Camera, DOAspectCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Camera)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOColorCore : TweenerCore<Color, Color, ColorOptions> { 
        public static event Action<UnityEngine.Object, DOColorCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((UnityEngine.Object)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOOrthoSizeCore : TweenerCore<float, float, FloatOptions> { 
        public static event Action<Camera, DOOrthoSizeCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Camera)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOPixelRectCore : TweenerCore<Rect, Rect, RectOptions> { 
        public static event Action<Camera, DOPixelRectCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Camera)streamingData.target, this);
            }
            return start;
        }
    }
    public class DORectCore : TweenerCore<Rect, Rect, RectOptions> { 
        public static event Action<Camera, DORectCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Camera)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOIntensityCore : TweenerCore<float, float, FloatOptions> { 
        public static event Action<Light, DOIntensityCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Light)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOFadeCore : TweenerCore<Color, Color, ColorOptions> { 
        public static event Action<Object, DOFadeCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Object)streamingData.target, this);
            }
            return start;
        }
    }

    public class DOFloatCore : TweenerCore<float, float, FloatOptions> {
        internal string property;

        public static event Action<Material, DOFloatCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Material)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOTimeCore : TweenerCore<float, float, FloatOptions> { 
        public static event Action<TrailRenderer, DOTimeCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((TrailRenderer)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOLocalMoveCore : TweenerCore<Vector3, Vector3, VectorOptions> { 
        public static event Action<Transform, DOLocalMoveCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DORotateCore : TweenerCore<DOQuaternion, DOVector3, QuaternionOptions> { 
        public static event Action<Transform, DORotateCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DORotateQuaternionCore : TweenerCore<DOQuaternion, DOQuaternion, NoOptions> { 
        public static event Action<Transform, DORotateQuaternionCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOLocalRotateCore : TweenerCore<DOQuaternion, DOVector3, QuaternionOptions> { 
        public static event Action<Transform, DOLocalRotateCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOLocalRotateQuaternionCore : TweenerCore<Quaternion, Quaternion, NoOptions> { 
        public static event Action<Transform, DOLocalRotateQuaternionCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOScaleCore : TweenerCore<Vector3, Vector3, VectorOptions> { 
        public static event Action<Transform, DOScaleCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOBlendableLocalRotateByCore : TweenerCore<DOQuaternion, DOVector3, QuaternionOptions> { 
        public static event Action<Transform, DOBlendableLocalRotateByCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    
    public class DOBlendableRotateByCore : TweenerCore<DOQuaternion, DOVector3, QuaternionOptions> { 
        public static event Action<Transform, DOBlendableRotateByCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            }
            return start;
        }
    }
    public class DOBlendableLocalMoveByCore : TweenerCore<DOVector3, DOVector3, VectorOptions> {
        public static event Action<Transform, DOBlendableLocalMoveByCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOShakeScaleCore : TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> {
        public static event Action<Transform, DOShakeScaleCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOShakeRotationCore : TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> {
        public static event Action<Transform, DOShakeRotationCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOShakePositionCore : TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> {
        public static event Action<Transform, DOShakePositionCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOPunchScaleCore : TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> {
        public static event Action<Transform, DOPunchScaleCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOPunchRotationCore : TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> {
        public static event Action<Transform, DOPunchRotationCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOPunchPositionCore : TweenerCore<DOVector3, DOVector3[], Vector3ArrayOptions> {
        public static event Action<Transform, DOPunchPositionCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOLookAtCore : TweenerCore<DOQuaternion, DOVector3, QuaternionOptions> {
        public static event Action<Transform, DOLookAtCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Transform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    
    
    
    public class DOFadeCoreFloat : TweenerCore<float, float, FloatOptions> {
        public static event Action<Component, DOFadeCoreFloat> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Component)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOSetFloatCore : TweenerCore<float, float, FloatOptions> {
        internal string floatName;

        public static event Action<AudioMixer, DOSetFloatCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((AudioMixer)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOValueCore : TweenerCore<float, float, FloatOptions> {
        public static event Action<Slider, DOValueCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Slider)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOFillAmountCore : TweenerCore<float, float, FloatOptions> {
        public static event Action<Image, DOFillAmountCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((Image)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOAnchorPosCore : TweenerCore<Vector2, Vector2, VectorOptions> {
        public static event Action<RectTransform, DOAnchorPosCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((RectTransform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOAnchorPos3DCore : TweenerCore<Vector3, Vector3, VectorOptions> {
        public static event Action<RectTransform, DOAnchorPos3DCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((RectTransform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOAnchorMaxCore : TweenerCore<Vector2, Vector2, VectorOptions> {
        public static event Action<RectTransform, DOAnchorMaxCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((RectTransform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOAnchorMinCore : TweenerCore<Vector2, Vector2, VectorOptions> {
        public static event Action<RectTransform, DOAnchorMinCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((RectTransform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOSizeDeltaCore : TweenerCore<Vector2, Vector2, VectorOptions> {
        public static event Action<RectTransform, DOSizeDeltaCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((RectTransform)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOHorizontalNormalizedPosCore : TweenerCore<float, float, FloatOptions> {
        public static event Action<ScrollRect, DOHorizontalNormalizedPosCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((ScrollRect)streamingData.target, this);
            } 
            return start;
        }
    }
    
    public class DOVerticalNormalizedPosCore : TweenerCore<float, float, FloatOptions> {
        public static event Action<ScrollRect, DOVerticalNormalizedPosCore> TrackStartup;
        internal override bool Startup() {
            var start = base.Startup();
            if (start && streamingData.HasValue) {
                TrackStartup?.Invoke((ScrollRect)streamingData.target, this);
            } 
            return start;
        }
    }
}