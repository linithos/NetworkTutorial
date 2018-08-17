using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


 public class Bullet : NetworkBehaviour
 {
     public float m_speed = 30;

     public float m_LifeDuration = 10;
     float LifeTime = 0.0f;

     public float m_Dmg = 1.5f;

     public ParticleSystem m_HitPs;

    [Header("Audio")]
    public AudioSource m_HitSound;

    void Update()
     {
         if (hasAuthority)
         {
             LifeTime += Time.deltaTime;

             if(LifeTime >= m_LifeDuration)
             {
                Destroy(this.gameObject);
             }
             else
             {
                 this.transform.Translate(Vector3.forward * Time.deltaTime * m_speed);
             }
             
         }
     }

    //  private void OnTriggerEnter(Collider other)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<Player>().TakeDamage(m_Dmg);
            
        }

        m_HitPs.transform.parent = null;
        m_HitPs.Play();
        Destroy(m_HitPs.gameObject,1);

        m_HitSound.transform.parent = null;
        m_HitSound.Play();
        Destroy(m_HitSound.gameObject, 2);

        m_speed = 0;

        Destroy(this.gameObject);
    }

    
    private void OnTriggerEnter(Collider other)
     {
         
         if (other.tag == "Player")
         {
             other.GetComponent<Player>().TakeDamage(m_Dmg);
         }

        m_HitPs.transform.parent = null;
        m_HitPs.Play();
        Destroy(m_HitPs.gameObject, 1);

        m_HitSound.transform.parent = null;
        m_HitSound.Play();
        Destroy(m_HitSound.gameObject, 2);

        m_speed = 0;

        Destroy(this.gameObject);
     }
    


 }


