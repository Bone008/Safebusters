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
    public int AOASAS = 5;  //Amount of active safes at start

    public Color[] colorGroups;
    public Type[] availableChallenges = { 
		typeof(ButtonChallenge),
		typeof(ButtonSequenceChallenge)
	};

    // will be extended with additional parameters and logic later
}
