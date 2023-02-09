using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static RenderingExtensions;

public class BubbleView : ViewManager {
    public Image bubbleImg;
    public TMP_Text txt;
    
    public int idx;
    public int val;
    
    public void UpdateView(int _idx, int _val) {
        idx = _idx;
        if (_val == val) return;
        val = _val;

        txt.text = val.ToString();
        txt.colorGradient = GetOrCreateVertexGradient(val); 
        txt.fontMaterial = GetOrCreateHueMaterial(txt.fontMaterial, val, isText: true);
        bubbleImg.material = GetOrCreateHueMaterial(bubbleImg.material, val, isText: false);
    }

    public Sequence AnimateUpgrade(int oldVal, int newVal, int mergeRef) {
        var seq = AnimatePop(oldVal, mergeRef, pushBubble: false);
        return seq
            .AppendInterval(.1f)
            .AppendCallback(() => {
                var lightning = c.GetManager().bubbleGrid.lightningPool.Get();
                lightning.PlayPs(newVal);
                lightning.transform.localPosition = transform.localPosition;
                DOVirtual.DelayedCall(1, () => c.GetManager().bubbleGrid.lightningPool.Push(lightning));
            })
            .AppendInterval(.1f)
            .AppendCallback(() => UpdateView(idx, newVal))
            .AppendInterval(.1f);
    }
    
    public Sequence AnimatePop(int value, int mergeRef, bool pushBubble = true) {
        return DOTween.Sequence()
            .AppendCallback(() => {
                var explosion = c.GetManager().bubbleGrid.explosionPool.Get();
                explosion.PlayPs(value);
                explosion.transform.localPosition = transform.localPosition;
                DOVirtual.DelayedCall(1, () => c.GetManager().bubbleGrid.explosionPool.Push(explosion));
            })
            .AppendInterval(.1f)
            .AppendCallback(() => {
                c.IncrementViewScore(value, mergeRef);
                if (pushBubble) {
                    c.GetManager().bubbleGrid.PushBubble(this);
                }
            });
    }

    public Sequence AnimateFall(Vector3 fromPos, int value, int mergeRef) {
        fromPos.x += Random.Range(-5f, 5f);
        transform.localPosition = fromPos;
        var floorYPos = fromPos.y - 640; 
        var explodeT = Random.Range(.5f, 1f);
        return AnimatePop(value, mergeRef)
            .Prepend(transform.DOLocalMoveY(floorYPos, explodeT).SetEase(Ease.InCubic))
            .InsertCallback(explodeT, () => c.PlaySound(new SoundInfo("explosion", volume: .25f)));
    }
        
    public Tween AnimateSpawn() {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        return transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBounce);
    }
    
}
