using TMPro;
using Entitas;
using UnityEngine.UI;
using DG.Tweening;

public class ScorePanelView : ViewManager {
    public TMP_Text scoreTxt;
    public Slider slider; 
    public BubbleView currLvlBubble;
    public BubbleView nxtLvlBubble;

    public void UpdateScore(bool tween = false) {
        var score = c.state.GetValueOrDefault<ViewScore, long>();
        scoreTxt.text = score.ToString();

        var levelScore = c.state.GetValueOrDefault<ViewLevelScore, long>(); 
        var currentLevel = c.state.GetValueOrDefault<Level, int>();
        if (c.state.Is<LevelComplete>()) currentLevel--;
        var targetScore = LogicExtensions.GetLevelUnlockRequirement(currentLevel + 1);
        var sliderValue = levelScore / (float)targetScore;
        
        DOTween.Kill("slider");
        if (tween) {
            slider.DOValue(sliderValue, .25f).SetTarget("slider");
        } else {
            slider.value = sliderValue;
        }
    }

    public void UpdateLevel() {
        UpdateScore();
        var level = c.state.GetValueOrDefault<Level, int>();
        var colorVal = GetColorValue(level);
        slider.targetGraphic.color = RenderingExtensions.GetColor(colorVal);
        UpdateBubble(currLvlBubble, level);
        UpdateBubble(nxtLvlBubble, level + 1);
    }

    void UpdateBubble(BubbleView v, int level) {
        var colorVal = GetColorValue(level);
        v.UpdateView(-1, colorVal);
        v.txt.text = (level + 1).ToString();
    }

    int GetColorValue(int level) {
        var exponent = level % (LogicExtensions.MAX_EXPONENT - 1) + 1; 
        return LogicExtensions.GetValue(exponent);
    }
}
