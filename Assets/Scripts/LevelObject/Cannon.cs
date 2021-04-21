using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cannon : MonoBehaviour
{
    public GameObject bullet;
    public float _speed = 3f;
    public float cooldown = 2f;
    public float delay = 0;
    public int trapNumber = 0;
    float cooldownOrigin;
    Transform _transform;
    Vector3 newPos, oriPos;
    Tween scaleTween, posTween;
    BoxCollider _collider;
    GameManager gm;
    public bool restartByDeath = true;
    AudioSource _sound;

    void Start()
    {
        cooldownOrigin = cooldown;
        _transform = GetComponent<Transform>();
        _collider = GetComponent<BoxCollider>();
        newPos = _transform.rotation * new Vector3(0f, 0.15f, 0f);
        oriPos = _transform.position;
        StartCoroutine("StartShoot");
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
        _sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (restartByDeath)
        {
            if (gm.reTrap)
            {
                StartCoroutine("StartShoot");
            }
        }
        if (Time.time >= cooldown)
        {
            Shoot();
        }
    }
    IEnumerator StartShoot()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        cooldown = Time.time + delay;
    }
    void Shoot()
    {
        scaleTween.Kill();
        posTween.Kill();
        transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
        transform.position = transform.position + newPos;
        scaleTween = transform.DOScale(0.15f, 0.85f).SetEase(Ease.OutElastic);
        posTween = transform.DOMove(oriPos, 0.85f).SetEase(Ease.OutElastic);
        GameObject _bullet = Instantiate(bullet, transform.position, new Quaternion(0, 0, 0, 0)) as GameObject;
        _bullet.GetComponent<BulletMove>().direction = _transform;
        _bullet.GetComponent<BulletMove>().speed = _speed;
        _bullet.GetComponent<BulletMove>()._cannonCollider = _collider;
        _bullet.GetComponent<BulletMove>().restartByDeath = restartByDeath;
        _bullet.GetComponent<TrapNumberReturn>().trapNumber = trapNumber;
        cooldown = Time.time + cooldownOrigin;
        _sound.pitch = Random.Range(1.08f, 0.92f);
        _sound.Play();
    }
}
