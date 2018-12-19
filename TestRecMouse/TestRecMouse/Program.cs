using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RecognitionApp;

namespace TestRecMouse
{
    class Program
    {
        static void Main(string[] args)
        {
            var truePositive = new int[9];
            var falseNegative = new int[9];
            var falsePositive = new int[9];
            var error = 0;
            string path = @"C:\Users\ACER\source\repos\RecognitionApp\RecognitionApp\UserGestures.xml";
            var PGC = new PerfectGesturesClass(path);
            XmlSerializer serializer = new XmlSerializer(typeof(GesturesCollection));
            StreamReader reader = new StreamReader(path);
            var gesturesCollection = (GesturesCollection)serializer.Deserialize(reader);
            reader.Close();
            Console.Write("Выберите номер тестируемого алгоритма:\n" +
                "1)Алгоритм Дугласа-Пекера\n" +
                "2)Алгоритм характеристических точек\n" +
                "3)Алгоритм ключа\n" +
                "4)Алгоритм характеристические точки + ключ\n");
            var numberOfAlgorithm = Convert.ToInt32(Console.ReadLine());
            gesturesCollection.Gesture[0].UserPath.Count();
            for (int i = 0; i < gesturesCollection.Gesture.Count(); i++)
            {
                for (int j = 0; j < gesturesCollection.Gesture[i].UserPath.Count(); j++)
                {
                    var points = PGC.AdopterPoints(gesturesCollection.Gesture[i].UserPath[j].Path);
                    var rec = new RecognitionMouse(points);
                    int index = -1;
                    switch (numberOfAlgorithm)
                    {
                        case 1: 
                            {
                                points = rec.DouglasPeucker();
                                index = rec.WhatIsItIndex();
                                break;
                            }
                        case 2:
                            {
                                points = rec.IdentifyChar();
                                index = rec.WhatIsItIndex();
                                break;
                            }
                        case 3:
                            {
                                index = rec.WhatIsItWithKey();
                                break;
                            }
                        case 4:
                            {
                                index = rec.WhatIsItWithIdealKey();
                                break;
                            }
                    }if (index == -1)
                    {
                        error++;
                    }
                    else
                    {
                        if (index == i)
                        {
                            truePositive[i]++;
                        }
                        else
                        {
                            falseNegative[i]++;
                            falsePositive[index]++;
                        }
                    }
                }
                Console.WriteLine(i);
            }
            using (var strWr = new StreamWriter(@"C:\Users\ACER\source\repos\RecognitionApp\text.txt"))
            {
                for (int i = 0; i < 9; i++)
                {
                    strWr.WriteLine($"{PGC.IdealGestures[i].Name} TruePositive  = {truePositive[i]} False Negative = {falseNegative[i]} False Positive = {falsePositive[i]}");
                }
                strWr.WriteLine($"Error {error}");
            }
        }
    }
}
