using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Remeber to allow permissions for using the mic in the Android App Manifest!

// From this tutorial https://medium.com/@louisvanhove/microphone-input-has-never-been-easier-in-unity-no-library-no-plugins-366062e7c74a

public class MicInput : MonoBehaviour
{
    
    private string _device;
    private AudioClip _clipRecord;
    private int _sampleWindow = 128;
    public float MicLoudness;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MicLoudness = LevelMax();
        this.gameObject.transform.localScale = new Vector3(MicLoudness, MicLoudness, MicLoudness) * 1000 + Vector3.one;
    }

    private void InitializeMic()
    {
        if (_device == null) _device = Microphone.devices[0];
        _clipRecord = Microphone.Start(_device, true, 999, 44100);
    }

    private void StopMic()
    {
        Microphone.End(_device);
    }

    private float LevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1);
        if (micPosition < 0) return 0;
        _clipRecord.GetData(waveData, micPosition);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

    private bool _isInitalized;
    private void OnEnable()
    {
        InitializeMic();
        _isInitalized = true;
    }

    void OnDisable()
    {
        StopMic();
    }

    void OnDestroy()
    {
        StopMic();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (!_isInitalized)
            {
                InitializeMic();
                _isInitalized = true;
            }
        }
        if (!focus)
        {
            StopMic();
            _isInitalized = false;
        }

    }
}
