using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : StaticInstance<SoundManager>
{
    [SerializeField] AudioClip[] _BGM;
    [SerializeField] AudioSource _musicPlayer;

    private void Update()
    {
        if (!_musicPlayer.isPlaying)
        {
            AudioSystem.Instance.PlayMusic(ChooseClip());
        }
    }

 

    private AudioClip ChooseClip()
    {
        int clipIndex = Random.Range(0, _BGM.Length);
        return _BGM[clipIndex];
    }
}
