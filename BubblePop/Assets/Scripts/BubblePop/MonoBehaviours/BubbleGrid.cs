using System;
using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using UnityEngine;
using static GridExtensions;

public class BubbleGrid : ViewManager {
    public GameObject bubblePrefab;
    public GameObject explosionPrefab;
    public GameObject lightningPrefab;

    ObjectPool<BubbleView> bubblePool; 
    public ObjectPool<ParticleSystemManager> explosionPool;
    public ObjectPool<ParticleSystemManager> lightningPool;

    BubbleView[] gridViews = new BubbleView[CELL_COUNT];
    public BubbleView GetBubble(int idx) {
        return gridViews[idx];
    }
    public BubbleView GetOrCreateBubble(int idx, int val) {
        var v = GetBubble(idx);
        if (v != null) {
            v.gameObject.SetActive(true);
        } else {
            v = gridViews[idx] = bubblePool.Get(); //this does SetActive 
        }
        v.UpdateView(idx, val);
        return v;
    }
    public void PushBubble(BubbleView v) {
        if (IsGridIndexValid(v.idx)) {
            gridViews[v.idx] = null;
        }
        bubblePool.Push(v);
    }
    public void TryPushBubble(int gridIdx) {
        if (IsGridIndexValid(gridIdx) && gridViews[gridIdx] != null) {
            PushBubble(gridViews[gridIdx]);
        }    
    }
    public BubbleView GetOffGridBubble(int val) {
        var v = bubblePool.Get();
        v.UpdateView(-1, val);
        return v;
    }

    void Awake() {
        if (bubblePool == null) {
            bubblePool = new ObjectPool<BubbleView>(() => {
                    return Instantiate(bubblePrefab, transform).GetComponent<BubbleView>();
                },
                go => go.gameObject.SetActive(false),
                go => go.gameObject.SetActive(true));
        } 
        if (explosionPool == null) {
            explosionPool = new ObjectPool<ParticleSystemManager>(() => {
                    return Instantiate(explosionPrefab, transform).GetComponent<ParticleSystemManager>();
                },
                go => go.gameObject.SetActive(false),
                go => go.gameObject.SetActive(true));
        } 
        if (lightningPool == null) {
            lightningPool = new ObjectPool<ParticleSystemManager>(() => {
                    return Instantiate(lightningPrefab, transform).GetComponent<ParticleSystemManager>();
                },
                go => go.gameObject.SetActive(false),
                go => go.gameObject.SetActive(true));
        } 
    }

    public void Prepare(int[] grid) {
        for (var i = 0; i < grid.Length; i++) {
            var val = grid[i];
            if (val == 0) {
                TryPushBubble(i);
                continue;
            }
            var v = GetOrCreateBubble(i, val); 
            var v2 = GetPositionFromGridIndex(c, i);
            v.transform.localPosition = v2;
        }
    }
    
    //1 of 2 ways for a bubble to spawn on the grid
    public Sequence AnimateShot(int targetIdx, List<Vector3> shotPath) {
        var sourceV = c.GetManager().shooterTrigger.bubble;
        var t = GetOrCreateBubble(targetIdx, sourceV.val).transform;
        t.SetAsLastSibling();
        t.position = sourceV.transform.position;
        sourceV.gameObject.SetActive(false); //sourceV is always ShooterTrigger.bubble

        var seq = DOTween.Sequence();
        for (var i = 1; i < shotPath.Count; i++) {
            var shotPos = transform.InverseTransformPoint(shotPath[i - 1]);
            var targetPos = transform.InverseTransformPoint(shotPath[i]);
            if (i == shotPath.Count - 1) {
                targetPos = GridExtensions.GetPositionFromGridIndex(c, targetIdx, useAnimState: true);
            }
            var distance = Vector2.Distance(shotPos, targetPos);
            var time = distance / 3000f;
            seq.Append(t.DOLocalMove(targetPos, time).SetEase(Ease.Linear));
        }
        return seq;
    }
    //2 of 2 ways for a bubble to spawn on the grid
    public Sequence AnimateSpawnRows(int[] grid, int rowsToAdd) {
        var sequence = DOTween.Sequence(); 
        var totalTime = .5f;
        var timePerRow = totalTime / rowsToAdd;
        var maxNewRow = rowsToAdd - 1;
        for (var row = 0; row < GRID_ROWS; row++) {
            for (var col = 0; col < GRID_COLUMNS; col++) {
                var idx = GetGridIndexFromRowAndColumn(row, col);
                var val = grid[idx];
                if (val == 0) {
                    TryPushBubble(idx);
                    continue;
                }
                var bubble = GetOrCreateBubble(idx, val);
                var newPos = GetPositionFromGridIndex(c, idx);
                var oldY = newPos.y + rowsToAdd * CELL_HEIGHT;
                bubble.transform.localPosition = new Vector3(newPos.x, oldY);
                //InSine = slow, then fast
                sequence.Insert(0, bubble.transform.DOLocalMoveY(newPos.y, totalTime).SetEase(Ease.Linear));
                if (row <= maxNewRow) {
                    bubble.transform.localScale = Vector3.zero;
                    var t = .1f + timePerRow * (maxNewRow - row);
                    sequence.Insert(t, bubble.transform.DOScale(1, timePerRow).SetEase(Ease.OutSine));
                }
            }
        }
        return sequence;
    }
}
