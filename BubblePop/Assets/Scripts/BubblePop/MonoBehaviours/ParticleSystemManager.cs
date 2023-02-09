using UnityEngine;
using static RenderingExtensions;

public class ParticleSystemManager : MonoBehaviour {
    public ParticleSystem[] particleSystems;

    public void PlayPs(int val) {
        var color = GetColor(val);
        foreach (var ps in particleSystems) {
            var colorM = ps.colorOverLifetime;
            var mColor = colorM.color;
            var grad = mColor.gradient;
            var keys = grad.colorKeys;
            for (var i = 0; i < keys.Length; i++) {
                var key = keys[i];
                keys[i] = new GradientColorKey(color, key.time);
            }
            grad.colorKeys = keys;
            mColor.gradient = grad;
            colorM.color = mColor;  
        }
        particleSystems[0].Play();
    }
}
