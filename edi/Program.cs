using OopFactory.X12.Parsing;
using OopFactory.X12.Parsing.Model;
using OopFactory.X12.Transformations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace edi
{
    class Program
    {

        static void Main(string[] args)
        {
            EdiParser parser;
            try
            {
                string inputFileName = "sampleRandom.txt";
            
                parser = new EdiParser();
                parser.ShowPaths();
                parser.x12ToXml(inputFileName);
                parser.x12Tox12WithSpaces(inputFileName);
                parser.x12Tox12Html(inputFileName);

                //Util.WriteLine(parser.x12ToXml("sample837.txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }

    }
    public static class Util
    {
        public static void Write(string message)
        {
            Console.Write(message);
        }

        public static void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }

    class EdiParser
    {
        string projectDirectory { get; set; }
        string x12Directory { get; set; }
        string xmlDirectory { get; set; }
        string x12WithSpacesDirectory { get; set; }
        string x12HtmlDirectory { get; set; }

        public EdiParser()
        {
            init();

        }

        void init()
        {
            projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"));
            x12Directory = Path.GetFullPath(Path.Combine(projectDirectory, @"x12\"));
            x12WithSpacesDirectory = Path.GetFullPath(Path.Combine(projectDirectory, @"x12WithSpaces\"));
            xmlDirectory = Path.GetFullPath(Path.Combine(projectDirectory, @"xml\"));
            x12HtmlDirectory = Path.GetFullPath(Path.Combine(projectDirectory, @"x12Html\"));
        }


        public void x12ToXml(string sourceFileName)
        {
            sourceFileName = Path.Combine(x12Directory, sourceFileName);
            using (FileStream fstream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
            {
                var x12Parser = new X12Parser();
                Interchange interchange = x12Parser.Parse(fstream);
                string xml = interchange.Serialize();
                toFile(Path.Combine(xmlDirectory, Path.GetFileNameWithoutExtension(sourceFileName) + ".xml"), xml);
            }
        }

        public void x12Tox12WithSpaces(string sourceFileName)
        {
            sourceFileName = Path.Combine(x12Directory, sourceFileName);
            using (FileStream fstream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
            {
                var x12Parser = new X12Parser();
                Interchange interchange = x12Parser.Parse(fstream);
                string x12WithSpaces = interchange.SerializeToX12(true);
                toFile(Path.Combine(x12WithSpacesDirectory, Path.GetFileNameWithoutExtension(sourceFileName) + ".txt"), x12WithSpaces);
            }

        }

        public void x12Tox12Html(string sourceFileName)
        {
            sourceFileName = Path.Combine(x12Directory, sourceFileName);
            using (Stream fstream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
            {
                var htmlService = new X12HtmlTransformationService(new X12EdiParsingService(suppressComments: false));
                string html = htmlService.Transform(new StreamReader(fstream).ReadToEnd());
                toFile(Path.Combine(x12HtmlDirectory, Path.GetFileNameWithoutExtension(sourceFileName) + ".html"), html);
            }

        }



        public void toFile(string fileName, string content)
        {
            FileStream stream;
            StreamWriter writer;
            using (stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (writer = new StreamWriter(stream))
            {
                try
                {
                    writer.Write(content);
                }
                catch (Exception ex)
                {
                    Util.WriteLine(ex.Message);
                }
            }
        }

        public void ShowPaths()
        {
            Console.WriteLine(projectDirectory);
            Console.WriteLine(x12Directory);
        }
    }
}
