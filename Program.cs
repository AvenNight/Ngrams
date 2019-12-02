using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ngrams
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 2)
            {
                Console.WriteLine("Input arguments is invalid! Default arguments set.");
                args = new[] { "text1.txt", "text2.txt", "2" };
                if (!File.Exists(args[0]))
                    File.Create(args[0]);
                if (!File.Exists(args[1]))
                    File.Create(args[1]);
                // loh
            }
            //foreach (var e in args) Console.WriteLine(e);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            string text1 = File.ReadAllText(args[0], Encoding.Default);
            string text2 = File.ReadAllText(args[1], Encoding.Default);
            int n = int.Parse(args[2]);
            var grams1 = GetGrams(ParseSentences(text1), n);
            var grams2 = GetGrams(ParseSentences(text2), n);

            Console.WriteLine($"Text №1: {args[0]}");
            Console.WriteLine($"Text №2: {args[1]}\n");
            Console.WriteLine($"{n}-Grams count in Text №1:  {grams1.Count()}");
            Console.WriteLine($"{n}-Grams count in Text №2:  {grams2.Count()}");
            Console.WriteLine($"Jaccard Index: {"",12}{GetJaccardIndex(grams1, grams2)}");

            watch.Stop();

            Console.WriteLine("\nOperations time: {0,11} ms", watch.ElapsedMilliseconds);

            Console.ReadKey();
        }

        public static List<List<string>> ParseSentences(string text)
        {
            char[] sentSeps = { '.', '!', '?', ';', '(', ')', '{', '}', '[', ']' };

            return text.ToLower().Split(sentSeps)
                .Select(sent =>
                    Regex.Split(sent, @"[^\w']+")
                    .Where(w => !string.IsNullOrEmpty(w)).ToList())
                .ToList();
        }

        public static List<string> GetGrams(List<List<string>> text, int n)
        {
            HashSet<string> grams = new HashSet<string>();

            foreach (List<string> sentence in text)
                for (int i = 0; i <= sentence.Count - n; i++)
                {
                    string gram = "";
                    for (int j = 0; j < n; j++)
                    {
                        if (j == 0)
                            gram += sentence[i + j];
                        else
                            gram += $" {sentence[i + j]}";
                    }

                    if (!grams.Contains(gram.ToString()))
                        grams.Add(gram.ToString());
                }

            return grams.OrderBy(g => g.Length).ThenBy(g => g).ToList();
        }

        public static double GetJaccardIndex(List<string> grams1, List<string> grams2)
        {
            double delta = grams1.Intersect(grams2).Count();
            double total = grams1.Union(grams2).Count();

            return delta / total;
        }
    }
}
