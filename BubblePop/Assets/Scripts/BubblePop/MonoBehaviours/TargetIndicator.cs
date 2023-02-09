using UnityEngine;
using UnityEngine.UI;
using static GridExtensions;

public class TargetIndicator : ViewManager {
    public Image crosshairImg;

    public void UpdateView(int targetIdx, int val) {
        var row = GetRowFromGridIndex(targetIdx, out var  _);
        var targetPos = GridExtensions.GetPositionFromGridIndex(c, targetIdx);
        transform.localPosition = targetPos;

        var isGameOverTarget = row == GRID_ROWS - 1;
        var brightness = isGameOverTarget ? 0 : 30;
        crosshairImg.material.SetFloat(RenderingExtensions.BrightnessProp, brightness);
    }

}
