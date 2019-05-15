using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recognition;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class AlgorithmTests
    {
        [TestMethod]
        public void HungarianTest()
        {
            var list1 = new List<double>() { 7, 3, 6, 9, 5 };
            var list2 = new List<double>() { 7, 5, 7, 5, 6 };
            var list3 = new List<double>() { 7, 6, 8, 8, 9 };
            var list4 = new List<double>() { 3, 1, 6, 5, 7 };
            var list5 = new List<double>() { 2, 4, 9, 9, 5 };
            var list = new List<List<double>>() {list1, list2,list3,list4,list5 };
            var temp = new Recognition.Recognizers.Algorithms.Hungarian(list);
            var res = temp.Execute().ToList();
            for (int i = 0; i < 5; i++)
            {
                var t = res.Find(x => x[0] == i);
                switch (i)
                {
                    case 0:
                        Assert.AreEqual(4, t[1]);
                        break;
                    case 1:
                        Assert.AreEqual(3, t[1]);
                        break;
                    case 2:
                        Assert.AreEqual(2, t[1]);
                        break;
                    case 3:
                        Assert.AreEqual(1, t[1]);
                        break;
                    case 4:
                        Assert.AreEqual(0, t[1]);
                        break;
                }
            }       
        }
    }
}
