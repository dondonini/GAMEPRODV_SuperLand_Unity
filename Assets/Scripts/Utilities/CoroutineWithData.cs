using System.Collections;
using UnityEngine;

/// <summary>
/// This allows coroutines to return a value
/// </summary>
public class CoroutineWithData<T>
{
    private IEnumerator _target;
    public T result;
    public Coroutine Coroutine { get; private set; }

    public CoroutineWithData(MonoBehaviour owner_, IEnumerator target_)
    {
        _target = target_;
        Coroutine = owner_.StartCoroutine(Start());
    }

    private IEnumerator Start()
    {
        while (_target.MoveNext())
        {
            result = (T)_target.Current;
            yield return result;
        }
    }
}