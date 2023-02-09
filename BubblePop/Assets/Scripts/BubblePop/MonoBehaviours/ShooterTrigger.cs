using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Entitas;

public class ShooterTrigger : ViewManager, IPointerDownHandler, IPointerUpHandler {
    public BubbleView bubble;
    public LineRenderer line;
    public TargetIndicator targetIndicator;
    
    bool holding;
    bool aiming;
    Vector2 direction;
    List<Vector3> linePoints = new List<Vector3>();
    int targetIdx; 

    public void UpdateBubble(int value) {
        bubble.UpdateView(-1, value); 
    }

    public void AnimateBubbleSpawn(int value) {
        UpdateBubble(value);
        bubble.AnimateSpawn();
    }
    
    public void OnPointerDown(PointerEventData d) {
        if (c.IsNextRoundBlocked()) return;
        holding = true;
    }

    public void OnPointerUp(PointerEventData d) {
        if (holding && aiming) {
            Shoot();
        }
        holding = false;
        line.gameObject.SetActive(false);
        targetIndicator.gameObject.SetActive(false);
    }

    void Update() {
        if (!holding) return;
        var mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        var localMousePos = (Vector2)transform.InverseTransformPoint(mousePos);
        direction = localMousePos.normalized;
        if (localMousePos.y < 0) {
            direction = -direction; //slingshot mode
        }
        var angle = Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        aiming = angle > 15 && angle < 165
            && localMousePos.magnitude > GridExtensions.CELL_WIDTH; 
        if (aiming) {
            RenderLine();
        }
        line.gameObject.SetActive(aiming);
        targetIndicator.gameObject.SetActive(aiming);
    }

    void Shoot() {
        if (targetIdx == -1) return;
        var e = c.state.GetSingleEntity<ShooterBubble>();
        e!.Add<GridIndex>(targetIdx);
        c.state.Add<ShotPath>(linePoints);
        
        c.PlaySound("shoot");
    }

    //check collisons by raycasting
    void RenderLine() {
        CalculateLinePoints(out var v);
        
        if (v == null) {
            targetIdx = -1;
            return;
        }
        
        line.positionCount = linePoints.Count;
        line.SetPositions(linePoints.ToArray());

        var lastHitPos = c.GetManager().bubbleGrid.transform.InverseTransformPoint(linePoints.Last()); 
        targetIdx = GridExtensions.GetEmptyNeighbourClosestToPoint(c, v.idx, lastHitPos);
        if (targetIdx == -1) return;
        
        targetIndicator.UpdateView(targetIdx, bubble.val);
    }

    void CalculateLinePoints(out BubbleView v) {
        var center = transform.position;
        center.z = 0;
        var hit = Physics2D.Raycast(center, direction, 1920);
        linePoints.Clear();
        linePoints.Add(center);
        linePoints.Add(hit.point);
        if (!hit.transform.TryGetComponent<BubbleView>(out v)) {
            v = CalculateLineRelections(direction);
        }
    }

    BubbleView CalculateLineRelections(Vector2 inDir) {
        if (linePoints.Count > 10) {
            throw new Exception("too many relfections. min shot angle is constrained to above 15deg");
        }
        var normal = inDir.x > 0 ? Vector2.right : Vector2.left;
        var reboundPos = linePoints.Last();
        if (inDir.x > 0) {
            reboundPos.x -= .0001f; //add negligible offset to avoid reflection colliding with source collider
        } else {
            reboundPos.x += .0001f;
        }
        var newDir = Vector2.Reflect(inDir, normal);
        var hit = Physics2D.Raycast(reboundPos, newDir, 1920);
        linePoints.Add(hit.point);
        if (hit.transform.TryGetComponent<BubbleView>(out var v)) {
            return v;
        }
        return CalculateLineRelections(newDir);
    }
}
