using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;



public class Player : NetworkBehaviour 
{
    [Header("objects")]
    public GameObject bullet;
    public GameObject m_PlayerCam;
    public GameObject m_PlayerBody;
    public Transform m_FirePos;
    

    [Header("player Info")]
    public float m_MaxLife = 20;
    public float m_Speed = 6;

    [SyncVar(hook = "OnChangeHealth")]
    public float m_CurLife;

    [Header("UI")]
    public Image m_HealthBar;


    [Header("Particles")]
    public ParticleSystem m_DeathPs;

    [Header("Audio")]
    public AudioSource m_FireSound;

    float m_RespawnTime = 0.0f;


    //spawn pos
    private NetworkStartPosition[] spawnPoints;


    bool m_bAlive = true;

    void Start()
    {
        m_CurLife = m_MaxLife;

        if (isLocalPlayer)
        {
            m_PlayerCam.transform.parent = null;
            m_PlayerCam.SetActive(true);

            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
        else
        {
            m_PlayerCam.SetActive(false);
        }
    }

    void Update()
    {
        if (isLocalPlayer && m_bAlive)
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 60f, Space.World);             
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(-Vector3.up * Time.deltaTime * 60f, Space.World);
            }

            if (Input.GetKey(KeyCode.W))
            {             
                this.transform.Translate(Vector3.forward * Time.deltaTime * m_Speed);
            }

            if (Input.GetKey(KeyCode.S))
            {
                this.transform.Translate(-Vector3.forward * Time.deltaTime * m_Speed);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                CmdSFireBullet();
            }

           // m_HealthBar.fillAmount = m_CurLife / m_MaxLife;
        }

        if (!m_bAlive)
        {
            m_RespawnTime += Time.deltaTime;

            if(m_RespawnTime >= 2)
            {
                Respawn();
            }
        }       
    }

    public void TakeDamage(float damege)
    {
        if (!m_bAlive)
        {
            return;
        }
        m_CurLife -= damege;

        if (m_CurLife <= 0)
        {
            m_CurLife = 0.0f;

            m_bAlive = false;

            m_DeathPs.Play();

            m_RespawnTime = 0.0f;

            m_PlayerBody.SetActive(false);
        }

    }

    void Respawn()
    {
        // Set the spawn point to origin as a default value
        Vector3 spawnPoint = Vector3.zero;

        // If there is a spawn point array and the array is not empty, pick a spawn point at random
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        }

        // Set the player’s position to the chosen spawn point
        transform.position = spawnPoint;

        m_PlayerBody.SetActive(true);

        m_bAlive = true;
    }

    void OnChangeHealth(float h)
    {
        m_HealthBar.fillAmount = h / m_MaxLife;
    }

    [Command]
    public void CmdSFireBullet()
    {
        m_FireSound.Play();
        GameObject b = (GameObject)Instantiate(bullet, m_FirePos.position, m_FirePos.rotation);
        NetworkServer.SpawnWithClientAuthority(b, connectionToClient);
    }



}
   

