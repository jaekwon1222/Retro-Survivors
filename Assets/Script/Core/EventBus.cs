using System;
using System.Collections.Generic;

// Simple event bus
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subs = new();

    public static void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var t = typeof(T);
        if (!_subs.ContainsKey(t)) _subs[t] = new List<Delegate>();
        _subs[t].Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var t = typeof(T);
        if (_subs.TryGetValue(t, out var list)) list.Remove(handler);
    }

    public static void Publish<T>(T e) where T : GameEvent
    {
        var t = typeof(T);
        if (!_subs.TryGetValue(t, out var list)) return;
        // iterate copy to avoid modification during invoke
        var snapshot = list.ToArray();
        foreach (var d in snapshot) (d as Action<T>)?.Invoke(e);
    }
}