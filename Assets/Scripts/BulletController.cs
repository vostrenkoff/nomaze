using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public bool allowMovement = false;
    public float speed = 20f;
    public float TimeBetweenShots;
    private float shotCounter;
    public ParticleSystem particle;
    public void BulletMovement(float speedoz)
    {
        /*if(speedoz < 1)
        {
            speedoz = speedoz + 10f;
        }
        speed += speedoz;*/
        allowMovement = true;
    }
    private void Update()
    {
        if (allowMovement)
        {

            transform.position += transform.forward * Time.deltaTime * speed;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Invoke("DestroyBullet", 1.5f);
        allowMovement = false;
        particle.Play();
        
    }
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
