using Entitas;
using UnityEngine;


public class BubblePopManagerC : UniqueComponent<BubblePopManager> { }
public class BubblePopManager : ViewManager {

    public ScorePanelView scorePanelView;
    public BubbleGrid bubbleGrid;
    public ShooterTrigger shooterTrigger;
    public SoundManager soundManager;
    public ConfirmPrompt confirmPrompt;
    
    void Start() {
        c.state.Add<BubblePopManagerC>(this);
    }
}
