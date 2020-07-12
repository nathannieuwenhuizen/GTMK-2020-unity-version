using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outliner : MonoBehaviour
{

    public float shrinkSpeed = .01f;
    public float minScale = 0;
    public float maxScale = 1;

    private Vector3 tempScale;
    private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool resetting = false;

    public Transform sprite;
    public Transform mask;
    public float maskWidth = .05f;

    public void Start()
    {
        gameObject.SetActive(false);
    }
    void FixedUpdate()
    {
        if (resetting) return;

        Scale -= shrinkSpeed;
        if (Scale < minScale)
        {
            World.instance.NewItteration();
        }
    }

    public void ResetScale(bool newWorld = false)
    {
        if (resetting) return;
        resetting = true;

        StartCoroutine(ResettingSize());
    }
    private IEnumerator ResettingSize()
    {
        float start = Scale;
        float index = 0;
        float duration = .3f;
        while (index < duration)
        {
            yield return new WaitForFixedUpdate();
            index += Time.deltaTime;
            Scale = Mathf.Lerp(start, maxScale, curve.Evaluate(index / duration));
        }
        resetting = false;
    }

    public float Scale
    {
        get
        {
            return sprite.localScale.x;
        }
        set
        {
            sprite.localScale = new Vector3(value, value, value);
            float maskScale = Mathf.Max(0, value - maskWidth);
            mask.localScale = new Vector3(maskScale, maskScale, maskScale);
        }
    }
}
