using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Demo Player
public class SimpleMusicPlayer : MonoBehaviour {

    public KeyCode nextKey = KeyCode.UpArrow;
    public KeyCode prevKey = KeyCode.DownArrow;

    public Text displayText;

    public AudioSource audioSource;
    public AudioClip[] list;

    private int index = 0;



    // Use this for initialization
    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        DisplayTitle();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(nextKey))
        {
            Next();
        }
        else if (Input.GetKeyUp(prevKey))
        {
            Prev();
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public void Next()
    {
        index = (int)Mathf.Repeat(++index, list.Length);
        if (audioSource != null && audioSource.isPlaying)
            Play(index);
        else
            DisplayTitle();
    }

    public void Prev()
    {
        index = (int)Mathf.Repeat(--index, list.Length);
        if (audioSource != null && audioSource.isPlaying)
            Play(index);
        else
            DisplayTitle();
    }

    public void Play()
    {
        Play(index);
    }

    public void Play(int i)
    {
        if (audioSource == null || list == null || list.Length == 0)
            return;

        Stop();

        if (0 <= i && i < list.Length)
        {
            if (list[i] != null)
            {
                audioSource.clip = list[i];
                audioSource.Play();

                index = i;
#if UNITY_EDITOR
                Debug.Log("SimpleMusicPlayer : index = " + index);
#endif
                DisplayTitle();
            }
        }
    }

    public void Stop()
    {
        if (audioSource == null || list == null || list.Length == 0)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private void DisplayTitle()
    {
        if (displayText == null)
            return;

        if (0 <= index && index < list.Length)
        {
            if (list[index] != null)
                displayText.text = list[index].name;
        }
    }

}
