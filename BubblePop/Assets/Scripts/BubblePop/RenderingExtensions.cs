using System;
using Entitas;
using TMPro;
using UnityEngine;

public static class RenderingExtensions {

    public static Color BUBBLE_BASE_COLOR = new Color(0.9725491f, 0.549019f, 0.07843138f, 1f);
    
    public const byte COLOR_INCREMENT = 255 / (LogicExtensions.MAX_EXPONENT - 1);
    //custom sorting to avoid similar numbers having similar colors
    public static int[] COLOR_ORDER = new int[]{ 1, 5, 10, 2, 6, 11, 3, 7, 4, 8, 9 };


    public static BubblePopManager GetManager(this Contexts c) {
        return c.state.Get<BubblePopManagerC>().value;   
    }

    public static void PlaySound(this Contexts c, SoundInfo sound) {
        c.GetManager().soundManager.PlaySound(sound);
    }

    public static void ToggleConfirmPrompt(this Contexts c, bool enabled) {
        var v = c.GetManager().confirmPrompt;
        if (enabled) {
            v.UpdateView();
            var isGameOver = c.state.Is<GameOver>();
            c.PlaySound(isGameOver ? "popup" : "congrats");
        }
        v.gameObject.SetActive(enabled);
    }
    

    public static bool IsAnimatingRound(this Contexts c) {
        return c.state.Is<Shooting>()
            || c.state.Has<AnimatingMergeEvent>()
            || c.state.Has<SpawnRows>();
    }

    public static bool IsConfirmPromptShown(this Contexts c) {
        return c.state.Is<GameOver>() || c.state.Is<LevelComplete>();
    }

    public static bool IsNextRoundBlocked(this Contexts c) {
        return c.IsAnimatingRound() || c.IsConfirmPromptShown();
    } 
    

    public static void IncrementViewScore(this Contexts c, long score, int mergeRef) {
        // var amplifier = mergeRef + 1; could implement amplifier
        c.state.Increment<ViewScore>(score);
        c.state.Increment<ViewLevelScore>(score);
    }
    

    public static byte GetHue(int value) {
        var exponent = LogicExtensions.GetExponent(value);
        return (byte)((COLOR_ORDER[exponent - 1] - 1) * COLOR_INCREMENT);
    }

    public static Color GetColor(int value, float saturation = 1, float brightness = 1, Color baseColor = default) {
        if (baseColor == default) {
            baseColor = BUBBLE_BASE_COLOR;
        }
        var hue = GetHue(value);
        var shiftFrom = new Vector3(baseColor.r, baseColor.g, baseColor.b);
        var v3 = shift_col(shiftFrom, new Vector3(hue, saturation, brightness));
        return new Color(v3.x, v3.y, v3.z);
    }

    //copied directly from ColorShifter shader (and I want to keep the formatting)
    static Vector3 shift_col(Vector3 RGB, Vector3 shift) {
        Vector3 RESULT = new Vector3();
        double VSU = shift.z*shift.y*Math.Cos(shift.x*3.14159265/180);
        double VSW = shift.z*shift.y*Math.Sin(shift.x*3.14159265/180);
        RESULT.x = (float)((.299*shift.z+.701*VSU+.168*VSW)*RGB.x
                + (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
                + (.114*shift.z-.114*VSU-.497*VSW)*RGB.z);                        
        RESULT.y = (float)((.299*shift.z-.299*VSU-.328*VSW)*RGB.x
                + (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
                + (.114*shift.z-.114*VSU+.292*VSW)*RGB.z);              
        RESULT.z = (float)((.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
                + (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
                + (.114*shift.z+.886*VSU-.203*VSW)*RGB.z);
        return (RESULT);
    }


    static int? _outlineProp;
    public static int OutlineProp => _outlineProp ??= Shader.PropertyToID("_OutlineColor");
    static int? _hueProp;
    public static int HueProp => _hueProp ??= Shader.PropertyToID("_Hue");
    static int? _brightnessProp;
    public static int BrightnessProp => _brightnessProp ??= Shader.PropertyToID("_Brightness");

    static Material[] hueMats;
    static Material[] hueTextMats;
    public static Material GetOrCreateHueMaterial(Material baseMat, int value, bool isText) {
        var idx = LogicExtensions.GetExponent(value) - 1;
        Material[] arr;
        if (isText) {
            arr = hueTextMats ??= new Material[LogicExtensions.MAX_EXPONENT];
        } else {
            arr = hueMats ??= new Material[LogicExtensions.MAX_EXPONENT];
        }
        if (arr[idx] != null) return arr[idx];
        var mat = new Material(baseMat);
        if (isText) {
            var color = GetColor(value, brightness: .65f);
            mat.SetColor(OutlineProp, color);
            mat.EnableKeyword("OUTLINE_ON");
            return arr[idx] = mat;
        }
        var hue = GetHue(value);
        mat.SetFloat(HueProp, hue);
        return arr[idx] = mat;
    }

    static VertexGradient?[] gradients;
    public static VertexGradient GetOrCreateVertexGradient(int value) {
        gradients ??= new VertexGradient?[LogicExtensions.MAX_EXPONENT];
        var idx = LogicExtensions.GetExponent(value) - 1;
        if (gradients[idx] != null) {
            return gradients[idx].Value;
        }
        var color = GetColor(value); 
        var brightColor = GetColor(value); //todo
        var grad = new VertexGradient(
            color,
            brightColor,
            color,
            color);
        gradients[idx] = grad;
        return grad;
    }

    public static void ClearFields() {
        hueMats = null;
        hueTextMats = null;
        gradients = null;
    }
}
