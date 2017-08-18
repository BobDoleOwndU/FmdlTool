using System;
using System.IO;
using System.Reflection;

namespace FmdlTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
                using (FileStream stream = new FileStream(args[0], FileMode.Open))
                {
                    Hashing.ReadDictionary(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\dictionary.txt");

                    Fmdl file = new Fmdl();
                    file.Read(stream);
                    file.OutputSection0Block3Info();
                    stream.Close();
                } //using
            Console.ReadKey();
        } //Main
    } //class
} //namespace
