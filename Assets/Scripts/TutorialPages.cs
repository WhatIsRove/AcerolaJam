using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPages : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;

    int currentPage = 1;

    public Animator anim;


    public void NextPage()
    {
        currentPage++;
        ChangePage();
    }

    void ChangePage()
    {
        AudioManager.Instance.Play("PageFlip");
        switch (currentPage)
        {
            case 1:
                page1.SetActive(true);
                page2.SetActive(false);
                break;
            case 2:
                page1.SetActive(false);
                page2.SetActive(true);
                break;
            case 3:
                anim.SetTrigger("Hide");
                FindObjectOfType<PlayerController>().anim.SetTrigger("GetUp");
                break;
        }
    }
}
