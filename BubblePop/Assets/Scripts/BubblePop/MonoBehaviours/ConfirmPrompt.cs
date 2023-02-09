using System;
using DG.Tweening;
using Entitas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPrompt : ViewManager {
   public RectTransform mainPanel;
   public TMP_Text titleTxt;
   public TMP_Text btnTxt;
   public Button confirmBtn;

   void Start() {
      confirmBtn.onClick.AddListener(OnConfirm);
   }

   void OnDestroy() {
      confirmBtn.onClick.RemoveListener(OnConfirm); 
   }
   
   void OnConfirm() {
      c.ToggleConfirmPrompt(false);
   }

   void OnEnable() {
      var targetScale = mainPanel.localScale; 
      mainPanel.localScale = new Vector3(.00001f, .00001f, .00001f);
      mainPanel.DOScale(targetScale, .5f).SetEase(Ease.OutSine);
   }

   void OnDisable() {
      c.state.Set<GameOver>(false);
      c.state.Set<LevelComplete>(false);
   }

   public void UpdateView() {
      var isGameOver = c.state.Is<GameOver>();
      titleTxt.text = isGameOver ? "Game Over" : "Perfect!";
      btnTxt.text = isGameOver ? "Try Again" : "Next Level";
   }
}
