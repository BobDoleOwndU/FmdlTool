using System.IO;

namespace FmdlTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
                using (FileStream stream = new FileStream(args[0], FileMode.Open))
                {
                    Fmdl file = new Fmdl();
                    file.Read(stream);
                    file.OutputSection2Info();
                    stream.Close();
                } //using
        } //Main
    } //class
} //namespace
