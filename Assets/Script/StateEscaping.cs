using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEscaping : IState
{
    public bool CanEnter(IState currentState)
    {
        return KnightFSMController.instance.IsEnemyInRangeAndEscape();
    }

    public bool CanExit()
    {
        return KnightFSMController.instance.NoEnemyInRange();
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
        KnightFSMController.instance.Escape();
    }
}
