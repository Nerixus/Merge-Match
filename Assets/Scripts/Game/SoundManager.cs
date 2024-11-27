using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : StaticInstance<SoundManager>
{
    public AudioClip mergeSound;
    public AudioClip mergeFailSound;
    public AudioClip[] objectMoveSound;
    public AudioClip questClaimedSound;
    public AudioClip questCompletedSound;
    public AudioClip newObjectSound;
    public AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayMergeSound() { source.PlayOneShot(mergeSound); }
    public void PlayMergeFailSound() { source.PlayOneShot(mergeFailSound); }
    public void PlayQuestClaimedSound() { source.PlayOneShot(questClaimedSound); }
    public void PlayQuestCompletedSound() { source.PlayOneShot(questCompletedSound); }
    public void PlayNewObjectSound() { source.PlayOneShot(newObjectSound); }
    public void PlayRandomObjectMoveSound()
    {
        AudioClip randomMoveSound = objectMoveSound[Random.Range(0, objectMoveSound.Length)];
        source.PlayOneShot(randomMoveSound);
    }
}
