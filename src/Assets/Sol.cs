using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sol : MonoBehaviour
{
    public float Velocitat = 1;
    public char eix;

    Transform planeta;
    Planeta script_planeta;
    float dist_planeta;

    // Start is called before the first frame update
    void Start()
    {
        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        planeta = planet.GetComponent<Transform>();
        script_planeta = planet.GetComponent<Planeta>();

        if(eix == 'z')
        {
            dist_planeta = planeta.position.z - script_planeta.shapeSettings.planetRadius - 200f;
            transform.position = new Vector3(planeta.position.x, planeta.position.y, dist_planeta);
        } else if(eix == 'x'){
            dist_planeta = planeta.position.x - script_planeta.shapeSettings.planetRadius - 200f;
            transform.position = new Vector3(dist_planeta, planeta.position.y, planeta.position.z);
        } else
        {
            dist_planeta = planeta.position.y - script_planeta.shapeSettings.planetRadius - 200f;
            transform.position = new Vector3(planeta.position.y, dist_planeta, planeta.position.z);
        }
        transform.LookAt(planeta.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Controls_generals.game_paused)
        {
            return;
        }

        if (eix == 'z')
            transform.RotateAround(planeta.position, transform.right, 0.1f * Velocitat);
        else
            transform.RotateAround(planeta.position, transform.up, 0.1f * Velocitat);
    }
}
