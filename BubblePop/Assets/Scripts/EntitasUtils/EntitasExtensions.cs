#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Entitas;

public static class EntitasExtensions {

    public static ICollector CreateCollector(this IContext context, CollectorConfig cfg) {
        if (cfg.collector != null) {
            return cfg.collector;
        } else if (cfg.matcher != null) {
            return context.CreateCollector(cfg.matcher);
        } else if (cfg.trigger != null) {
            return context.CreateCollector(cfg.trigger.Value);
        } else if (cfg.triggers != null) {
            return context.CreateCollector(cfg.triggers);
        } else {
            throw new Exception($"CollectorConfig is invalid, can't CreateCollector");
        }
    }
    
    /// <param name="comp">A ref to the Component, not the value of it. You shouldn't modify the comp by calling c.state.Remove() c.state.Replace() before calling comp.value</param>
    public static bool TryGet<T>(this IContext c, [NotNullWhen(true)] out T? comp) where T : class, IUniqueValueComponent {
        var e = c.GetSingleEntity<T>();
        if (e == null) {
            comp = default;
            return false;
        } else {
            comp = e.Get<T>();
            return true;
        }
    }
    
    public static TValue? GetValueOrDefault<TComp, TValue>(this IContext context, TValue? defaultValue = default(TValue)) where TComp : UniqueComponent<TValue>, new() {
        return context.TryGet<TComp>(out var comp) ? comp.value : defaultValue;
    }
    
    public static TValue? GetValueOrDefault<TComp, TValue>(this IEntity? e, TValue? defaultValue = default(TValue)) where TComp : Component<TValue>, new() {
        var comp = e?.GetOrDefault<TComp>();
        return comp != null ? comp.value : defaultValue;
    }

    public static TComp? GetOrDefault<TComp>(this IEntity? e) where TComp : class, IValueComponent, new() {
        return (TComp?)e?.GetComponentUnsafe(ComponentLookup.GetIndex<TComp>());
    }
    
    public static long Increment<T>(this IContext context, long amount = 1) where T : UniqueComponent<long>, new() {
        var e = context.GetSingleEntity<T>() ?? context.CreateEntity();
        return e.Increment<T>(amount);
    }
    
    public static int Increment<T>(this IContext context, int amount = 1) where T : UniqueComponent<int>, new() {
        var e = context.GetSingleEntity<T>() ?? context.CreateEntity();
        return e.Increment<T>(amount);
    }
    
    public static long Increment<T>(this IEntity e, long amount = 1) where T : Component<long>, new() {
        var currentValue = e.TryGet<T>(out var comp) ? comp.value : default;
        var newValue = checked(currentValue + amount);
        e.Replace<T>(newValue);
        return newValue;
    }
    
    public static int Increment<T>(this IEntity e, int amount = 1) where T : Component<int>, new() {
        var currentValue = e.TryGet<T>(out var comp) ? comp.value : default;
        var newValue = checked(currentValue + amount);
        e.Replace<T>(newValue);
        return newValue;
    }
    
    public static bool TryGet<T>(this IEntity? e, [NotNullWhen(true)] out T? comp) where T : class, IValueComponent, new() {
        comp = null;
        if (e != null) {
            var idx = ComponentLookup.GetIndex<T>();
            comp = e.GetComponentUnsafe(idx) as T;
        }
        return comp != null;
    }
    
}