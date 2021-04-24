using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PilotTesting_Question : MonoBehaviour
{
    [Header("UI")]
    public Image continueTip;
    public RectTransform bg;
    public Text _text;

    [Header("Level")]
    public int nextLevelId = 0;
    public bool ExitGameAfterLevel = false;
    bool canPass = false;
    Sequence startSequence;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SkipIEnumerator");
        startSequence = DOTween.Sequence();
        startSequence.AppendInterval(0.5f);
        startSequence.Append(bg.DOScaleY(2f, 0.4f));
        startSequence.Join(_text.DOFade(1, 0.8f));
    }

    // Update is called once per frame
    void Update()
    {
        if (canPass && Input.GetButtonDown("Enter"))
        {
            if (ExitGameAfterLevel) Application.Quit();
            else StartCoroutine("NextLevelIEnumerator");
        }
    }

    IEnumerator SkipIEnumerator()
    {
        yield return new WaitForSeconds(1.7f);
        continueTip.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.35f);
        canPass = true;
    }
    IEnumerator NextLevelIEnumerator()
    {
        startSequence.Append(bg.DOScaleY(0f, 0.4f));
        startSequence.Join(continueTip.DOFade(0f,0.6f));
        startSequence.AppendInterval(0.3f);
        startSequence.Append(_text.DOFade(0, 0.5f));
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadScene(nextLevelId);
    }
}
