using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CheckPoint : MonoBehaviour
{
    public UnityEvent PassEvent;
    private Animator anim;
    PlayerMovement move;
    GameObject gm;
    SoundEffectManager _sound;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        gm = GameObject.FindWithTag("GM");
        _sound = GameObject.Find("Level_SE_Manager").GetComponent<SoundEffectManager>();
    }

    // Update is called once per frame


    IEnumerator RebirthAnimIenumerator()
    {
        yield return new WaitForSeconds(1.15f);

            anim.Play("CheckPoint_Revival",0,0);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            move.checkPoint = this.gameObject;
            move.checkPointV3 = this.transform.position;
            anim.SetBool("enter", true);
            gm.SendMessage("CheckCoin");
            this.GetComponent<BoxCollider>().enabled = false;
            GameObject.FindWithTag("UI_cpTip").GetComponent<ProgressTip>().SendMessage("PassCP");
            PassEvent.Invoke();
            _sound.PlayOneSound(2,0.7f);
        }
    }
}
