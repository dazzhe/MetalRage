using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : SingletonBehaviour<SoundManager> {
    public List<AudioClip> BGMList;
    public List<AudioClip> SEList;
    public int MaxSE = 20;
    private AudioSource bgmSource = null;
    private List<AudioSource> seSources = null;
    private Dictionary<string, AudioClip> bgmDict = null;
    private Dictionary<string, AudioClip> seDict = null;

    private void Start() {
        //create listener
        if (FindObjectsOfType(typeof(AudioListener)).All(o => !((AudioListener)o).enabled)) {
            this.gameObject.AddComponent<AudioListener>();
        }
        //create audio sources
        this.bgmSource = this.gameObject.AddComponent<AudioSource>();
        this.seSources = new List<AudioSource>();

        //create clip dictionaries
        this.bgmDict = new Dictionary<string, AudioClip>();
        this.seDict = new Dictionary<string, AudioClip>();

        Action<Dictionary<string, AudioClip>, AudioClip> addClipDict = (dict, c) => {
            if (!dict.ContainsKey(c.name)) {
                dict.Add(c.name, c);
            }
        };

        this.BGMList.ForEach(bgm => addClipDict(this.bgmDict, bgm));
        this.SEList.ForEach(se => addClipDict(this.seDict, se));
    }

    public void PlaySE(string seName) {
        if (!this.seDict.ContainsKey(seName)) {
            throw new ArgumentException(seName + " not found", "seName");
        }

        AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null) {
            if (this.seSources.Count >= this.MaxSE) {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            this.seSources.Add(source);
        }

        source.clip = this.seDict[seName];
        source.volume = 0.3f;
        source.Play();
    }

    public void StopSE() {
        this.seSources.ForEach(s => s.Stop());
    }

    public void PlayBgm(string bgmName) {
        if (!this.bgmDict.ContainsKey(bgmName)) {
            throw new ArgumentException(bgmName + " not found", "bgmName");
        }

        if (this.bgmSource.clip == this.bgmDict[bgmName]) {
            return;
        }

        this.bgmSource.Stop();
        this.bgmSource.clip = this.bgmDict[bgmName];
        this.bgmSource.Play();
    }

    public void StopBgm() {
        this.bgmSource.Stop();
        this.bgmSource.clip = null;
    }
}
