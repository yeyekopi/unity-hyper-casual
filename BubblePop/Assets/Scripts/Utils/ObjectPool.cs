using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class ObjectPool<T>
{

    readonly Func<T> _factoryMethod;
    readonly Action<T> _pushMethod;
    readonly Action<T> _popMethod;
    readonly Stack<T> _objectPool;
    int creationCount;
    
    public int Count => _objectPool.Count;
    public int CreationCount => creationCount; 
    public ObjectPool(Func<T> factoryMethod, Action<T> pushMethod = null, Action<T> popMethod = null)
    {
        _factoryMethod = factoryMethod;
        _pushMethod = pushMethod;
        _popMethod = popMethod;
        _objectPool = new Stack<T>();
    }

    public T Get()
    {
        T get;
        if (_objectPool.Count == 0) {
            creationCount++;
            get = _factoryMethod();
        }
        else
        {
            get = _objectPool.Pop();
        }
        if (_popMethod != null)
        {
            _popMethod(get);
        }
        return get;
    }

    public void Push(T obj)
    {
        if (_pushMethod != null)
        {
            _pushMethod(obj);
        }
        _objectPool.Push(obj);
    }

    public void Populate(int count) {
        for (int i = 0; i < count; i++) {
            Push(_factoryMethod());
        }
        creationCount += count;
    }


    public IEnumerable<T> PopAllSilently() {
        while (Count > 0) {
            yield return _objectPool.Pop();
        }
    }

    public IEnumerator<T> GetEnumerator() {
        return _objectPool.GetEnumerator();
    }
}

