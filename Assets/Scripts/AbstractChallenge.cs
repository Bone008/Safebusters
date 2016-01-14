using UnityEngine;
using System;

[RequireComponent(typeof(Safe))]
public abstract class AbstractChallenge : MonoBehaviour
{
    public GameObject frontGameObject;
    public GameObject backGameObject;

    protected InputState frontInputState = new InputState();
    protected InputState backInputState = new InputState();

    protected bool hasFocusFront = true;
    protected bool hasFocusBack = true;

    protected Safe safe;

    void Start()
    {
        safe = GetComponent<Safe>();
        InitChallenge();
    }

    // overridden by concrete challenge classes to generate their initial state
    protected abstract void InitChallenge();


    public void SetFrontInput(InputState state)
    {
        frontInputState = state;
    }
    public void SetBackInput(InputState state)
    {
        backInputState = state;
    }
    
    public void SetFrontFocus(bool flag)
    {
        hasFocusFront = flag;
    }

    public void SetBackFocus(bool flag)
    {
        hasFocusBack = flag;
    }
}
