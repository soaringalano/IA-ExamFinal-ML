using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSearching : IState
{
    public bool CanEnter(IState currentState)
    {
        return true;
    }

    public bool CanExit()
    {
        return true;
    }

    public void Enter()
    {

    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {
        KnightFSMController.instance.Search();
    }
}
