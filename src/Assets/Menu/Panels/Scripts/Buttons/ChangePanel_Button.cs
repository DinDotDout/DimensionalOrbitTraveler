using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangePanel_Button : MonoBehaviour
{
    [SerializeField] GameObject nextMenu;
    [SerializeField] protected AnimatorFunctions animatorFunctions;

    public void next()
    {
        Fade();
    }

    private void Fade()
    {
        animatorFunctions.Disabled = true;
        CanvasGroup cg = transform.parent.gameObject.GetComponent<CanvasGroup>();

        //Toggle the end value depending on the faded state ( from 1 to 0)
        StartCoroutine(DoFade(cg, cg.alpha, 0));
    }

    private IEnumerator DoFade(CanvasGroup cg,float start, float end)//Runto complition beforex
    {
        float Duration = 0.5f;
        float counter = 0f;

        while (counter < Duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / Duration);
            yield return null; //we don't need a return value.
        }
        animatorFunctions.Disabled = false;
        nextMenu.SetActive(true);
        nextMenu.GetComponent<CanvasGroup>().alpha = 1;
        transform.parent.gameObject.SetActive(false);
    }
}
