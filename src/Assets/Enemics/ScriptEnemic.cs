using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ScriptEnemic : MonoBehaviour
{
    // Public vars
    public float mal = 2f;
    public float vida_max = 100f;
    public float lookRadius = 10f;
    public float combatRadius = 1f;
    public float Speed = 2f;
    public float WalkSpeed = 1.5f;
    public float turnSpeed = 150f;
    public float temps_estuneat = 1f;
    public Canvas canvas;
    public Canvas marcador;
    public Image healthbar;

    // System vars
    AudioSource[] audios;
    GameObject controls_generals;
    Rigidbody rb;
    Collider collider;
    Animator animator;
    Vector3 pos_influencia;
    Transform planet;
    Transform objectiu;
    Planeta script_planeta;
    float estuneat = 0;
    float vida;
    bool atacat = false;
    int iframes = 0;
    bool ja_mort = false;
    float distance_to_player = 0;

    void Awake()
    {
        audios = GetComponents<AudioSource>();


        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<Transform>();
        script_planeta = GameObject.FindGameObjectWithTag("Planet").GetComponent<Planeta>();
        controls_generals = GameObject.FindGameObjectWithTag("General");
        collider = gameObject.GetComponent<Collider>();
        vida = vida_max;
        canvas.enabled = false;

        animator = GetComponent<Animator>();
        pos_influencia = transform.position;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Vector3 gravityUp = (transform.position - planet.position).normalized;
        // Gravetat i Alinear amb planeta
        rb.AddForce(gravityUp * (-script_planeta.shapeSettings.planetGravity));
        transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;

        if (iframes > 0)
        {
            iframes--;
        }

        if (vida < vida_max)
        {
            canvas.enabled = true;
            canvas.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform.position, transform.up);
            healthbar.fillAmount = vida / vida_max;
        }

        if (objectiu != null)
        {
            distance_to_player = (objectiu.position - transform.position).magnitude;
            float v = animator.GetFloat("Velocitat");

            if ((distance_to_player <= lookRadius) && ((objectiu.position - pos_influencia).magnitude < lookRadius * 1.5))
            {
                // Mirar al jugador
                transform.LookAt(objectiu.position, transform.up);

                // Moure per envant
                if (distance_to_player > combatRadius && estuneat <= 0)
                {
                    Vector3 localMove = transform.TransformVector(v * Speed * Vector3.forward) * Time.deltaTime;
                    rb.MovePosition(rb.position + localMove);

                    if (v < 1f)
                        animator.SetFloat("Velocitat", v + 0.1f);
                    animator.SetBool("Combat", false);
                }// Combat
                else
                {
                    if (v > 0f)
                        animator.SetFloat("Velocitat", v - 0.1f);

                    if (estuneat > 0)
                    {
                        estuneat -= 0.05f;
                        animator.SetBool("Combat", false);
                    }
                    else
                    {
                        animator.SetBool("Combat", true);
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Atacar"))
                        {
                            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && !atacat)
                            {
                                controls_generals.GetComponent<Controls_generals>().ferir(mal);
                                atacat = true;

                                audios[0].Play();
                            }
                            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
                            {
                                atacat = false;
                            }
                        }
                    }
                }
            }
            else
            {

                if ((transform.position - pos_influencia).magnitude > 2f)
                {
                    //tornam al centre de influencia
                    transform.LookAt(pos_influencia, transform.up);

                    Vector3 localMove = transform.TransformVector(v * Speed * Vector3.forward) * Time.deltaTime;
                    rb.MovePosition(rb.position + localMove);
                }
                else
                {
                    //si ens hi trobam aprop ens aturam
                    if (v > 0f)
                    {
                        animator.SetFloat("Velocitat", v - 0.1f);
                    }
                    if (vida != vida_max)
                        vida++;
                }

                animator.SetBool("Combat", false);
            }
        }
        else
        {
            // Marcador en cas de nau
            marcador.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform.position, transform.up);

            float v = animator.GetFloat("Velocitat");
            if (v > 1)
                animator.SetFloat("Velocitat", v - 0.1f);
            else if (v < 1)
                animator.SetFloat("Velocitat", v + 0.1f);


            Vector3 localMove = transform.TransformVector(v * WalkSpeed * Vector3.forward) * Time.deltaTime;
            rb.MovePosition(rb.position + localMove);
        }
    }

    public void setObjectiu(Transform objectiu, float look_dist, float comb_dist, float velocitat)
    {
        this.objectiu = objectiu;

        if (objectiu != null)
            marcador.enabled = false;

        if (look_dist != 0)
            lookRadius = look_dist;

        if (comb_dist != 0)
            combatRadius = comb_dist;

        Speed *= velocitat;
    }

    public bool ferir(float mal, Vector3 forca, float delay)
    {
        vida -= mal;
        rb.AddForce(forca, ForceMode.VelocityChange);
        estuneat = temps_estuneat;
        if (vida <= 0)
        {
            if (!ja_mort)
            {
                ja_mort = true;
                GameObject mort = Instantiate((GameObject)Resources.Load("prefabs/Mort", typeof(GameObject)), transform.position, transform.rotation);
                mort.GetComponent<script_audio_mort>().Play(audios[1]);
                controls_generals.GetComponent<Controls_generals>().mort(this.name);
                Invoke("morir", delay);
            }

            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EspaDOTe") && iframes == 0)
        {
            //other.gameObject.transform.root.
            iframes = 30;
            // calcular angulo
            Vector3 dir = objectiu.position - transform.position;
            // normalizamos el vector
            dir = dir.normalized;

            // Blood Spurt towards player
            GameObject blood = Instantiate((GameObject)Resources.Load("prefabs/WFX_BImpact SoftBody", typeof(GameObject)), collider.bounds.center, Quaternion.LookRotation(dir));
            blood.transform.parent = transform;
            GameObject p = transform.gameObject;
            bool damage = objectiu.gameObject.GetComponent<Player>().Strong;
            if(damage)
                ferir(80, -dir * 5, 0.5f);
            else
                ferir(40, -dir * 5, 0.5f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((objectiu == null || distance_to_player > combatRadius) && collision.gameObject.CompareTag("item"))
        {
            transform.Rotate(0, 5, 0, Space.Self);
        }
        else if ((objectiu == null || distance_to_player > combatRadius) && collision.gameObject.CompareTag("Enemic"))
        {
            transform.Rotate(0, 5, 0, Space.Self);
        }
    }

    private void morir()
    {
        Instantiate((GameObject)Resources.Load("prefabs/WFX_Explosion_No_sound", typeof(GameObject)), transform.position, transform.rotation);
        Destroy(gameObject);
    }

    float SphericalDistance(Vector3 position1, Vector3 position2, float radius)
    {
        return Mathf.Acos(Vector3.Dot(Vector3.Normalize(position1), Vector3.Normalize(position2))) * radius;
    }

}