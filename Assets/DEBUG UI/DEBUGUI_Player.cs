using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DEBUGUI_Player : MonoBehaviour
{
    public GameObject playerObject;
    Rigidbody2D rb;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI dragText;
    public RectTransform leftStickRect;
    public RectTransform rightStickRect;
    public Slider leftTriggerSlider;
    public Slider rightTriggerSlider;
    public float stickMoveMultiplier;
    void Update()
    {
        if (playerObject == null)
            return;

        if (rb == null)
            rb = playerObject.GetComponent<Rigidbody2D>();

        velocityText.text = $"VELOCITY = (x={rb.velocity.x},y={rb.velocity.y})";
        dragText.text = $"LINEAR DRAG = {rb.drag}";
        leftStickRect.anchoredPosition = InputManager.ins.L_Input * stickMoveMultiplier;
        rightStickRect.anchoredPosition = InputManager.ins.R_Input * stickMoveMultiplier;
        leftTriggerSlider.value = InputManager.ins.LeftTrigger_Input;
        rightTriggerSlider.value = InputManager.ins.RightTrigger_Input;
    }
}
