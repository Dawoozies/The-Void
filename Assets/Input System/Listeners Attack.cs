using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Listener_LightAttackInput
{
    void Update_LightAttackInput(bool lightAttackInput);
}

public interface Listener_AnyAttackInput
{
    void Update_AnyAttackInput(string attackType, bool anyAttackInput);
}