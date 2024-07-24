using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audios_Personatge : MonoBehaviour
{
    [SerializeField] AudioClip atack1;
    [SerializeField] AudioClip atack2;
    [SerializeField] AudioClip atack3;

    [SerializeField] AudioClip dashAtack;

    [SerializeField] AudioClip strongAtack1;
    [SerializeField] AudioClip strongAtack2;


    [SerializeField] AudioClip jump1;
    [SerializeField] AudioClip jump2;

    [SerializeField] AudioClip hit1;
    [SerializeField] AudioClip hit2;

    [SerializeField] AudioClip die;

    public AudioClip GetAttackClip(int i)
    {
        switch (i)
        {
            case 1:
                return atack1;
            case 2:
                return atack2;
            case 3:
                return atack3;
            default:
                return null;
        }
    }

    public AudioClip GetDashAttackClip()
    {
        return dashAtack;
    }

    public AudioClip GetStrongAttackClip(int i)
    {
        switch (i)
        {
            case 1:
                return strongAtack1;
            case 2:
                return strongAtack2;
            default:
                return null;
        }
    }

    public AudioClip GetJumpClip(int j)
    {
        switch (j)
        {
            case 1:
                return jump1;
            case 2:
                return jump2;
            default:
                return null;
        }
    }

    int h = 0;
    public AudioClip GetHitClip()
    {
        h = h % 2;
        h++;
        switch (h)
        {
            case 1:
                return hit1;
            case 2:
                return hit1;
            default:
                return null;
        }
    }

    public AudioClip GetDieClip()
    {
        return die;
    }
}
