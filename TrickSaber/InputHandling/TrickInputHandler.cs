using System;
using System.Collections;
using System.Collections.Generic;

namespace TrickSaber.InputHandling;

internal class TrickInputHandler : IEnumerable<KeyValuePair<TrickAction, HashSet<InputHandler>>>
{
    private readonly Dictionary<TrickAction, HashSet<InputHandler>> _trickHandlerSets = [];

    public void Add(TrickAction action, InputHandler handler)
    {
        if (action == TrickAction.None)
        {
            return;
        }

        if (!_trickHandlerSets.TryGetValue(action, out var set))
        {
            set = [];
            _trickHandlerSets.Add(action, set);
        }

        set.Add(handler);
    }

    public HashSet<InputHandler> GetHandlers(TrickAction action)
    {
        return _trickHandlerSets[action];
    }

    public IEnumerator<KeyValuePair<TrickAction, HashSet<InputHandler>>> GetEnumerator() => 
        _trickHandlerSets.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}