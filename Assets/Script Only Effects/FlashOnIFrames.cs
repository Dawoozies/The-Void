using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnIFrames : MonoBehaviour, Listener_IFrames
{
    SpriteRenderer spriteRenderer;
    public Color onColor;
    public Color offColor;
    public float onTime;
    public float timer;

    Queue<float> timerValues = new Queue<float>();
    public void Update_OnIFrames()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (timer < onTime)
            timer += Time.deltaTime;

        if (timer >= onTime)
        {
            if (spriteRenderer.color != onColor)
            {
                spriteRenderer.color = onColor;
            }
            else
            {
                spriteRenderer.color = offColor;
            }

            timer = 0;
        }  
    }
    private void LateUpdate()
    {
        if (spriteRenderer == null)
            return;

        if (timerValues.Count > 1)
            timerValues.Dequeue();

        timerValues.Enqueue(timer);

        //serializedTimerValues = timerValues.ToArray();

        if(timerValues.Count == 2)
        {
            float t1 = timerValues.Dequeue();
            float t2 = timerValues.Dequeue();

            if (t1 == t2)
            {
                if(spriteRenderer.color == onColor)
                    spriteRenderer.color = offColor;
            }

            timerValues.Enqueue(t1);
            timerValues.Enqueue(t2);
        }
    }
}
