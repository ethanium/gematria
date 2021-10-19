using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gematria.Services
{
    public class TextService
    {
        public List<string> Read(string path)
        {
            string[] results = File.ReadAllLines(path);
            return results.ToList();
        }

        public void Write(string path, string phrase)
        {
            phrase = phrase.Replace("  ", " ").Trim().ToUpper();

            string[] lines = File.ReadAllLines(path);
            List<string> upperList = lines.ToList();
            upperList = upperList.Select(x => x.Trim().ToUpper()).ToList();
            lines = upperList.ToArray();
            string firstWord = phrase.Split(',').Select(x => x.Trim()).First().ToUpper();
            if (lines.Contains(firstWord.ToUpper()) || lines.Contains(phrase))
            {
                int index = lines.ToList().IndexOf(firstWord);
                if (index > -1)
                {
                    lines[index] = phrase;
                }
            }
            else
            {
                List<string> list = lines.ToList();
                list.Add(phrase);
                lines = list.ToArray();
            }
            File.WriteAllLines(path, lines);
        }
    }
}
