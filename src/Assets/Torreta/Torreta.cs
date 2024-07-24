using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torreta : MonoBehaviour
{
    public float Speed = 2f;
    public float mouseSensivity = 500f;
    public float FireRate = 0.05f;
    public float barrelRotationSpeed;
    public float max_rotacio = 60f;
    public float mal_bala = 5f;
    public float forca_bala = 1f;
    public float max_v_barrel = 15f;

    // Gameobjects need to control rotation and aiming
    public Transform arm;
    public Transform Barrel;
    public ParticleSystem muzzelFlash;
    public Transform camera;
    public GameObject impacte_bala;
    public GameObject explosio;

    AudioSource[] audios;

    int dispars_encertats = 0;
    int dispars_fallats = 0;

    float mouseY;
    float currentPitch = 0;

    float mouseX;
    float currentYaw = 0;

    float pitchMin = -10;
    float pitchMax = 50;

    float yawMin = -30;
    float yawMax = 30;

    // Gun barrel rotation
    float barrel_rotation = 0f;
    bool just_aturat = false;

    float yRotation = 0f;
    float nextFire = 0f;

    private void Start()
    {
        audios = GetComponents<AudioSource>();
    }

    void Update()
    {
        if (Controls_generals.game_paused)
        {
            particulesDispar();
            return;
        }
        // Rotar torreta
        float x = Input.GetAxis("Horizontal");
        if (x != 0)
        {
            transform.Rotate(Vector3.up * Speed * x);
        }


        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float lastYaw = currentYaw;
        currentYaw += mouseX;
        currentYaw = Mathf.Clamp(currentYaw, yawMin, yawMax);
        if (currentYaw == yawMax || currentYaw == yawMin)
        {
            mouseX = currentYaw - lastYaw;
        }

        mouseY = -Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        float lastPitch = currentPitch;
        currentPitch += mouseY;
        currentPitch = Mathf.Clamp(currentPitch, pitchMin, pitchMax);
        if (currentPitch == pitchMax || currentPitch == pitchMin)
        {
            mouseY = currentPitch - lastPitch;
        }

        camera.Rotate(Vector3.right * mouseY);
        camera.RotateAround(transform.position, transform.up, mouseX);

        arm.Rotate(Vector3.right * mouseY);
        arm.RotateAround(transform.position, transform.up, mouseX);

        // Disparar
        if (Input.GetButton("Fire1"))
        {
            if (audios[1].isPlaying)
                audios[1].Stop();
            just_aturat = true;

            if (!audios[0].isPlaying)
                audios[0].Play();

            if (barrel_rotation < max_v_barrel)
                barrel_rotation += 0.5f;

            //start particle system
            if (!muzzelFlash.isPlaying)
                muzzelFlash.Play();

            if (Time.time >= nextFire)
            {
                nextFire = Time.time + FireRate;
                RaycastHit hit;
                if (Physics.Raycast(camera.position, camera.forward, out hit))
                {
                    if (hit.transform.tag == "Enemic")
                    {
                        hit.transform.GetComponent<ScriptEnemic>().ferir(mal_bala, -hit.normal * forca_bala, 0.5f);
                        Instantiate((GameObject)Resources.Load("prefabs/WFX_BImpact SoftBody", typeof(GameObject)), hit.point, Quaternion.LookRotation(hit.normal));
                        dispars_encertats++;
                    }
                    else
                    {
                        Instantiate((GameObject)Resources.Load("prefabs/WFX_BImpact Dirt", typeof(GameObject)), hit.point, Quaternion.LookRotation(hit.normal));
                        dispars_fallats++;
                    }
                }
            }
        }
        else
        {
            particulesDispar();
        }

        // Rotar per disparar
        if(barrel_rotation > 0)
        {
            Barrel.Rotate(Vector3.forward, barrel_rotation);
        }
    }

    public void particulesDispar()
    {
        if (audios[0].isPlaying)
            audios[0].Stop();

        if (just_aturat && !audios[1].isPlaying)
        {
            audios[1].Play();
            just_aturat = false;
        }

        if (barrel_rotation > 0)
        {
            barrel_rotation -= 0.1f;
        }

        // stop the particle system
        if (muzzelFlash.isPlaying)
        {
            muzzelFlash.Stop();
        }
    }

    public int getDisparsFallats()
    {
        return dispars_fallats;
    }

    public int getDisparsEncertats()
    {
        return dispars_encertats;
    }

}