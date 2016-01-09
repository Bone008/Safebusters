using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Controller of the level container that holds all safes.
/// </summary>
public class Level : MonoBehaviour
{

    public GameObject safePrefab;
    public LevelGenerationOptions generationOptions = new LevelGenerationOptions();
    public LevelChallengePrefabs challengePrefabs = new LevelChallengePrefabs();
    public AnimationCurve doorOpeningSpeed;

    [HideInInspector]
    public List<Safe> safes;

    void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
		// get safe-bounds
        Bounds bounds = safePrefab.GetComponent<Safe>().GetFrameBounds();
        float safeWidth = bounds.size.x;
        float safeHeight = bounds.size.y;

		// calculate base offset
        int rows = Mathf.CeilToInt(generationOptions.safesToGenerate / (float)generationOptions.safesPerRow);
        Vector3 baseOffset = new Vector3((-generationOptions.safesPerRow + 1) * safeWidth / 2, rows * safeHeight - safeHeight / 2, 0);
		Vector3 offset;
        
		// create safe gameobjects and logic
		safes = new List<Safe>();

        for (int i = 0; i < generationOptions.safesToGenerate; i++)
        {
			// create GameObject
            var go = Instantiate(safePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            go.name = "Safe " + i;

            // offset from top left position
            offset = new Vector3(safeWidth * (i % generationOptions.safesPerRow), -safeHeight * (int)(i / generationOptions.safesPerRow), 0);
            go.transform.localPosition = baseOffset + offset;

			// chose challenge
            Type challengeType = generationOptions.availableChallenges[UnityEngine.Random.Range(0, generationOptions.availableChallenges.Length)];
            AbstractChallenge challenge = (AbstractChallenge)go.AddComponent(challengeType);

			// instantiate challenge-prefabs
            GameObject frontPrefab = challengePrefabs.GetFrontPrefab(challengeType);
            GameObject backPrefab = challengePrefabs.GetBackPrefab(challengeType);

			// create Safe logic
			Safe safe = go.GetComponent<Safe>();
			safe.challenge = challenge;
            safe.SpawnChallengeObjects(frontPrefab, backPrefab);     
            safe.SetBackwards(i % 2 == 1);
            safe.SetMaxTimer(UnityEngine.Random.Range(30, 61));
            safe.doorOpeningSpeed = doorOpeningSpeed;
            safe.SetActive(true);

            // get random colorgroup (should be improved at some point)
            Color color = generationOptions.colorGroups[UnityEngine.Random.Range(0, generationOptions.colorGroups.Length)];
            safe.SetDisplayColor(color);

			// set 1 safe to activate after solved challenge (should be improved at some point)
            safe.SetNumberOfSafesToActivate(1);

            safes.Add(safe);
        }
    }


    void Update()
    {
    }
}
