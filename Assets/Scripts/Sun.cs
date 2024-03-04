using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private float dispearTime = 5f;
    public void Start()
    {
        //Time.timeScale = 0.1f;
        StartCoroutine(DisaperAfter(time: dispearTime));
        //StartCoroutine(FlowerSunThrow());
    }

    IEnumerator DisaperAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isDisolving", true);
    }

    void AnimationEventDisolve()
    {
        Destroy(gameObject);
    }

    public IEnumerator FlowerSunThrow()
    {
        transform.localScale = new(0.3f, 0.3f, 0);
        Vector3 pos = new(0.1f, -0.01f, 0);
        Vector3 originalPos = transform.localPosition;
        Vector3 scale = transform.localScale;
        while (scale.x < Vector3.one.x)
        {
            float t = Mathf.Clamp01(Time.time * 2f);

            pos.x = Mathf.Lerp(-0.7f, 1f, t);
            pos.y = -pos.x * pos.x;
            ////pos = Vector3.Lerp(Vector3.zero, new(1f, -pos.x * pos.x), Time.time * 0.1f);
            scale = Vector3.Lerp(new(0.3f, 0.3f, 0f), Vector3.one, t);
            transform.localPosition = pos + originalPos;
            transform.localScale = scale;
            yield return null;
        }
    }

    public IEnumerator SkySunThrow(Vector3 dest)
    {
        Vector3 pos;
        Vector3 originalPos = transform.localPosition;
        while (Vector3.Distance(transform.localPosition, dest) > 0.1f)
        {
            float t = Mathf.Clamp01(Time.time * 2f);
            pos = Vector3.Lerp(originalPos, dest, t);
            transform.localPosition = pos;
            yield return null;
        }
    }
}
