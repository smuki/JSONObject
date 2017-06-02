namespace TestSuite
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Data;
    using System.Web.UI;
    using System.Text;
    using System.Xml;

    using Volte.Data.Json;

    class TestApp
    {
        static JSONTable oOWXF = new JSONTable();
        static string str;

        static void Test1()
        {
            //load from ToString
            GC.Collect();
            int gc0 = GC.CollectionCount (0);
            int gc1 = GC.CollectionCount (1);
            int gc2 = GC.CollectionCount (2);

            StreamReader objReader = new StreamReader ("JSONObject1.js");

            str = objReader.ReadToEnd();

           JSONObject _JSONObject = new JSONObject(str);

            StreamWriter swer31 = new StreamWriter ("JSONObject2.js", false);
            swer31.Write(_JSONObject.ToString());
            swer31.Flush();
            swer31.Close();


            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            oOWXF = new JSONTable();
            oOWXF.Parser (str);
            oOWXF.Open();

            Console.WriteLine("Rows  :"+oOWXF.RecordCount);
            Console.WriteLine("Fields:"+oOWXF.Fields.Count);

            StreamWriter swer3 = new StreamWriter ("vdata1.js", false);
            swer3.Write(oOWXF.ToString());
            swer3.Flush();
            swer3.Close();

            Console.WriteLine ("");
            Console.WriteLine ("LoadJson: " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine ("GC 0:" + (GC.CollectionCount (0) - gc0).ToString());
            Console.WriteLine ("GC 1:" + (GC.CollectionCount (1) - gc1).ToString());
            Console.WriteLine ("GC 2:" + (GC.CollectionCount (2) - gc2).ToString());

        }

        static void Test2()
        {
            //write to json
            GC.Collect();
            int  gc0 = GC.CollectionCount (0);
            int gc1 = GC.CollectionCount (1);
            int gc2 = GC.CollectionCount (2);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            str = oOWXF.ToString();

            Console.WriteLine ("");
            Console.WriteLine ("write to ToString: " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine ("GC 0:" + (GC.CollectionCount (0) - gc0).ToString());
            Console.WriteLine ("GC 1:" + (GC.CollectionCount (1) - gc1).ToString());
            Console.WriteLine ("GC 2:" + (GC.CollectionCount (2) - gc2).ToString());

            StreamWriter swer2 = new StreamWriter ("vdat2.js", false);
            swer2.Write (str);
            swer2.Flush();
            swer2.Close();
        }

        static void Test3()
        {
            //write to ToString
            GC.Collect();
            int  gc0 = GC.CollectionCount (0);
            int gc1 = GC.CollectionCount (1);
            int gc2 = GC.CollectionCount (2);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            str = oOWXF.ToString();

            Console.WriteLine ("");
            Console.WriteLine ("write to Xml: " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine ("GC 0:" + (GC.CollectionCount (0) - gc0).ToString());
            Console.WriteLine ("GC 1:" + (GC.CollectionCount (1) - gc1).ToString());
            Console.WriteLine ("GC 2:" + (GC.CollectionCount (2) - gc2).ToString());


        }

        static void Test4()
        {
            //write to ToString
            GC.Collect();
            int  gc0 = GC.CollectionCount (0);
            int gc1 = GC.CollectionCount (1);
            int gc2 = GC.CollectionCount (2);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            JSONTable oOWXFxml = new JSONTable();
            //oOWXFxml.LoadXMLString(str);
            //oOWXFxml.Open();

            Console.WriteLine ("");
            Console.WriteLine ("write to Xml: " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine ("GC 0:" + (GC.CollectionCount (0) - gc0).ToString());
            Console.WriteLine ("GC 1:" + (GC.CollectionCount (1) - gc1).ToString());
            Console.WriteLine ("GC 2:" + (GC.CollectionCount (2) - gc2).ToString());

        }

        static void Test5()
        {
            //write to ToString
            GC.Collect();
            int  gc0 = GC.CollectionCount (0);
            int gc1 = GC.CollectionCount (1);
            int gc2 = GC.CollectionCount (2);

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            JSONTable oOWXF3 = new JSONTable();

            foreach (AttributeMapping _AttributeMapping in oOWXF.Fields) {
                oOWXF3.Declare (_AttributeMapping);
            }

            foreach (string cName in oOWXF.Variable.Names) {

                oOWXF3.Variable[cName] = oOWXF.Variable[cName];
            }

            while (!oOWXF.EOF) {
                oOWXF3.AddNew();

                for (int i = 0; i < oOWXF.Fields.Count; i++) {
                    oOWXF3[i] = oOWXF[i];
                }

                oOWXF3.Update();
                oOWXF.MoveNext();
            }

            oOWXF.Close();

            Console.WriteLine("Rows  :"+oOWXF.RecordCount);
            Console.WriteLine("Fields:"+oOWXF.Fields.Count);

            Console.WriteLine ("");
            Console.WriteLine ("Copy To another: " + stopwatch.ElapsedMilliseconds + "ms");
            Console.WriteLine ("GC 0:" + (GC.CollectionCount (0) - gc0).ToString());
            Console.WriteLine ("GC 1:" + (GC.CollectionCount (1) - gc1).ToString());
            Console.WriteLine ("GC 2:" + (GC.CollectionCount (2) - gc2).ToString());


            Console.WriteLine ("Write to vdata3");

            StreamWriter swer3 = new StreamWriter ("vdat3.js", false);
            swer3.Write(oOWXF3.ToString());
            swer3.Flush();
            swer3.Close();

        }

        static void Main()
        {


            Test1();
            Test2();
            Test3();
            //Test4();
            Test5();

        }
    }
}
