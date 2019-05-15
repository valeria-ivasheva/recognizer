using Recognition;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace TestRecMultiStroke
{
    class Program
    {
        static void Main(string[] args)
        {
            var truePositive = new int[19];
            var falseNegative = new int[19];
            var falsePositive = new int[19];
            var error = 0;
            string path = "UserMultiStrokeGestures.xml";//"UserGestures.xml";//"multistrokeGestures.xml";///@"UserMultiStrokeGestures.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(GesturesCollection));
            StreamReader reader = new StreamReader(path);
            var gesturesCollection = (GesturesCollection)serializer.Deserialize(reader);
            reader.Close();
            var PGC = new PerfectGesturesClass(path);
            Console.Write("Выберите номер тестируемого алгоритма:\n" +
                "1)Направление + характеристические точки\n" +
                "2)Хаусдорф, среднее значение и облако точек\n" +
                "3)Алгоритм ключа\n" +
                "4)Алгоритм характеристические точки + ключ\n");
            var numberOfAlgorithm = Convert.ToInt32(Console.ReadLine());
            gesturesCollection.Gesture[0].UserPath.Count();
            for (int i = 0; i < gesturesCollection.Gesture.Count(); i++)
            {
                for (int j = 0; j < gesturesCollection.Gesture[i].UserPath.Count(); j++)
                {
                    var points = PGC.AdopterPoints(gesturesCollection.Gesture[i].UserPath[j].Path);
                    int index = -1;
                    switch (numberOfAlgorithm)
                    {
                        case 1:
                            {
                                //recognizer = new Recognition.Recognizers.CharacteristicsPointsRecognizer();
                                var rec = new RecognitionMouse(path, new Recognition.Recognizers.CharacteristicsPointsRecognizer());
                                index = rec.RecognizeGestures(points);
                                break;
                            }
                        case 2:
                            {
                                //recognizer = new Recognition.Recognizers.MultistrokeRecognizer();
                                var rec = new RecognitionMouse(path, new Recognition.Recognizers.MultistrokeRecognizer());
                                index = rec.RecognizeGestures(points);
                                break;
                            }
                        case 3:
                            {
                                var rec = new RecognitionMouse(path, new Recognition.Recognizers.HungarianRecognizer());
                                index = rec.RecognizeGestures(points);
                                break;
                            }
                        case 4:
                            {
                                
                                break;
                            }
                    }
                    //rec = new RecognitionMouse(path, new Recognition.Recognizers.CharacteristicsPointsRecognizer());                    
                    if (index == -1)
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

                    //switch (numberOfAlgorithm)
                    //{
                    //    case 1:
                    //        {
                    //            points = rec.DouglasPeucker();
                    //            index = rec.WhatIsItIndex();
                    //            break;
                    //        }
                    //    case 2:
                    //        {
                    //            points = rec.IdentifyChar();
                    //            index = rec.WhatIsItIndex();
                    //            break;
                    //        }
                    //    case 3:
                    //        {
                    //            index = rec.WhatIsItWithKey();
                    //            break;
                    //        }
                    //    case 4:
                    //        {
                    //            index = rec.WhatIsItWithIdealKey();
                    //            break;
                    //        }
                    //}
                    //    if (index == -1)
                    //    {
                    //        error++;
                    //    }
                    //    else
                    //    {
                    //        if (index == i)
                    //        {
                    //            truePositive[i]++;
                    //        }
                    //        else
                    //        {
                    //            falseNegative[i]++;
                    //            falsePositive[index]++;
                    //        }
                    //    }
                    //}
                    //Console.WriteLine(i);
                }
                Console.WriteLine(i);
            }

            using (var strWr = new StreamWriter(@"resultsNew.txt"))
            {
                for (int i = 0; i < 9; i++)
                {
                    strWr.WriteLine($"{PGC.IdealGestures[i].Name} TruePositive  = {truePositive[i]} False Negative = {falseNegative[i]} False Positive = {falsePositive[i]}");
                }
                var t = truePositive.Sum();
                strWr.WriteLine($"All {t}");
                strWr.WriteLine($"Error {error}");
            }
        }
    }
}
