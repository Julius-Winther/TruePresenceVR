using UnityEngine;
using System.Collections.Generic;
using Convai.Scripts.Runtime.Features.LipSync;
using Convai.Scripts.Runtime.Core;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.FullSerializer;
using Unity.IO.LowLevel.Unsafe;


public class EmotionHandler : MonoBehaviour
{
    [Tooltip("Check this box to use the 7 emotion pools defined here in the inspector or leave the box unchecked to use the emotion pools defined in code (This has minor performance implications)")]
    [SerializeField] private bool UseVisibleListOfEmotions = false;
    private ConvaiLipSync convaiLipSync;
    private AudioSource audioSource;
    private bool hasAudioSourcePlayed = false;

    [Tooltip("List of emotions pulled from the lip-sync script at run-time")]
    [SerializeField] private List<string> characterEmotions = new List<string>();
     private List<EmotionsEnums> enumsList = new List<EmotionsEnums>();
    private int highestSum = 0;
    private string Redefined_EmotinalState = "";
    [HideInInspector] public int EmotinalState_Blend = 0;


    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool1;
    private int emotionPool1_Sum;
    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool2;
    private int emotionPool2_Sum;
    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool3;
    private int emotionPool3_Sum;
    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool4;
    private int emotionPool4_Sum;
    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool5;
    private int emotionPool5_Sum;
    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool6;
    private int emotionPool6_Sum;
    [Tooltip("A pool of emotions where each entry corresponds to an emotion that a Convai NPC can feel")]
    [SerializeField] private List<string> emotionPool7;
    private int emotionPool7_Sum;


    void Awake()
    {
        convaiLipSync = GetComponent<ConvaiLipSync>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(audioSource.isPlaying && hasAudioSourcePlayed == false)
        {
            if(UseVisibleListOfEmotions)
            {
                FindEmotinalState();
            }
            else
            {
                FindEmotinalState_V2();
            }
            hasAudioSourcePlayed = true;
        }
        if(!audioSource.isPlaying)
        {
            hasAudioSourcePlayed = false;
        }
    }

    
    private void FindEmotinalState()
    {   
        if(convaiLipSync.GetCharacterEmotions() != null)
        {
            characterEmotions = convaiLipSync.GetCharacterEmotions();


            emotionPool1_Sum = 0;
            emotionPool2_Sum = 0;
            emotionPool3_Sum = 0;
            emotionPool4_Sum = 0;
            emotionPool5_Sum = 0;
            emotionPool6_Sum = 0;
            emotionPool7_Sum = 0;

            highestSum = 0;
            Redefined_EmotinalState = "";

            foreach(string emotion in characterEmotions)
            {
                for (int i = 0; i < emotionPool1.Count; i++)
                {    
                    if(emotion == emotionPool1[i])
                    {
                        emotionPool1_Sum += 1;
                    }
                }
                
                for (int i = 0; i < emotionPool2.Count; i++)
                {
                    if(emotion == emotionPool2[i])
                    {
                        emotionPool2_Sum += 1;
                    }
                }

                for (int i = 0; i < emotionPool3.Count; i++)
                {
                    if(emotion == emotionPool3[i])
                    {
                        emotionPool3_Sum += 1;
                    }
                }

                for (int i = 0; i < emotionPool4.Count; i++)
                {
                    if(emotion == emotionPool4[i])
                    {
                        emotionPool4_Sum += 1;
                    }
                }

                for (int i = 0; i < emotionPool5.Count; i++)
                {
                    if(emotion == emotionPool5[i])
                    {
                        emotionPool5_Sum += 1;
                    }
                }

                for (int i = 0; i < emotionPool6.Count; i++)
                {
                    if(emotion == emotionPool6[i])
                    {
                        emotionPool6_Sum += 1;
                    }
                }

                for (int i = 0; i < emotionPool7.Count; i++)
                {
                    if(emotion == emotionPool7[i])
                    {
                        emotionPool7_Sum += 1;
                    }
                }
            }

            if(emotionPool1_Sum > highestSum)
            {
                highestSum = emotionPool1_Sum;
                Redefined_EmotinalState = "State1";
            }
            if(emotionPool2_Sum > highestSum)
            {
                highestSum = emotionPool2_Sum;
                Redefined_EmotinalState = "State2";
            }
            if(emotionPool3_Sum > highestSum)
            {
                highestSum = emotionPool3_Sum;
                Redefined_EmotinalState = "State3";
            }
            if(emotionPool4_Sum > highestSum)
            {
                highestSum = emotionPool4_Sum;
                Redefined_EmotinalState = "State4";
            }
            if(emotionPool5_Sum > highestSum)
            {
                highestSum = emotionPool5_Sum;
                Redefined_EmotinalState = "State5";
            }
            if(emotionPool6_Sum > highestSum)
            {
                highestSum = emotionPool6_Sum;
                Redefined_EmotinalState = "State6";
            }
            if(emotionPool7_Sum > highestSum)
            {
                highestSum = emotionPool7_Sum;
                Redefined_EmotinalState = "State7";
            }

            Debug.Log("Redefined EmotinalState: " + Redefined_EmotinalState + " (Sum Score: " + highestSum + ")");
        }
    
    }

    private void FindEmotinalState_V2()
    {
        if(convaiLipSync.GetCharacterEmotions() != null)
        {
            characterEmotions = convaiLipSync.GetCharacterEmotions();

            emotionPool1_Sum = 0;
            emotionPool2_Sum = 0;
            emotionPool3_Sum = 0;
            emotionPool4_Sum = 0;
            emotionPool5_Sum = 0;
            emotionPool6_Sum = 0;
            emotionPool7_Sum = 0;

            highestSum = 0;
            Redefined_EmotinalState = "";


            foreach(string emotion in characterEmotions)
            {
                switch (emotion)
                {

                //-------------------Pool_1-------------------
                case "Serenity":
                    emotionPool1_Sum += 1;
                    break;

                case "Acceptance":
                    emotionPool1_Sum += 1;
                    break;

                case "Trust":
                    emotionPool1_Sum += 1;
                    break;

                case "Interest":
                    emotionPool1_Sum += 1;
                    break;

                //-------------------Pool_2-------------------
                case "Joy":
                    emotionPool2_Sum += 1;
                    break;

                case "Admiration":
                    emotionPool2_Sum += 1;
                    break;

                case "Anticipation":
                    emotionPool2_Sum += 1;
                    break;

                case "Vigilance":
                    emotionPool2_Sum += 1;
                    break;
                
                //-------------------Pool_3-------------------
                case "Ecstasy":
                    emotionPool3_Sum += 1;
                    break;

                case "Amazement":
                    emotionPool3_Sum += 1;
                    break;

                case "Surprise":
                    emotionPool3_Sum += 1;
                    break;

                //-------------------Pool_4-------------------
                case "Apprehension":
                    emotionPool4_Sum += 1;
                    break;

                case "Fear":
                    emotionPool4_Sum += 1;
                    break;

                case "Terror":
                    emotionPool4_Sum += 1;
                    break;

                //-------------------Pool_5-------------------
                case "Pensiveness":
                    emotionPool5_Sum += 1;
                    break;

                case "Sadness":
                    emotionPool5_Sum += 1;
                    break;

                case "Grief":
                    emotionPool5_Sum += 1;
                    break;
                
                //-------------------Pool_6-------------------
                case "Boredom":
                    emotionPool6_Sum += 1;
                    break;

                case "Disgust":
                    emotionPool6_Sum += 1;
                    break;

                case "Loathing":
                    emotionPool6_Sum += 1;
                    break;
                
                //-------------------Pool_7-------------------
                case "Annoyance":
                    emotionPool7_Sum += 1;
                    break;

                case "Anger":
                    emotionPool7_Sum += 1;
                    break;

                case "Rage":
                    emotionPool7_Sum += 1;
                    break;
                //----------------------------------------------
                
                }
            } 
            
            if(emotionPool1_Sum > highestSum)
            {
                highestSum = emotionPool1_Sum;
                Redefined_EmotinalState = "State1";
                EmotinalState_Blend = 0;
            }
            if(emotionPool2_Sum > highestSum)
            {
                highestSum = emotionPool2_Sum;
                Redefined_EmotinalState = "State2";
                EmotinalState_Blend = 1;
            }
            if(emotionPool3_Sum > highestSum)
            {
                highestSum = emotionPool3_Sum;
                Redefined_EmotinalState = "State3";
                EmotinalState_Blend = 2;
            }
            if(emotionPool4_Sum > highestSum)
            {
                highestSum = emotionPool4_Sum;
                Redefined_EmotinalState = "State4";
                EmotinalState_Blend = 3;
            }
            if(emotionPool5_Sum > highestSum)
            {
                highestSum = emotionPool5_Sum;
                Redefined_EmotinalState = "State5";
                EmotinalState_Blend = 4;
            }
            if(emotionPool6_Sum > highestSum)
            {
                highestSum = emotionPool6_Sum;
                Redefined_EmotinalState = "State6";
                EmotinalState_Blend = 5;
            }
            if(emotionPool7_Sum > highestSum)
            {
                highestSum = emotionPool7_Sum;
                Redefined_EmotinalState = "State7";
                EmotinalState_Blend = 6;
            }

            Debug.Log("Redefined EmotinalState: " + Redefined_EmotinalState + " (Sum Score: " + highestSum + ")");
        }
    }
}

public enum EmotionsEnums
{
   // State Of Mind 
   /* Yellow */ Serenity = 1, Joy = 1, Ecstasy = 1, 
   /* Light Green */ Acceptance = 1, Trust = 1, Admiration = 1,
   /* Green */ Apprehension = 1, Fear = 1, Terror = 1,
   /* Blue */ Distraction = 1, Surprise = 1, Amazement = 1,
   /* Purple */ Pensiveness = 1, Sadness = 1, Grief = 1,
   /* Pink */ Boredom = 1, Disgust = 1, Loathing = 1,
   /* Red */ Annoyance = 1, Anger = 1, Rage = 1,
   /* Orange */ Interest = 1, Anticipation = 1, Vigilance = 1

}