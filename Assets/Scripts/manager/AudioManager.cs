using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public Sound[] sounds;

    private void Awake() {
        instance = this;
        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    public void PlayJump() {
        Play($"jump0");
    }

    public void PlayDownForce() {
        Play($"jump1");
    }

    public void PlaySlide() {
        Play("slide0");
    }

    public void PlayDeath() {
        //string index = Random.Range(0, 2).ToString();
        //Play($"death{index}");
        Play("death0");
    }

    public void PlayScore() {
        Play("score0");
    }

    public void PlayStartGame() {
        Play("start0");
    }

    public void PlayHit() {
        Play("hit0");
    }

    public void PlayLeaderboard() {
        Play("leaderboard");
    }

    public void PlayPopup() {
        Play("regular_popup");
    }

    public void PlayToggle() {
        Play("toggle0");
    }

    public void PlayNormalButton() {
        Play("click0");
    }

    public void PlayCloseButton() {
        Play("click1");
    }

    private void Play(string clipName) {
        Sound clip = Array.Find(sounds, s => s.name == clipName);
        if (clip == null) {
            Debug.Log($"Could not find sound name: {clipName}");
        } else {
            clip.source.Play();
        }
    }
}