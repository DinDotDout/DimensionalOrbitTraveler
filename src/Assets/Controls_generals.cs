using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controls_generals : MonoBehaviour
{
    // Planet
    GameObject planeta;

    // Enemys
    GameObject Enemics;

    public int temps_nau = 60;

    //Pause variables & objects
    GameObject Pause;
    public static bool game_paused = false;
    public static bool end_screen = false;

    // TOWER MODE
    // Tower mode variables
    public int temps_torre = 120;
    public float EnemyRate = 1f;
    float nextEnemy = 0f;
    float time_start;
    // Tower Mode objects
    GameObject Torre;
    GameObject Portals;
    GameObject Altars;
    // Audio Timer Que
    AudioSource[] audios;

    // PROTA MODE
    GameObject Player;
    bool lastAudioisFirst = true;

    // SPACESHIP MODE
    GameObject SpaceShip;

    // PLAYER AND TOWER VARIABLES
    public float vida_max = 100;
    private float vida = 100;
    // Hud hp
    GameObject barra_vida;

    // TOWER AND SPACESHIP VARIABLES
    GameObject mira;

    // PLAYER AND SPACESHIP VARIABLES
    int nEnemies;

    // HUD
    // Hud Image
    public Sprite hud_player = null;
    public Sprite hud_torre = null;
    public Sprite hud_spaceship = null;
    // Hud Elements
    Text estatObjectiu;
    Text n_vida;
    Image imatge_vida;
    Image imatge_hud;
    GameObject objectiu;

    // Enemy kill count
    Dictionary<string, int> enemics_morts;
    int disp_fallats = 0;
    int disp_encertats = 0;

    public enum GameMode
    {
        SpaceShip = 0,
        Player = 1,
        Tower = 2
    }
    GameMode gameMode;
    bool activate;

    // Start is called before the first frame update
    void Start()
    {
        audios = GetComponents<AudioSource>();

        imatge_hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Image>();
        estatObjectiu = GameObject.FindGameObjectWithTag("Estat").GetComponent<Text>();
        n_vida = GameObject.FindGameObjectWithTag("n_vida").GetComponent<Text>();
        barra_vida = GameObject.FindGameObjectWithTag("barra_vida");
        objectiu = GameObject.FindGameObjectWithTag("Objectiu");
        imatge_vida = GameObject.FindGameObjectWithTag("vida").GetComponent<Image>();
        planeta = GameObject.FindGameObjectWithTag("Planet");
        Planeta.resolution = 256;
        planeta.GetComponent<Planeta>().GeneratePlanet();
        mira = GameObject.FindGameObjectWithTag("Mira");
        Pause = GameObject.FindGameObjectWithTag("Pause");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameMode = (GameMode)0;
        activate = true;

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
            SceneManager.UnloadSceneAsync(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        switch (gameMode)
        {
            case GameMode.SpaceShip:
                ModeNau();
                break;
            case GameMode.Player:
                ModeProta();
                break;
            case GameMode.Tower:
                ModeTorre();
                break;
            default:
                //End
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (end_screen)
                {
                    Guanyar();
                } else
                {
                    Perdre();
                }
                break;
        }
    }

    public void ChangeMode()
    {
        Time.timeScale = 0;
        DestroyMode();
        gameMode = (GameMode)(((int)gameMode + 1));
        activate = true;
    }

    public void DestroyMode()
    {
        aturaAudios();

        if (Enemics != null)
            DestroyImmediate(Enemics);


        switch (gameMode)
        {
            case GameMode.SpaceShip:
                if (SpaceShip != null)
                    DestroyImmediate(SpaceShip);
                break;
            case GameMode.Player:
                if (Player != null)
                    DestroyImmediate(Player);
                break;
            case GameMode.Tower:
                if (Torre != null)
                    DestroyImmediate(Torre);
                if (Portals != null)
                    DestroyImmediate(Portals);
                if (Altars != null)
                    DestroyImmediate(Altars);
                break;
            default:
                break;
        }
    }

    public void ModeNau()
    {
        if (activate)
        {
            aturaAudios();

            disp_fallats = 0;
            disp_encertats = 0;

            audios[2].Play();
            enemics_morts = new Dictionary<string, int>();
            mira.SetActive(true);
            barra_vida.SetActive(true);
            objectiu.SetActive(true);
            imatge_hud.sprite = hud_spaceship;
            initVida(temps_nau);
            SpaceShip = Instantiate((GameObject)Resources.Load("prefabs/nave", typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity);
            planeta.GetComponent<Planeta>().GenerateItems();
            nEnemies = Mathf.Clamp((int)(0.3 * planeta.GetComponent<Planeta>().GenerateEnemies(null)), 12, 50);
            end_screen = false;
            activate = false;
            InfoRound();
        }

        // Update Enemy kills
        SetEnemyCount();
        setComptador();
    }

    public void ModeProta()
    {
        if (activate)
        {
            aturaAudios();
            audios[3].Play();
            lastAudioisFirst = true;

            enemics_morts = new Dictionary<string, int>();
            mira.SetActive(false);
            barra_vida.SetActive(true);
            objectiu.SetActive(true);
            initVida(100);
            imatge_hud.sprite = hud_player;
            planeta.GetComponent<Planeta>().GenerateItems();
            Player = Instantiate((GameObject)Resources.Load("prefabs/Prota", typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity);
            nEnemies = Mathf.Clamp((int)(0.2 * planeta.GetComponent<Planeta>().GenerateEnemies(Player.transform)), 10, 50);
            end_screen = false;
            activate = false;

            InfoRound();
        }
        // Update Enemy kills
        SetEnemyCount();

        // Change track
        if (!audios[3].isPlaying && !audios[4].isPlaying)
        {
            if (lastAudioisFirst)
                audios[4].Play();
            else
                audios[3].Play();
            lastAudioisFirst = !lastAudioisFirst;
        }
    }

    public void ModeTorre()
    {
        if (activate)
        {
            aturaAudios();
            audios[5].Play();

            enemics_morts = new Dictionary<string, int>();
            mira.SetActive(true);
            barra_vida.SetActive(true);
            objectiu.SetActive(true);
            initVida(200);
            initRellotge(temps_torre);
            imatge_hud.sprite = hud_torre;

            Torre = planeta.GetComponent<Planeta>().GenerateTorre();
            planeta.GetComponent<Planeta>().GeneratePortalsAltars();
            planeta.GetComponent<Planeta>().GenerateItems();
            planeta.GetComponent<Planeta>().InitEnemies();
            end_screen = false;
            activate = false;

            InfoRound();
        }

        // Spawn enemics
        if (Time.time >= nextEnemy)
        {
            nextEnemy = Time.time + EnemyRate;
            planeta.GetComponent<Planeta>().GenerateEnemiesAtPortals();
        }

        // Update clock
        int tempsRestant = temps_torre - (int)(Time.time - time_start);

        if (tempsRestant >= 0)
            setRellotge(tempsRestant);
        else
        {
            if (!end_screen)
            {
                disp_encertats += Torre.GetComponentInChildren<Torreta>().getDisparsEncertats();
                disp_fallats += Torre.GetComponentInChildren<Torreta>().getDisparsFallats();
            
                RoundEnd();
            }
        }
    }

    private void aturaAudios()
    {
        foreach (AudioSource aud in audios)
        {
            aud.Stop();
        }
    }

    public void initVida(float v)
    {
        imatge_vida.color = Color.green;
        vida_max = v;
        vida = v;
        imatge_vida.fillAmount = vida / vida_max;
        n_vida.text = vida + " / " + vida_max;
    }

    public void SetEnemyCount()
    {
        if (!end_screen)
        {
            estatObjectiu.text = nEnemies.ToString();
            estatObjectiu.color = Color.white;
            if (nEnemies <= 0)
            {
                estatObjectiu.text = "0";
                if (gameMode == GameMode.SpaceShip)
                {
                    disp_fallats += SpaceShip.GetComponent<Controls_Nau>().getDisparsFallats();
                    disp_encertats += SpaceShip.GetComponent<Controls_Nau>().getDisparsEncertats();
                }
                RoundEnd();
            }
        }
    }

    public void initRellotge(int segons)
    {
        time_start = Time.time;
        setRellotge(segons);
        estatObjectiu.color = Color.white;
    }

    public void setRellotge(int segons)
    {
        if (segons <= 0)
        {
            segons = 0;
        }

        if (segons <= 10 && segons > 0)
        {
            if (!audios[0].isPlaying)
                audios[0].Play();
        }

        estatObjectiu.text = (segons / 60) + " : " + (segons % 60);
    }

    public void initComptador(int segons)
    {
        imatge_vida.color = Color.green;
        vida_max = segons;
        vida = segons;
        imatge_vida.fillAmount = vida / vida_max;
        n_vida.text = ((int)vida / 60) + " : " + ((int)vida % 60);
    }

    public void setComptador()
    {
        if (!end_screen)
        {
            vida -= 1 * Time.deltaTime;
            if (vida <= 0)
            {
                Perdre();
            }
            else
            {
                if (vida / vida_max > 0.3f && vida / vida_max <= 0.7f)
                    imatge_vida.color = Color.yellow;
                if (vida / vida_max <= 0.3f)
                    imatge_vida.color = Color.red;
                imatge_vida.fillAmount = vida / vida_max;
                n_vida.text = ((int)vida / 60) + " : " + ((int)vida % 60);
            }
        }
    }

    public void PauseGame()
    {
        if (game_paused)
        {
            Pause.transform.GetChild(0).gameObject.SetActive(false);
            Pause.transform.GetChild(1).gameObject.SetActive(false);
            Pause.transform.GetChild(2).gameObject.SetActive(false);
            Pause.transform.GetChild(3).gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
            game_paused = false;
        }
        else
        {
            if (audios[0].isPlaying)
                audios[0].Stop();

            if (audios[1].isPlaying)
                audios[1].Stop();
            if (end_screen)
            {
                Pause.transform.GetChild(3).gameObject.SetActive(true);
            } else
            {
                Pause.transform.GetChild(0).gameObject.SetActive(true);
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            game_paused = true;
        }
    }

    public void InfoRound()
    {
        Pause.transform.GetChild(2).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        game_paused = true;

        switch (gameMode)
        {
            case GameMode.SpaceShip:
                Pause.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
                Pause.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
                Pause.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
                break;
            case GameMode.Player:
                Pause.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
                Pause.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                Pause.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(false);
                break;
            case GameMode.Tower:
                Pause.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
                Pause.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
                Pause.transform.GetChild(2).transform.GetChild(2).gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void RoundEnd()
    {
        Pause.transform.GetChild(3).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        end_screen = true;
        game_paused = true;

        switch (gameMode)
        {
            case GameMode.SpaceShip:
                Pause.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
                Pause.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
                Pause.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
                break;
            case GameMode.Player:
                Pause.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
                Pause.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);
                Pause.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
                break;
            case GameMode.Tower:
                Pause.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
                Pause.transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
                Pause.transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ferir(float mal)
    {
        vida -= mal;
        if (vida <= 0)
        {
            if (!end_screen)
            {
                Perdre();
            }
        }
        else
        {
            if (vida / vida_max > 0.3f && vida / vida_max <= 0.7f)
                imatge_vida.color = Color.yellow;
            if (vida / vida_max <= 0.3f)
                imatge_vida.color = Color.red;
            imatge_vida.fillAmount = vida / vida_max;
            n_vida.text = vida + " / " + vida_max;
        }
    }

    public void mort(string nom)
    {
        nEnemies--;
        if (enemics_morts.ContainsKey(nom))
        {
            enemics_morts[nom] = enemics_morts[nom] + 1;
        }
        else
        {
            enemics_morts.Add(nom, 1);
        }
    }

    public void Guanyar()
    {
        aturaAudios();

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadSceneAsync(4, LoadSceneMode.Single);
    }

    public void Perdre()
    {
        aturaAudios();

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);
    }
}
