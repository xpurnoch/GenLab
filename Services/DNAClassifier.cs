namespace BioLabManager.Services
{
	public static class DNAClassifier
	{
		public static string PredictOrganism(string sequence)
		{
			if (string.IsNullOrWhiteSpace(sequence)) 
				return "Unknown ❓";

			sequence = sequence.ToUpper();
			int a = sequence.Count(c => c == 'A');
			int t = sequence.Count(c => c == 'T');
			int c = sequence.Count(c => c == 'C');
			int g = sequence.Count(c => c == 'G');
			int total = a + t + c + g;

			if (total == 0) 
				return "Invalid sequence 🚫";

			double gcContent = (double)(g + c) / total;
			int length = sequence.Length;

			// --- repetitions ---
			bool hasCpG = sequence.Contains("CGCG") || sequence.Contains("CGC");
			bool hasTATA = sequence.Contains("TATAAA");
			bool hasCAAT = sequence.Contains("CAAT");
			bool hasAlu = sequence.Contains("GGCCGGGCGCGGTGGCT");
			bool hasRBS = sequence.Contains("AGGA");
			bool atRich = ((double)(a + t) / total) > 0.65;

			// --- organism score ---
			var scores = new Dictionary<string, int>
			{
				{ "Bacteria 🧫", 0 },
				{ "Virus 🦠", 0 },
				{ "Human 🧑", 0 },
				{ "Mouse 🐁", 0 },
				{ "Primate 🐒", 0 },
				{ "Plant 🌿", 0 },
				{ "Fungi 🍄", 0 }
			};

			// --- bacteria ---
			if (gcContent > 0.55) scores["Bacteria 🧫"] += 2;
			if (length < 1500) scores["Bacteria 🧫"] += 1;
			if (hasRBS) scores["Bacteria 🧫"] += 2;

			// --- virus ---
			if (length < 300) scores["Virus 🦠"] += 3;
			if (atRich) scores["Virus 🦠"] += 1;

			// --- human ---
			if (length > 1000 && gcContent < 0.5) scores["Human 🧑"] += 2;
			if (hasAlu) scores["Human 🧑"] += 3;
			if (hasTATA || hasCAAT || hasCpG) scores["Human 🧑"] += 2;

			// --- mouse ---
			if (length > 400 && gcContent > 0.5 && hasCpG) scores["Mouse 🐁"] += 3;
			if (hasTATA || hasCAAT) scores["Mouse 🐁"] += 1;

			// --- primate ---
			if (length > 1000 && gcContent >= 0.5 && gcContent <= 0.6) scores["Primate 🐒"] += 3;
			if (hasCpG || hasTATA) scores["Primate 🐒"] += 1;

			// --- plant ---
			if (gcContent < 0.4 && length > 600) scores["Plant 🌿"] += 2;
			if (atRich) scores["Plant 🌿"] += 1;

			// --- fungi ---
			if (gcContent > 0.65 && length > 500) scores["Fungi 🍄"] += 3;

			var best = scores.OrderByDescending(kv => kv.Value).First();
			return best.Value > 0 ? best.Key : "Unknown ❓";
		}
	}

}
