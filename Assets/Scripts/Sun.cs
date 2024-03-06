using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private static readonly float dispearTime = 5f;

    int sunNum = 25;



    //static Vector3 SunPosition => Camera.main.ScreenToWorldPoint(sunPos.position);

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
        float t = 0;
        while (scale.x < Vector3.one.x)
        {
            t += Time.deltaTime * 2;
            pos.x = Mathf.Lerp(-0.7f, 1f, t);
            pos.y = -pos.x * pos.x;
            ////pos = Vector3.Lerp(Vector3.zero, new(1f, -pos.x * pos.x), Time.time * 0.1f);
            scale = Vector3.Lerp(new(0.3f, 0.3f, 0f), Vector3.one, t);
            transform.localPosition = pos + originalPos;
            transform.localScale = scale;
            yield return null;
        }
        StartCoroutine(DisaperAfter(time: dispearTime));
    }

    public IEnumerator SkySunThrow(Vector3 dest)
    {
        Vector3 pos;
        Vector3 originalPos = transform.localPosition;
        float t = 0;
        while (Vector3.Distance(transform.localPosition, dest) > 0.1f)
        {
            t += Time.deltaTime * 0.1f;
            pos = Vector3.Lerp(originalPos, dest, t);
            transform.localPosition = pos;
            yield return null;
        }
        StartCoroutine(DisaperAfter(time: dispearTime));
    }

    public IEnumerator BeCollected()
    {
        StopAllCoroutines();
        Vector3 originalPos = transform.localPosition;
        Vector3 destPositon = GameManager.Instance.SunIconPosition;
        float t = 0;
        while (Vector3.Distance(transform.localPosition, destPositon) > 0.1f)
        {
            t += Time.deltaTime * 2;
            Vector3 pos = Vector3.Lerp(originalPos, destPositon, t);
            Vector3 scale = Vector3.Lerp(new(1f, 1f, 0f), new(0.3f, 0.3f, 0), t);
            transform.localPosition = pos;
            transform.localScale = scale;
            yield return null;
        }
        Destroy(gameObject);
        GameManager.Instance.SunNum += sunNum;
    }
}