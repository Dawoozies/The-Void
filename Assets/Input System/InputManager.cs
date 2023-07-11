using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using BehaviourRecord.Player;
public class InputManager : MonoBehaviour
{
    public static InputManager ins;
    private void Awake()
    {
        ins = this;
    }

    PlayerControls inputActions;
    public Vector2 L_Input;
    public Vector2 R_Input;
    public float LeftTrigger_Input;
    public float RightTrigger_Input;
    private void OnEnable()
    {
        inputActions = new PlayerControls();
        inputActions.PlayerDefault.LeftStick.performed += inputActions => L_Input = inputActions.ReadValue<Vector2>();
        inputActions.PlayerDefault.RightStick.performed += inputActions => R_Input = inputActions.ReadValue<Vector2>();
        inputActions.PlayerDefault.LeftTrigger.performed += inputActions => LeftTrigger_Input = inputActions.ReadValue<float>();
        inputActions.PlayerDefault.RightTrigger.performed += inputActions => RightTrigger_Input = inputActions.ReadValue<float>();
        inputActions.PlayerDefault.Jump.performed += (inputActions) =>
        {
            JumpDown_Input = inputActions.ReadValueAsButton();
            if(JumpDown_Input)
                JumpDownBuffer.Input(0.2f);
            StateHandlers.Player.Handler.OnJumpPerformed(JumpDown_Input);
            if(GameManager.ins.FindByID("Mantis") != null)
                StateHandlers.Mantis.Handler.OnJumpPerformed(JumpDown_Input);
            Record.OnJumpPerformed(JumpDown_Input);
        };
        inputActions.PlayerDefault.RightBumper.performed += (inputActions) =>
        {
            RightBumper_Input = inputActions.ReadValueAsButton();
            if (RightBumper_Input)
                RightBumperBuffer.Input(0.25f);
        };
        inputActions.PlayerDefault.LeftBumper.performed += (inputActions) =>
        {
            LeftBumper_Input = inputActions.ReadValueAsButton();
            if (LeftBumper_Input)
                LeftBumperBuffer.Input(0.25f);
        };
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    public InputButtonBuffer JumpDownBuffer = new InputButtonBuffer();
    public bool JumpDown_Input;
    public bool JumpDown_BufferedInput;
    const float JUMP_BUFFER_TIME = 0.2f;
    public InputButtonBuffer RightBumperBuffer = new InputButtonBuffer();
    public bool RightBumper_Input;
    public bool RightBumper_BufferedInput;
    public InputButtonBuffer LeftBumperBuffer = new InputButtonBuffer();
    public bool LeftBumper_Input;
    public bool LeftBumper_BufferedInput;
    public void ManagedUpdate(float timeDelta)
    {
        JumpDownBuffer.Update(timeDelta);
        JumpDown_BufferedInput = JumpDownBuffer.isActive;
        RightBumperBuffer.Update(timeDelta);
        RightBumper_BufferedInput = RightBumperBuffer.isActive;
        LeftBumperBuffer.Update(timeDelta);
        LeftBumper_BufferedInput = LeftBumperBuffer.isActive;
    }
}
public class InputButtonBuffer
{
    public bool isActive;
    public float timeLeft;
    public void Input(float bufferTime)
    {
        timeLeft = bufferTime;
        isActive = true;
    }
    public void Update(float timeDelta)
    {
        if (timeLeft > 0)
            timeLeft -= timeDelta;
        if (timeLeft <= 0)
        {
            isActive = false;
            timeLeft = 0;
        }
    }
}