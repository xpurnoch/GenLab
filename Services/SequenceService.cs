using System.Text;

namespace BioLabManager.Services
{
	public static class SequenceService
	{
		public static string DnaToMrnaComplement(string dna)
		{
			if (string.IsNullOrWhiteSpace(dna)) 
				return "";

			var rna = new StringBuilder();
			foreach (char nucleotide in dna.ToUpper())
			{
				switch (nucleotide)
				{
					case 'A': rna.Append('U'); break;
					case 'T': rna.Append('A'); break;
					case 'C': rna.Append('G'); break;
					case 'G': rna.Append('C'); break;
					default: rna.Append(' '); break;
				}
			}
			return rna.ToString();
		}

		public static string TranslateRnaToProtein(string rna)
		{
			var codonTable = new Dictionary<string, string>
            {
				// Start
				["AUG"] = "M",

				// Stop
				["UAA"] = "*", ["UAG"] = "*", ["UGA"] = "*",

				// Amino acids
				["UUU"] = "F", ["UUC"] = "F",
				["UUA"] = "L", ["UUG"] = "L", ["CUU"] = "L", ["CUC"] = "L", ["CUA"] = "L", ["CUG"] = "L",
				["AUU"] = "I", ["AUC"] = "I", ["AUA"] = "I",
				["GUU"] = "V", ["GUC"] = "V", ["GUA"] = "V", ["GUG"] = "V",

				["UCU"] = "S", ["UCC"] = "S", ["UCA"] = "S", ["UCG"] = "S", ["AGU"] = "S", ["AGC"] = "S",
				["CCU"] = "P", ["CCC"] = "P", ["CCA"] = "P", ["CCG"] = "P",
				["ACU"] = "T", ["ACC"] = "T", ["ACA"] = "T", ["ACG"] = "T",
				["GCU"] = "A", ["GCC"] = "A", ["GCA"] = "A", ["GCG"] = "A",

				["UAU"] = "Y", ["UAC"] = "Y",
				["CAU"] = "H", ["CAC"] = "H",
				["CAA"] = "Q", ["CAG"] = "Q",
				["AAU"] = "N", ["AAC"] = "N",
				["AAA"] = "K", ["AAG"] = "K",

				["GAU"] = "D", ["GAC"] = "D",
				["GAA"] = "E", ["GAG"] = "E",

				["UGU"] = "C", ["UGC"] = "C",
				["UGG"] = "W",

				["CGU"] = "R", ["CGC"] = "R", ["CGA"] = "R", ["CGG"] = "R", ["AGA"] = "R", ["AGG"] = "R",
				["GGU"] = "G", ["GGC"] = "G", ["GGA"] = "G", ["GGG"] = "G"
			};

			var protein = new StringBuilder();
			for (int i = 0; i <= rna.Length - 3; i += 3)
			{
				var codon = rna.Substring(i, 3);
				protein.Append(codonTable.TryGetValue(codon, out var aa) ? aa : "?");
			}
			return protein.ToString();
		}
	}
}
