using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public Transform direction;
    public float speed;
    float saveTime = 8f;
    float nowTime;
    public bool restartByDeath;
    Vector3 newPos;
    SphereCollider _collider;
    public BoxCollider _cannonCollider;

    public ParticleSystem exParticle;
    public GameObject mesh;
    GameManager gm;
    PlayerMovement move;
    AudioSource _sound;

    // Start is called before the first frame update
    void Start()
    {
        _collider = this.GetComponent<SphereCollider>();
        Physics.IgnoreCollision(_collider, _cannonCollider, true);
        nowTime = Time.time;
        newPos = direction.rotation * new Vector3(0f, -speed, 0f);
        gm = GameObject.FindWithTag("GM").GetComponent<GameManager>();
        move = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        _sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= saveTime + nowTime)
        {
            Destroy(this.gameObject);
        }
        if (restartByDeath)
        {
            if (gm.reTrap) Destroy(this.gameObject);
        }
        transform.Translate(newPos * Time.deltaTime);

    }
    // private void OnCollisionStay(Collision other) {

    // }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Hazard") || other.CompareTag("Player"))
            Explosion();
    }

    void Explosion()
    {
        mesh.SetActive(false);
        _collider.enabled = false;
        transform.rotation = direction.rotation;
        exParticle.Play();
        StartCoroutine("BulletOver");
        _sound.pitch = Random.Range(1.08f, 0.92f);
        _sound.Play();
    }

    IEnumerator BulletOver()
    {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);

    }
}
