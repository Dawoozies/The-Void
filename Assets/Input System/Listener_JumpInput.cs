using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public interface Listener_JumpInput
{
    void Update_JumpInput(bool jumpInput);
}

public interface Listener_JumpReleaseInput
{
    void Update_JumpReleaseInput();
}