using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Random_Song_Generator
{
    public static class NameGenerator
    {
        private static string resourcePrefix = "Random_Song_Generator.Dictionary.";
        private static string resourceSuffix = ".txt";
        private static string[] resourceNames = new string[] {"Aa","Ao","Ba","Bo","Ca","Co","Da","Do","E","F","G","H","I","J","K","L","M",
                                                "N","O","Pa","Po","Q","Ra","Ri","Sa","Sl","Su","T","U","V","W","X","Y","Z"};
        private static string[] dictionary = null;
        private static Random rand = new Random();

        public static string generateName()
        {
            if (dictionary == null)
            {
                loadDictionary();
            }
            int numberOfWords = rand.Next(1,4);

            string name = "";

            for (int i = 0; i < numberOfWords; i++)
            {
                name += generateRandomWord();
                name += " ";  //dont forget to trim!
            }

            return name.Trim();
        }

        private static string generateRandomWord()
        {
            int seed = rand.Next(0,dictionary.Length);
            return dictionary[seed];
        }

        //re-using an old dictionary I have that has been split up into pieces
        //ideally the dictionary would be in a single file but i dont want to merge it back together
        private static void loadDictionary()
        {
            List<string> wordList = new List<string>();

            try
            {

                Assembly Asm = Assembly.GetExecutingAssembly();

                for (int i = 0; i < resourceNames.Length; i++)
                {
                    Stream strm = Asm.GetManifestResourceStream(resourcePrefix + resourceNames[i] + resourceSuffix);

                    StreamReader reader = new StreamReader(strm);

                    string data = reader.ReadToEnd();
                    string[] words = data.Split('\n');
                    for (int j = 0; j < words.Length; j++)
                    {
                        wordList.Add(words[j].Trim());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            dictionary = wordList.ToArray();
        }
    }

}
