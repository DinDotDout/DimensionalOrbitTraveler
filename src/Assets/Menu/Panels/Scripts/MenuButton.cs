using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour
{
    public CanvasGroup cg;
    GameObject thisMenu;
    public GameObject nextMenu;
    [SerializeField] AnimatorFunctions animatorFunctions;
    bool isFaded = false;
    float Duration = 0.5f;
    void Start()
    {
        thisMenu = GameObject.FindGameObjectWithTag("Menu");
        //eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    public void play()
    {
        var menuGame = GameObject.FindGameObjectWithTag("Menu Game");
        Fade();
    }

    public void options()
    {
        var menuOptions = GameObject.FindGameObjectWithTag("Menu Options");
        Fade();
    }

    public void quit()
    {
        Application.Quit();
    }
    public void Fade()
    {
        animatorFunctions.Disabled = true;

        //Toggle the end value depending on the faded state ( from 1 to 0)
        StartCoroutine(DoFade(cg.alpha, 0));

        //Toggle the faded state
        isFaded = !isFaded;
    }
    public IEnumerator DoFade(float start, float end)//Runto complition beforex
    {
        float counter = 0f;

        while (counter < Duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / Duration);
            yield return null; //we don't need a return value.
        }
        animatorFunctions.Disabled = false;
        thisMenu.SetActive(false);
        nextMenu.SetActive(true);
        nextMenu.GetComponent<CanvasGroup>().alpha = 1;
    }
}
