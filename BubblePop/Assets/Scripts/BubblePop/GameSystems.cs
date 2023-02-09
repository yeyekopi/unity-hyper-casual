using System;
using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using UnityEngine;

public sealed class GameSystems : Systems {
    public GameSystems(Contexts c) {
        Add(new LogicSystem(c));
        Add(new AnimationSystem(c));
        Add(new RenderingSystem(c));
    }
}

public class LogicSystem : CompositeSystem {
    public LogicSystem(Contexts c) : base(c) {
        AddInitialize(() => {
            if (!c.state.Has<GridC>()) {
                LogicExtensions.RegenerateGrid(c); 
                var value = LogicExtensions.GetRandomBubbleValue();
                c.state.CreateEntity().Add<ShooterBubble>(value);
            }
        });

        AddReactEach(Matcher.AllOf<ShooterBubble, GridIndex>(), e => {
            var gridIdx = e.Get<GridIndex>().value;
            var value = e.Get<ShooterBubble>().value;
            var grid = c.state.Get<GridC>().value;
            LogicExtensions.HandleShot(c, gridIdx, value, grid);
        });
    }
}

//shooting -> merging + falling -> new rows -> new shooter (or game over)
public class AnimationSystem : CompositeSystem {
    public AnimationSystem(Contexts c) : base(c) {
        AddReactEach(Matcher.AllOf<Shooting>().Added(), e => {
            var m = c.GetManager();
            var gridIdx = e.Get<GridIndex>().value;
            var shotPath = c.state.Get<ShotPath>().value;
            c.state.Remove<ShotPath>();
            
            var tween = m.bubbleGrid.AnimateShot(gridIdx, shotPath);
            tween.OnKill(() => e.Destroy());
        });
        
        AddReact(Matcher.AllOf<Shooting>().Removed(), _ => {
            var anyMergeEvents = TryIncrementAnimatingMergeEvent(c);
            if (!anyMergeEvents) {
                c.PlaySound("contact");
            }
        });

        //merge event = some popping + some falling + 1 upgrading  
        //merge events should be animated in sequence with a delay inbetween defined by masterTween
        AddReactEach(Matcher.AllOf<AnimatingMergeEvent>(), e => {
            var mergeRef = e.Get<AnimatingMergeEvent>().value;
            var mergE = c.state.GetEntityWith<MergeEvent>(mergeRef);
            if (mergE == null) return;
            
            var mergeIdx = mergE.Get<GridIndex>().value;
            var newVal = mergE.Get<BubbleValue>().value; 
            var oldVal = mergE.Get<OldBubbleValue>().value; 

            var m = c.GetManager();
            var mergeBubble = m.bubbleGrid.GetOrCreateBubble(mergeIdx, oldVal);
            Sequence masterTween;
            if (mergE.Is<Explosion2048>()) {
                masterTween = mergeBubble.AnimatePop(oldVal, mergeRef);
            } else {
                masterTween = mergeBubble.AnimateUpgrade(oldVal, newVal, mergeRef);
            }

            var pops = c.state.GetEntitiesWith<Popping>(mergeRef);
            var matchAmt = pops.Count;
            foreach (var popE in pops) {
                var popIdx = popE.Get<GridIndex>().value;
                var popVal = popE.Get<BubbleValue>().value;
                m.bubbleGrid.GetOrCreateBubble(popIdx, popVal).AnimatePop(popVal, mergeRef);
                popE.Destroy();
            }

            var falls = c.state.GetEntitiesWith<Falling>(mergeRef);
            foreach (var fallE in falls) {
                var fallIdx = fallE.Get<GridIndex>().value;
                var fallVal = fallE.Get<BubbleValue>().value;
                var v = m.bubbleGrid.GetOrCreateBubble(fallIdx, fallVal);
                var fromPos = v.transform.localPosition;
                m.bubbleGrid.PushBubble(v);
                m.bubbleGrid.GetOffGridBubble(fallVal).AnimateFall(fromPos, fallVal, mergeRef);
                fallE.Destroy();
            }

            var voulme = .5f + Math.Min(matchAmt * .1f, 1);
            masterTween.InsertCallback(0f, () => c.PlaySound(new SoundInfo("explosion", voulme)));
            masterTween.InsertCallback(.1f, () => c.PlaySound(new SoundInfo("lightning", voulme, pitch: mergeRef + 1)));
                    
            masterTween.OnKill(() => TryIncrementAnimatingMergeEvent(c));
            
            mergE.Destroy();
        });

        AddReact(Matcher.AllOf<AnimatingMergeEvent>().Removed(), _ => {
            if (c.state.TryGet<SpawnRows>(out var rC)) {
                var m = c.GetManager();
                var grid = c.state.Get<GridC>().value;
                m.bubbleGrid.AnimateSpawnRows(grid, rC.value).OnKill(() => {
                    c.state.Remove<SpawnRows>();
                });
            }
        });

        AddReact(new[] {
            Matcher.AllOf<Shooting>().Removed(),
            Matcher.AllOf<AnimatingMergeEvent>().Removed(),
            Matcher.AllOf<SpawnRows>().Removed(),
        }, _ => {
            if (c.IsAnimatingRound()) return;
            if (c.IsConfirmPromptShown()) {
                DOVirtual.DelayedCall(.25f, () => c.ToggleConfirmPrompt(true));
            }
        });

        AddReact(new[] {
            Matcher.AllOf<GameOver>().Removed(),
            Matcher.AllOf<LevelComplete>().Removed(),
        }, _ => {
            c.state.Replace<ViewLevelScore>(0);
        });
        AddReact(Matcher.AllOf<GameOver>().Removed(), _ => {
            var grid = c.state.Get<GridC>().value;
            c.GetManager().bubbleGrid.Prepare(grid);
        });
        AddReact(Matcher.AllOf<LevelComplete>().Removed(), _ => {
            c.GetManager().scorePanelView.UpdateLevel();  
        });

        AddReact(new[] {
            Matcher.AllOf<Shooting>().Removed(),
            Matcher.AllOf<AnimatingMergeEvent>().Removed(),
            Matcher.AllOf<SpawnRows>().Removed(),
            Matcher.AllOf<GameOver>().Removed(),
            Matcher.AllOf<LevelComplete>().Removed(),
        }, _ => {
            if (c.IsNextRoundBlocked()) return;
            var m = c.GetManager();
            var currBubble = c.state.Get<ShooterBubble>().value;
            m.shooterTrigger.AnimateBubbleSpawn(currBubble);
        });
    }

    static bool TryIncrementAnimatingMergeEvent(Contexts c) {
        var masterE = c.state.GetSingleEntity<AnimatingMergeEvent>(); 
        var nMergeRef = masterE?.Get<AnimatingMergeEvent>().value + 1 ?? 0;
        if (c.state.GetEntityWith<MergeEvent>(nMergeRef) == null) {
            masterE.Destroy();
            return false;
        }
        if (masterE == null) {
            c.state.Add<AnimatingMergeEvent>(0);
        } else {
            masterE.Increment<AnimatingMergeEvent>();
        }
        return true;
    }
}

public class RenderingSystem : CompositeSystem {
    public RenderingSystem(Contexts c) : base(c) {
        AddReact(Matcher.AllOf<BubblePopManagerC>(), _ => {
            var m = c.GetManager();
            var grid = c.state.Get<GridC>().value;
            var currBubble = c.state.Get<ShooterBubble>().value;
            m.bubbleGrid.Prepare(grid);
            m.shooterTrigger.UpdateBubble(currBubble);
            m.scorePanelView.UpdateLevel();
        });

        AddReact(new[] {
            Matcher.AllOf<ViewScore>().Added(),
            Matcher.AllOf<ViewLevelScore>().Added(),
        }, e => {
            c.GetManager().scorePanelView.UpdateScore(tween: true);
        });
    }
}
