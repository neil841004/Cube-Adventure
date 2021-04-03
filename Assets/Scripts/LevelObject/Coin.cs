using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    public bool neverNoPick = false;
    public ObjectPool pool;
    public GameObject[] coinEffects = new GameObject[2];
    public Transform coinMesh;
    Material[] coinMaterial = new Material[4];
    public Color green, gray;
    public SphereCollider getTrigger, failTrigger;
    Color coinMeshColor;
    PlayerMesh playerMesh;
    Tween colorTween;



    void Start()
    {
        playerMesh = GameObject.FindWithTag("PlayerMesh").GetComponent<PlayerMesh>();
        for (int i = 0; i < 4; i++)
        {
            coinMaterial[i] = coinMesh.GetChild(i).GetComponent<MeshRenderer>().material;
        }
        if (neverNoPick) failTrigger.gameObject.SetActive(false);
    }

    public void PickCoin()
    {
        if (playerMesh) playerMesh.SendMessage("GetCoin");
        pool.ReUse(transform.position, Quaternion.Euler(-90, 0, 0));
        this.gameObject.SetActive(false);
    }

    public void NotPickCoin()
    {
        getTrigger.enabled = false;

        coinEffects[0].SetActive(false);
        coinEffects[1].GetComponent<DOTweenAnimation>().DOPause();
        coinEffects[1].GetComponent<SpriteRenderer>().DOFade(0, 0.4f);

        for (int i = 0; i < 4; i++)
        {
            coinMaterial[i].SetColor("_TintColor", gray);
        }
    }

    public void ResetCoinState()
    {
        if (getTrigger.enabled == false)
        {
            getTrigger.enabled = true;
            coinEffects[0].SetActive(true);
            coinEffects[1].GetComponent<DOTweenAnimation>().DOPlay();
            // coinEffects[1].GetComponent<SpriteRenderer>().DOFade(0, 0.4f);
            for (int i = 0; i < 4; i++)
            {
                coinMaterial[i].SetColor("_TintColor", green);
            }
        }
    }

}
