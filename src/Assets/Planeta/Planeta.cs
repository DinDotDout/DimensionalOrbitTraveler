using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomaEnum
{
    Forest = 0,
    Desert = 1,
    Tundra = 2
}

public class Planeta : MonoBehaviour
{
    [Range(2, 256)]
    public static int resolution = 100;
    public bool autoUpdate = true;

    public static BiomaEnum bioma = BiomaEnum.Forest;

    public static int planet_seed = 00000000;
    public float densitat_enemics = 5f;
    public static float densitat_objectes = 5f;
    public float dist_torre = 10f;
    public float dist_portals = 10f;
    public float dist_altars = 2f;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingFold;
    [HideInInspector]
    public bool colorSettingFold;

    [HideInInspector]
    public ShapeGenerator shapeGenerator = new ShapeGenerator();
    [HideInInspector]
    public ColorGenerator colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshCollider[] meshColliders;
    Terreno[] terreno;

    System.Random rand;
    HashSet<int> V_item;

    GameObject Torre;
    GameObject enemics;
    GameObject portals;
    GameObject altars;

    bool finished;

    void Initialize()
    {
        Finished = true;
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        shapeGenerator.UpdateSettings(shapeSettings, planet_seed);
        colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        if (meshColliders == null || meshColliders.Length == 0)
        {
            meshColliders = new MeshCollider[6];
        }
        terreno = new Terreno[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        GameObject mesh = new GameObject("Mesh");
        mesh.tag = "Mesh";
        mesh.transform.parent = transform;
        mesh.transform.position = transform.position;
        for (int i = 0; i < 6; i++)
        {

            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = mesh.transform;
            meshObj.transform.position = meshObj.transform.parent.position;
            meshObj.layer = 9;

            meshObj.AddComponent<MeshRenderer>();

            Mesh actualMesh = new Mesh();
            meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = actualMesh;

            meshColliders[i] = meshObj.AddComponent<MeshCollider>();

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terreno[i] = new Terreno(shapeGenerator, actualMesh, meshColliders[i], resolution, directions[i]);
        }
        V_item = new HashSet<int>();
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
    }

    public void GeneratePlanetMenu()
    {
        Initialize();
        GenerateMeshMenu();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        foreach (Terreno face in terreno)
        {
            face.constructMesh();
        }
        UpdateBiome();
        GenerateItems();
    }

    void GenerateMeshMenu()
    {
        StartCoroutine(GenerateEachMesh());
    }

    IEnumerator GenerateEachMesh()
    {
        Finished = false;
        foreach (Terreno face in terreno)
        {
            yield return StartCoroutine(face.constructMeshConcurrent());
        }

        UpdateBiome();
        GenerateItems();
        Finished = true;
    }

    public void UpdateBiome()
    {
        colorGenerator.UpdateBioma((int)bioma);
        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    public bool Finished { get => finished; set => finished = value; }

    public void DestroyItems()
    {
        Transform mesh = GameObject.FindGameObjectWithTag("Mesh").GetComponent<Transform>();

        foreach (Transform child in mesh)
        {
            while (child.childCount != 0)
            {
                DestroyImmediate(child.GetChild(0).gameObject);
            }
        }
    }

    public void GenerateItems()
    {
        V_item = new HashSet<int>();
        
        rand = new System.Random(planet_seed);

        DestroyItems();
        float sq_distance = Mathf.Pow(dist_torre, 2);
        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j < (shapeSettings.planetRadius / (100 / shapeSettings.planetRadius)) * densitat_objectes; j++)
            {
                int val = rand.Next(meshFilters[i].sharedMesh.vertices.Length);
                while (!V_item.Add(val))
                {
                    val = rand.Next(meshFilters[i].sharedMesh.vertices.Length);
                }

                Vector3 world_v = localToWorld.MultiplyPoint3x4(meshFilters[i].sharedMesh.vertices[val]);

                int item_sel;

                bool vertical = false;
                bool gros = false;

                string prefab = null;
                switch ((int)bioma)
                {
                    case 0: //Plain
                        item_sel = rand.Next(10);
                        switch (item_sel)
                        {
                            case 0:
                                if (!Physics.CheckBox(world_v, new Vector3(1.0f, 1f, 1.0f), new Quaternion(0, 0, 0, 0), 9)) //Hem de mirar si se solapa amb un altre objecte
                                {
                                    prefab = "RockPlain";
                                    gros = true;
                                }
                                break;
                            case 1:
                            case 2:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 0.5f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "TreePlain";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 3:
                            case 4:
                            case 5:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 0.01f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "ReedPatch1";
                                }
                                break;
                            case 6:
                            case 7:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 0.01f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "ReedPatch2";
                                }
                                break;
                            default:
                                if (!Physics.CheckBox(world_v, new Vector3(0.25f, 0.01f, 0.25f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "ReedPlain";
                                }
                                break;
                        }
                        break;

                    case 1: //Desert
                        item_sel = rand.Next(20);
                        switch (item_sel)
                        {
                            case 0:
                                if (!Physics.CheckBox(world_v, new Vector3(1.0f, 1f, 1.0f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "deadtree";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 1:
                                if (!Physics.CheckBox(world_v, new Vector3(1.0f, 1f, 1.0f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "palmtree";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 2:
                            case 3:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 0.5f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "cactus";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                if (!Physics.CheckBox(world_v, new Vector3(0.25f, 0.01f, 0.25f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "small_bush";
                                }
                                break;
                            case 9:
                            case 10:
                                if (!Physics.CheckBox(world_v, new Vector3(1f, 0.01f, 1f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "bones";
                                }
                                break;
                            default:
                                //Fail placing
                                break;
                        }
                        break;
                    case 2: //Tundra
                        item_sel = rand.Next(20);
                        switch (item_sel)
                        {
                            case 0:
                            case 1:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 2f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "pine_snow3";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 2:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 2f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "pine_snow2";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 3:
                            case 4:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 2f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "pine_snow1";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 5:
                                if (!Physics.CheckBox(world_v, new Vector3(0.5f, 2f, 0.5f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "pine";
                                    gros = true;
                                    vertical = true;
                                }
                                break;
                            case 6:
                            case 7:
                                if (!Physics.CheckBox(world_v, new Vector3(1f, 1f, 1f), new Quaternion(0, 0, 0, 0), 9))
                                {
                                    prefab = "ice_spike";
                                    gros = true;
                                }
                                break;
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                                if (!Physics.CheckBox(world_v, new Vector3(0.25f, 0.01f, 0.25f), new Quaternion(0,0,0,0), 9))
                                {
                                    prefab = "ReedTundra";
                                }
                                break;
                            default: //fail
                                break;
                        }
                        break;
                    default:
                        break;
                }

                bool enfora = true;
                if (Torre != null)
                {
                    if ((world_v - Torre.transform.position).sqrMagnitude < sq_distance)
                    {
                        enfora = false;
                    }
                }

                if (prefab != null && (!gros || enfora))
                {
                    GameObject item = Instantiate((GameObject)Resources.Load("prefabs/" + prefab, typeof(GameObject)), world_v, Quaternion.identity);
                    item.transform.parent = meshFilters[i].transform;
                    item.tag = "item";

                    Vector3 vecUp;
                    if (vertical) //Miram si la seva posició s'ha d'alinear amb el terreno o amb el nucli del planeta
                    {
                        vecUp = (item.transform.position - transform.position).normalized;
                    } else
                    {
                        vecUp = meshFilters[i].sharedMesh.normals[val];
                    }
                    item.transform.rotation = Quaternion.FromToRotation(item.transform.up, vecUp) * item.transform.rotation;
                }
            }
        }
    }

    public void DestroyEnemies()
    {
        if (enemics != null)
            DestroyImmediate(enemics);
    }

    public int GenerateEnemies(Transform objectiu)
    {
        int nEnemies = 0;
        if (V_item == null)
            V_item = new HashSet<int>();

        if(enemics != null)
            DestroyImmediate(enemics);

        enemics = new GameObject("Enemics");
        enemics.tag = "Enemics";
        enemics.transform.parent = transform;

        System.Random r = new System.Random();

        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j < (shapeSettings.planetRadius / (100 / shapeSettings.planetRadius)) * densitat_enemics; j++)
            {
                int val = r.Next(meshFilters[i].sharedMesh.vertices.Length);
                while (!V_item.Add(val))
                {
                    val = r.Next(meshFilters[i].sharedMesh.vertices.Length);
                }
                Vector3 world_v = localToWorld.MultiplyPoint3x4(meshFilters[i].sharedMesh.vertices[val]);

                string prefab = null;
                switch ((int)bioma)
                {
                    case 0: //Plain
                        switch (Random.Range(1, 5))
                        {
                            case 1:
                            case 2:
                            case 3:
                                prefab = "Slime";
                                break;
                            default:
                                prefab = "TurtleShell";
                                break;
                        }
                        break;
                    case 1: //Desert
                        switch (Random.Range(1, 4))
                        {
                            case 1:
                                prefab = "Wolf";
                                break;
                            case 2:
                                prefab = "TurtleShell";
                                break;
                            default:
                                prefab = "Esqueleto";
                                break;
                        }
                        break;
                    case 2: //Tundra
                        switch (Random.Range(1, 6))
                        {
                            case 1:
                            case 2:
                                prefab = "Knight";
                                break;
                            case 3:
                                prefab = "Wolf";
                                break;
                            default:
                                prefab = "Esqueleto";
                                break;
                        }
                        break;
                    default:
                        break;
                }
                

                GameObject item = Instantiate((GameObject)Resources.Load("prefabs/" + prefab, typeof(GameObject)), world_v, Quaternion.identity);

                item.transform.parent = enemics.transform;
                Vector3 gravityUp = (item.transform.position - transform.position).normalized;
                Vector3 localUp = item.transform.up;
                item.transform.rotation = Quaternion.FromToRotation(localUp, gravityUp) * item.transform.rotation;
                item.GetComponent<ScriptEnemic>().setObjectiu(objectiu, 0f, 0f, 1f);
                nEnemies++;
            }
        }
        return nEnemies;
    }

    public void InitEnemies()
    {
        if (enemics != null)
            DestroyImmediate(enemics);

        enemics = new GameObject("Enemics");
        enemics.tag = "Enemics";
        enemics.transform.parent = transform;
    }

    public void GenerateEnemiesAtPortals()
    {
        if (V_item == null)
            V_item = new HashSet<int>();

        if (rand == null)
            rand = new System.Random(planet_seed);

        string prefab;
        switch (Random.Range(1, 6))
        {
            case 1:
                prefab = "Esqueleto";
                break;
            case 2:
                prefab = "Slime";
                break;
            case 3:
                prefab = "TurtleShell";
                break;
            case 4:
                prefab = "Knight";
                break;
            case 5:
                prefab = "Wolf";
                break;
            default:
                prefab = "Esqueleto";
                break;
        }

        int r = rand.Next(portals.transform.childCount);

        Transform portal = portals.transform.GetChild(r);
        portal.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();

        Vector3 posicio = portal.position;
        posicio = posicio - (posicio - Torre.transform.position).normalized * 1f;

        GameObject item = Instantiate((GameObject)Resources.Load("prefabs/" + prefab, typeof(GameObject)), posicio, Quaternion.identity);

        item.transform.parent = enemics.transform;
        Vector3 gravityUp = (item.transform.position - transform.position).normalized;
        item.transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * item.transform.rotation;
        item.GetComponent<ScriptEnemic>().setObjectiu(altars.transform.GetChild(r).transform.GetChild(3).transform, 100f, 2.7f, 0.5f);
    }

    public GameObject GenerateTorre()
    {
        if(Torre != null)
            DestroyImmediate(Torre);

        rand = new System.Random(planet_seed);
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        int r = rand.Next(5);
        Vector3 world_v = localToWorld.MultiplyPoint3x4(meshFilters[r].sharedMesh.vertices[rand.Next(meshFilters[r].sharedMesh.vertices.Length)]);
        Torre = Instantiate((GameObject)Resources.Load("prefabs/Torre", typeof(GameObject)), world_v, Quaternion.identity);
        Torre.transform.parent = transform;
        Vector3 gravityUp = (Torre.transform.position - transform.position).normalized;
        Vector3 localUp = Torre.transform.up;
        Torre.transform.rotation = Quaternion.FromToRotation(localUp, gravityUp) * Torre.transform.rotation;

        return Torre;
    }

    public void DestroyPortalsAltars()
    {
        DestroyImmediate(portals);
        DestroyImmediate(altars);
    }

    public void GeneratePortalsAltars()
    {
        if (portals != null)
            DestroyImmediate(portals);

        portals = new GameObject("Portals");
        portals.tag = "Portals";
        portals.transform.parent = transform;


        if (altars != null)
            DestroyImmediate(altars);

        altars = new GameObject("Altars");
        altars.tag = "Altars";
        altars.transform.parent = transform;

        Vector3 gravityUp;
        GameObject portal;
        GameObject altar;

        for (float i = 1; i >= -1; i -= 1 * 2)
        {
            for (float j = 1; j >= -1; j -= 1 * 2)
            {
                portal = Instantiate((GameObject)Resources.Load("prefabs/Portal", typeof(GameObject)), Torre.transform.position + i * dist_portals * Torre.transform.right + j * dist_portals * Torre.transform.forward + 2 * Torre.transform.up, Quaternion.identity);
                gravityUp = (portal.transform.position - transform.position).normalized;
                portal.transform.LookAt(Torre.transform, gravityUp);
                portal.transform.parent = portals.transform;

                altar = Instantiate((GameObject)Resources.Load("prefabs/Altar", typeof(GameObject)), Torre.transform.position + i * dist_altars * Torre.transform.right + j * dist_altars * Torre.transform.forward, Quaternion.identity);
                gravityUp = (altar.transform.position - transform.position).normalized;
                altar.transform.up = -gravityUp;
                altar.transform.Translate(0f, 2.0f, 0f, Space.Self);
                altar.transform.parent = altars.transform;
            }
        }
        
    }
}
