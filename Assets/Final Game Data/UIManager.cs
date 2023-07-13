using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using RuntimeContainers;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager ins;
    public TextMeshProUGUI stockLeft;
    public TextMeshProUGUI percentage;
    public TextMeshProUGUI jumpsLeft;
    public Color defaultColor;
    public Color damageColor;
    public Color healColor;
    const float PERCENT_SMOOTH_TIME = 0.2f;
    Vector3 v;
    float t = 1;
    float s;
    Color currentColor;

    public Slider bossStaggerSlider;
    public void Awake()
    {
        ins = this;
    }
    public void Start()
    {
        RuntimePlayerDamage.onPlayerPercentageChanged += OnPlayerPercentageChanged;
        StateHandlers.Player.Handler.onJumpsLeftChanged += OnJumpsLeftChanged;

    }
    public void Update()
    {
        percentage.rectTransform.localScale = Vector3.SmoothDamp(percentage.rectTransform.localScale, Vector3.one, ref v, PERCENT_SMOOTH_TIME);
        percentage.color = Color.Lerp(defaultColor, currentColor, t);
        t = Mathf.SmoothDamp(t, 0f, ref s, PERCENT_SMOOTH_TIME);
    }
    public void OnPlayerPercentageChanged(float previousPercentage, float newPercentage)
    {
        percentage.text = $"{newPercentage}%";
        float percentDelta = newPercentage - previousPercentage;
        //Debug.LogError($"{percentDelta/100f}");
        float sizeChange = Mathf.Abs((newPercentage+percentDelta) / 100f);
        percentage.rectTransform.localScale = percentage.rectTransform.localScale + new Vector3(sizeChange, sizeChange, sizeChange);
        t = 1;
        if (percentDelta > 0)
            currentColor = damageColor;
        else 
            currentColor = healColor;
    }
    void OnJumpsLeftChanged(int jumpsLeft)
    {
        this.jumpsLeft.text = $"Jumps Left = {jumpsLeft}";
    }
    public void OnStaggerValueChanged(int newStaggerValue)
    {
        bossStaggerSlider.value = newStaggerValue;
    }
}
