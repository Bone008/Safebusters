using System.Collections.Generic;
using Model.Challenges;
using UnityEngine;

namespace Model
{
    public class Level
    {
        //public List<ColorGroup> ColorGroups { get { return new List<ColorGroup>(); } }
		public Color[] ColorGroups { get; set; }
		public List<Safe> Safes { get; set; }
		public int SafeCount { get { return 12; } }
        public int SafesPerRow { get { return 4; } }

        public Level()
        {
			// init color groups
			ColorGroups = new Color[] {
				new Color (0.5f, 0.7f, 0.6f),
				new Color (0.7f, 0.7f, 0.5f)
			};

			// create random safes (not spawned yet)
            Safes = new List<Safe>();
			for (int i = 0; i < SafeCount; i++) {
				// get challenge
				IChallenge challenge;
				if (Random.value > 0.3)
					challenge = new ButtonChallenge();
				else
					challenge = new PlaceholderChallenge();

				// get random colorgroup
				Color color = ColorGroups[Random.Range(0, ColorGroups.Length)];

				// get number of safes to activate
				int numberToActivate = 1; // complicated algorithm

				// add new safe to list
				Safes.Add (new Safe (challenge, color, numberToActivate) {
					IsActive = Random.value > 0.2f, /* remove later */
					IsOpen = Random.value < 0.1f /* remove later */					
				});
			}
        }
    }
}