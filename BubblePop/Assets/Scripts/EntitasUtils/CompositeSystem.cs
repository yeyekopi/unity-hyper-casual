using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using InlineSystems;

#nullable enable
public readonly struct CollectorConfig {
    public readonly TriggerOnEvent? trigger;
    public readonly TriggerOnEvent[]? triggers;
    public readonly IMatcher? matcher;
    public readonly ICollector? collector;

    public IMatcher? Matcher => matcher ?? trigger?.matcher;
    
    private CollectorConfig(ICollector? collector = null, TriggerOnEvent? trigger = null, TriggerOnEvent[]? triggers = null, IMatcher? matcher = null) {
        this.collector = collector;
        this.trigger = trigger;
        this.triggers = triggers;
        this.matcher = matcher;
    }
    
    public static CollectorConfig Create(TriggerOnEvent trigger) => new CollectorConfig(
        trigger: trigger
    );
    public static CollectorConfig Create(TriggerOnEvent[] triggers) => new CollectorConfig(
        triggers: triggers
    );
    public static CollectorConfig Create(IMatcher matcher) => new CollectorConfig(
        matcher: matcher
    );
    
    public static CollectorConfig Create(ICollector collector) => new CollectorConfig(
        collector: collector
    );
}
#nullable restore


public class CompositeSystem : Systems {
    public readonly Contexts c;
    protected int systemCount;
    
    public CompositeSystem(Contexts c) {
        this.c = c;
    }

    public override ISystems Add(ISystem system) {
        systemCount++;
        return base.Add(system);
    }
    
    public void AddInitialize(Action initialize) {
        Add(new InlineInitializeSystem(initialize, this, systemCount));
    }
    
    public SmartReactiveSystem AddReactEach(IMatcher matcher, Action<IEntity> execute, bool enabled = true) {
        return AddReactEach(CollectorConfig.Create(matcher), matcher.Matches, execute, enabled);
    }
    
    public SmartReactiveSystem AddReactEach(TriggerOnEvent trigger, Action<IEntity> execute, bool enabled = true) {
        return AddReactEach(CollectorConfig.Create(trigger), GetPredicate(trigger), execute, enabled);
    }
    
    public SmartReactiveSystem AddReactEach(TriggerOnEvent[] triggers, Action<IEntity> execute, bool enabled = true) {
        return AddReactEach(CollectorConfig.Create(triggers), e => true, execute, enabled);
    }
    
    public SmartReactiveSystem AddReactEach(CollectorConfig config, Func<IEntity, bool> filter, Action<IEntity> execute, bool enabled = true) {
        var sys = new InlineSmartReactiveEachSystem(c, config, filter, execute, this, systemCount, enabled);
        Add(sys);
        return sys;
    }
    
    public SmartReactiveSystem AddReact(IMatcher matcher, Action<List<IEntity>> execute, bool enabled = true) {
        return AddReact(CollectorConfig.Create(matcher), matcher.Matches, execute, enabled: enabled);
    }
    
    public SmartReactiveSystem AddReact(TriggerOnEvent trigger, Action<List<IEntity>> execute, bool enabled = true) {
        return AddReact(CollectorConfig.Create(trigger), GetPredicate(trigger), execute, enabled: enabled);
    }
    
    public SmartReactiveSystem AddReact(TriggerOnEvent[] triggers, Action<List<IEntity>> execute, bool enabled = true) {
        return AddReact(CollectorConfig.Create(triggers), e => true, execute, enabled: enabled);
    }

    public SmartReactiveSystem AddReact(CollectorConfig config, Func<IEntity, bool> entityPredicate, Action<List<IEntity>> execute, bool enabled = true) {
        var sys = new InlineSmartReactiveSystem(c, config, entityPredicate, execute, this, systemCount, enabled);
        Add(sys);
        return sys;
    }

    protected Func<IEntity, bool> GetPredicate(TriggerOnEvent trigger) {
        switch (trigger.groupEvent) {
            case GroupEvent.Added:
                return e => trigger.matcher.Matches(e);
            case GroupEvent.Removed:
                return e => !trigger.matcher.Matches(e);
            case GroupEvent.AddedOrRemoved:
                return e => true;
            default:
                throw new ArgumentOutOfRangeException(nameof(trigger.groupEvent), trigger.groupEvent, null);
        }
    }
}

namespace InlineSystems {
    public class InlineInitializeSystem : IInitializeSystem {
        Action initialize;
        string label;
        readonly object parent;
        readonly int systemNumber;

        public InlineInitializeSystem(Action initialize, object parent, int systemNumber) {
            this.initialize = initialize;
            this.parent = parent;
            this.systemNumber = systemNumber;
        }

        public void Initialize() {
            initialize();
        }

        public override string ToString() {
            return label ??= $"{parent.GetType().FullName} #{systemNumber} (AddInitialize)";
        }

        string ISystem.label => parent.GetType().FullName;
    }
    
    public sealed class InlineSmartReactiveEachSystem : SmartReactiveSystem {
        Func<IEntity, bool> filter;
        Action<IEntity> execute;
        string _label;
        readonly Contexts c;
        readonly object parent;
        readonly int systemNumber;
        readonly CollectorConfig collectorCfg;

        public InlineSmartReactiveEachSystem(Contexts c, CollectorConfig collectorCfg, Func<IEntity, bool> filter, Action<IEntity> execute, object parent, int systemNumber, bool enabled) : base(c.state, enabled) {
            this.c = c;
            this.filter = filter;
            this.execute = execute;
            this.parent = parent;
            this.systemNumber = systemNumber;
            this.collectorCfg = collectorCfg;
            if (enabled) { EnsureCollector(); }
        }

        protected override ICollector GetTrigger(IContext context) {
            return context.CreateCollector(collectorCfg);
        }

        protected override bool Filter(IEntity entity) {
            return filter(entity);
        }

        protected override void Execute(List<IEntity> entities) {
            foreach (var e in entities) {
                try { execute(e); } catch (Exception ex) { Debug.LogException(ex); }
            }
        }

        public void Prewarm() {
            Collector.Prewarm();
        }

        public override string ToString() {
            return _label ?? (_label = $"{parent.GetType().FullName} #{systemNumber} {collectorCfg.Matcher} AddReactEach");
        }

        public override string label => parent.GetType().FullName;
    }


    public sealed class InlineSmartReactiveSystem : SmartReactiveSystem {
        Func<IEntity, bool> filter;
        Action<List<IEntity>> execute;
        string _label;
        readonly object parent;
        readonly int systemNumber;
        readonly CollectorConfig collectorCfg;
        readonly Contexts c;

        public InlineSmartReactiveSystem(Contexts c, CollectorConfig collectorCfg, Func<IEntity, bool> filter, Action<List<IEntity>> execute, object parent, int systemNumber, bool enabled) : base(c.state, enabled) {
            this.filter = filter;
            this.execute = execute;
            this.parent = parent;
            this.systemNumber = systemNumber;
            this.collectorCfg = collectorCfg;
            this.c = c;
            if (enabled) { EnsureCollector(); }
        }

        protected override ICollector GetTrigger(IContext context) {
            return context.CreateCollector(collectorCfg);
        }

        protected override bool Filter(IEntity entity) {
            return filter(entity);
        }

        protected override void Execute(List<IEntity> entities) {
            execute(entities);
        }

        public override string ToString() {
            return _label ?? (_label = $"{parent.GetType().FullName} #{systemNumber} {collectorCfg.Matcher} AddReact");
        }

        public void Prewarm() {
            Collector.Prewarm();
        }

        public override string label => parent.GetType().FullName;
    }
}