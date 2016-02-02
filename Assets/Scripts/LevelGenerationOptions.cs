using UnityEngine;
using System;

/// <summary>
/// All parameters necessary to generate levels of varying difficulties.
/// </summary>
[Serializable]
public class LevelGenerationOptions
{
    public int safesToGenerate = 12;
    public int safesPerRow = 3;
    public int safesActiveAtStart = 5;  //Amount of active safes at start
    public bool activateRandomly = false;
    public int maxLives = 3;
    public float punishmentDuration = 5.0f; //Duration of a punishment if we lose a life
    public int minTimer = 60;
    public int maxTimer = 90;

    public Color[] colorGroups;
    public Type[] availableChallenges = { 
		typeof(ButtonChallenge),
		typeof(ButtonSequenceChallenge),
        typeof(InterconnectedDialChallenge),
		typeof(SlidersKnobsChallenge),
        typeof(SynchronousSlidersChallenge),
		typeof(AccelerometerChallenge),
        typeof(PasswordReelsChallenge),
	};

    // will be extended with additional parameters and logic later
}
