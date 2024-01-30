using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
        text.color = Color.yellow;
        StartCoroutine(FadeTask());
    }

    IEnumerator FadeTask() {
        while (text.color.a > 0.1f) {
            text.color -= new Color(0, 0, 0, 0.05f);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
    }
}
