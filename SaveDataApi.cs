using System.Collections.Generic;
using System.IO;

namespace TheifCloack
{
    class DataApi
    {

        public Dictionary<int, Dictionary<string, string>> Read(string filename)
        {
            string[] lines = File.ReadAllText(filename).Split(char.Parse("-"));

            Dictionary<int, Dictionary<string, string>> data = new Dictionary<int, Dictionary<string, string>>();

            int counter = 0;
            foreach(string line in lines)
            {
                string[] datas = line.Trim().Split(char.Parse("\n"));
                data.Add(counter, new Dictionary<string, string>(){
                    {"has_watch", datas[0]},
                    {"max_charge", datas[1]}
                });
                counter += 1;
            }
            return data;
        }

        public void GenDef(string filename)
        {
            
            List<string> g = new List<string>{
                "false",
                "100",
                "-",
                "false",
                "100",
                "-",
                "false",
                "100",
                "-",
                "false",
                "100",
            };
            

            File.WriteAllLines(filename, g.ToArray());
        }

        public void Write(string filename, int id, string has, string max)
        {
            string[] lines = File.ReadAllText(filename).Split(char.Parse("-"));

            Dictionary<int, Dictionary<string, string>> data = new Dictionary<int, Dictionary<string, string>>();

            int counter = 0;
            foreach(string line in lines)
            {
                string[] datas = line.Trim().Split(char.Parse("\n"));
                data.Add(counter, new Dictionary<string, string>(){
                    {"has_watch", datas[0]},
                    {"max_charge", datas[1]}
                });
                counter += 1;
            }
            data[id] = new Dictionary<string, string>(){
                {"has_watch", has},
                {"max_charge", max}
            };

            List<string> to = new List<string>();
            foreach(int k in data.Keys)
            {
                Dictionary<string, string> xd = data[k];
                to.Add(xd["has_watch"]);
                to.Add(xd["max_charge"]);
                to.Add("-");
            }
            to.RemoveAt(to.Count - 1);

            string done = string.Join("\n", to.ToArray());
            File.WriteAllText(filename, done);
        }
    }
}