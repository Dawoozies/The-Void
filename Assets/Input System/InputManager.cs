using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
public class InputManager : MonoBehaviour
{
    public static InputManager ins;
    private void Awake()
    {
        ins = this;
    }

    PlayerControls inputActions;
    public Vector2 R_Input;

    //Listeners
    List<Listener_JumpInput> listeners_JumpInput;
    List<Listener_DodgeInput> listeners_DodgeInput;
    List<Listener_LightAttackInput> listeners_LightAttackInput;
    private void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerDefault.Movement.performed += inputActions => R_Input = inputActions.ReadValue<Vector2>();

            inputActions.PlayerDefault.Jump.performed += inputActions =>
            {
                if (listeners_JumpInput == null)
                    return;

                foreach (Listener_JumpInput listener in listeners_JumpInput)
                {
                    listener.Update_JumpInput(inputActions.ReadValueAsButton());
                }
            };

            inputActions.PlayerDefault.Dodge.performed += inputActions =>
            {
                if (listeners_DodgeInput == null)
                    return;

                foreach (Listener_DodgeInput listener in listeners_DodgeInput)
                {
                    listener.Update_DodgeInput(inputActions.ReadValueAsButton());
                }
            };

            inputActions.PlayerDefault.LightAttack.performed += inputActions =>
            {
                if (listeners_LightAttackInput == null)
                    return;

                foreach(Listener_LightAttackInput listener in listeners_LightAttackInput)
                {
                    listener.Update_LightAttackInput(inputActions.ReadValueAsButton());
                }
            };
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    //Subscribe Methods
    public void Subscribe(Listener_JumpInput listener)
    {
        if (listeners_JumpInput == null)
            listeners_JumpInput = new List<Listener_JumpInput>();

        listeners_JumpInput.Add(listener);
    }

    public void Subscribe(Listener_DodgeInput listener)
    {
        if (listeners_DodgeInput == null)
            listeners_DodgeInput = new List<Listener_DodgeInput>();

        listeners_DodgeInput.Add(listener);
    }

    public void Subscribe(Listener_LightAttackInput listener)
    {
        if (listeners_LightAttackInput == null)
            listeners_LightAttackInput = new List<Listener_LightAttackInput>();

        listeners_LightAttackInput.Add(listener);
    }
}
