using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Plant
{
    public class SunFlower : PlantBase
    {
        SpriteRenderer highlightHead;
        readonly float throwSunBasicCD = 10;
        readonly float throwSunRandomCD = 5;// basicCD+-RandomCD�ڵ����ֵ


        private new void Start()
        {
            base.Start();
            StartCoroutine(ThrowSun());
            highlightHead = transform.GetChild(1).GetComponent<SpriteRenderer>(); // ��Ҫ��֤ index ����
        }



        IEnumerator ThrowSun()
        {
            float duration = 1f;
            yield return new WaitForSeconds(Random.Range(0f, throwSunRandomCD - duration));
            while (true)
            {
                StartCoroutine(HighlightFlowerHead(duration));
                yield return new WaitForSeconds(
                    Random.Range(throwSunBasicCD - throwSunRandomCD, throwSunBasicCD + throwSunRandomCD));
            }
        }

        IEnumerator HighlightFlowerHead(float duration)
        {
            float currentTime = 0f;
            Color newColor = highlightHead.color;
            while (currentTime < duration)
            {
                float newAlpha = Mathf.Lerp(0f, 1f, currentTime / duration);
                newColor.a = newAlpha;
                highlightHead.color = newColor;

                currentTime += Time.deltaTime;
                yield return null;
            }

            // ��ɺ�ظ�����͸����
            // ���ǹ������ظ�����ͷ�����Ǹо����岻��
            Color finalColor = highlightHead.color;
            finalColor.a = 0;
            highlightHead.color = finalColor;
            
            var newSun = Instantiate(bullet, transform.position, Quaternion.identity);// sunflower��bullet����Sun
            
            StartCoroutine(newSun.GetComponent<Sun>().FlowerSunThrow());
        }
    }

}
