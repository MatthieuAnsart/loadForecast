// --------------------------------------------------------------- -*- C# -*-
// File: CustomDataSource.cs
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
// 
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 
// Copyright IBM Corporation 1998, 2013. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// --------------------------------------------------------------------------

//-------------------------------------------------------------- -*- C# -*-
// C# version of customdatasource.cpp of OPL distrib
//--------------------------------------------------------------------------
using System;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;

namespace CustomDataSource
{
    class Warehouse
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            int status = 127;
            const string DATADIR = "../..";
            try
            {
                OplFactory.DebugMode = true;
                OplFactory oplF = new OplFactory();
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler(Console.Out);
                OplModelSource modelSource = oplF.CreateOplModelSource(DATADIR + "/customDataSource.mod");
                OplSettings settings = oplF.CreateOplSettings(errHandler);
                OplModelDefinition def = oplF.CreateOplModelDefinition(modelSource, settings);
                Cplex cplex = oplF.CreateCplex();
                OplModel opl = oplF.CreateOplModel(def, cplex);
                OplDataSource dataSource = new MyCustomDataSource(oplF);
                opl.AddDataSource(dataSource);
                opl.Generate();
                oplF.End();
                status = 0;
            }
            catch (ILOG.OPL.OplException ex)
            {
                Console.WriteLine(ex.Message);
                status = 2;
            }
            catch (ILOG.Concert.Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 3;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = 4;
            }

            Environment.ExitCode = status;

            Console.WriteLine("--Press <Enter> to exit--");
            Console.ReadLine();
        }


        internal class MyCustomDataSource : CustomOplDataSource
        {
            internal MyCustomDataSource(OplFactory oplF)
                : base(oplF)
            {
            }
            public override void CustomRead()
            {
                OplDataHandler handler = DataHandler;

                // initialize the int 'simpleInt'
                handler.StartElement("anInt");
                handler.AddIntItem(3);
                handler.EndElement();

                // initialize the int array 'simpleIntArray'
                handler.StartElement("anIntArray");
                handler.StartArray();
                handler.AddIntItem(1);
                handler.AddIntItem(2);
                handler.AddIntItem(3);
                handler.EndArray();
                handler.EndElement();

                // initialize int array indexed by float 'anArrayIndexedByFloat'
                handler.StartElement("anArrayIndexedByFloat");
                handler.StartIndexedArray();
                handler.SetItemNumIndex(2.0);
                handler.AddIntItem(1);
                handler.SetItemNumIndex(2.5);
                handler.AddIntItem(2);
                handler.SetItemNumIndex(1.0);
                handler.AddIntItem(3);
                handler.SetItemNumIndex(1.5);
                handler.AddIntItem(4);
                handler.EndIndexedArray();
                handler.EndElement();

                // initialize int array indexed by string 'anArrayIndexedByString'
                handler.StartElement("anArrayIndexedByString");
                handler.StartIndexedArray();
                handler.SetItemStringIndex("idx1");
                handler.AddIntItem(1);
                handler.SetItemStringIndex("idx2");
                handler.AddIntItem(2);
                handler.EndIndexedArray();
                handler.EndElement();

                // initialize a tuple in the order the components are declared
                handler.StartElement("aTuple");
                handler.StartTuple();
                handler.AddIntItem(1);
                handler.AddNumItem(2.3);
                handler.AddStringItem("not named tuple");
                handler.EndTuple();
                handler.EndElement();

                // initialize a tuple using tuple component names.
                handler.StartElement("aNamedTuple");
                handler.StartNamedTuple();
                handler.SetItemName("f");
                handler.AddNumItem(3.45);
                handler.SetItemName("s");
                handler.AddStringItem("named tuple");
                handler.SetItemName("i");
                handler.AddIntItem(99);
                handler.EndNamedTuple();
                handler.EndElement();

                // initialize the tuple set 'simpleTupleSet'
                handler.StartElement("aTupleSet");
                handler.StartSet();
                // first tuple
                handler.StartTuple();
                handler.AddIntItem(1);
                handler.AddNumItem(2.5);
                handler.AddStringItem("a");
                handler.EndTuple();
                // second element
                handler.StartTuple();
                handler.AddIntItem(3);
                handler.AddNumItem(4.1);
                handler.AddStringItem("b");
                handler.EndTuple();
                handler.EndSet();
                handler.EndElement();

                // initialize element 3 and 2 of the tuple array 'simpleTupleArray' in that particular order
                handler.StartElement("aTupleArray");
                handler.StartIndexedArray();
                // initialize 3rd cell
                handler.SetItemIntIndex(3);
                handler.StartTuple();
                handler.AddIntItem(1);
                handler.AddNumItem(2.5);
                handler.AddStringItem("a");
                handler.EndTuple();
                // initialize second cell
                handler.SetItemIntIndex(2);
                handler.StartTuple();
                handler.AddIntItem(3);
                handler.AddNumItem(4.1);
                handler.AddStringItem("b");
                handler.EndTuple();
                handler.EndIndexedArray();
                handler.EndElement();

                // initialize int array indexed by tuple set 'anArrayIndexedByTuple'
                handler.StartElement("anArrayIndexedByTuple");
                handler.StartIndexedArray();
                handler.StartItemTupleIndex();
                handler.AddIntItem(3);
                handler.AddNumItem(4.1);
                handler.AddStringItem("b");
                handler.EndItemTupleIndex();
                handler.AddIntItem(1);
                handler.EndIndexedArray();
                handler.EndElement();

                // initialize a 2-dimension int array 'a2DIntArray'
                handler.StartElement("a2DIntArray");
                handler.StartArray();
                for (int i = 1; i <= 2; i++)
                {
                    handler.StartArray();
                    for (int j = 1; j <= 3; j++)
                        handler.AddIntItem(i * 10 + j);
                    handler.EndArray();
                }
                handler.EndArray();
                handler.EndElement();

            }
        }
    }
}

