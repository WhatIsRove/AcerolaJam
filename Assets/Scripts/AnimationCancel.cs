using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationCancel : MonoBehaviour
{
    public SkinnedMeshRenderer[] meshes;

    public Material negative;
    public Material positive;

    void AllowCancel()
    {
        FindObjectOfType<PlayerController>().canCancel = true;
    }

    void DisAllowCancel()
    {
        FindObjectOfType<PlayerController>().canCancel = false;
        FindObjectOfType<PlayerController>().anim.SetBool("Cancel", false);
    }

    void PushPlayer()
    {
        FindObjectOfType<PlayerController>().PushPlayer();
    }

    void Dodge()
    {
        FindObjectOfType<PlayerController>().Dodge();
        AudioManager.Instance.Play("Dodge");
    }

    void ResetMaxSpeed()
    {
        FindObjectOfType<PlayerController>().ResetMaxSpeed();
    }

    void Whoosh()
    {
        AudioManager.Instance.Play("SwordWoosh0");
    }

    void SetSpiritMode()
    {
        Material[] mats = new Material[] { negative };
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.materials = mats;
        }
    }

    void LeaveSpiritMode()
    {
        Material[] mats = new Material[] { positive };
        foreach (SkinnedMeshRenderer mesh in meshes)
        {
            mesh.materials = mats;
        }
    }
}
