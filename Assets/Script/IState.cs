using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{

    public void Enter();
    public bool CanEnter(IState currentState);
    public void Exit();

    public bool CanExit();

    public void Update();

    public void FixedUpdate();


}
