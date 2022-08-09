using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip[] sounds;

    public static AudioController ins;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
    }

    public static void PlaySound(int index)
    {
        AudioSource source = ins.gameObject.AddComponent<AudioSource>();
        source.clip = ins.sounds[index];
        source.loop = false;
        source.Play();
        ins.StartCoroutine(ins.DestroySound(source));
    }

    public static void PlaySound(AudioClip clip)
    {
        AudioSource source = ins.gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = false;
        source.Play();
        ins.StartCoroutine(ins.DestroySound(source));
    }

    IEnumerator DestroySound(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        Destroy(source);
    }
}
