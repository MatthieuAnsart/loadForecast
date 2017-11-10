// --------------------------------------------------------------- -*- C# -*-
// File: Iterators.cs
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
// C# version of iterators.cpp of OPL distrib
//--------------------------------------------------------------------------

using System;
using System.Collections;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;

namespace Iterators
{
    class Iterators
    {
        // The purpose of this sample is to check the result of filtering by iterating on the generated data element.
        //
        // The data element is an array of strings that is indexed by a set of strings.
        // It is filled as the result of an iteration on a set of tuples that filters out the duplicates.
        // It is based on the model used in "Sparsity" run configuration of the "transp" example.
        //
        //
        // The simplified model is:
        //
        // {string} Products = ...;
        // tuple Route { string p; string o; string d; }
        // {Route} Routes = ...;
        // {string} orig[p in Products] = { o | <p,o,d> in Routes };
        //
        static int sample1()
        {
            int status = 127;
            const string DATADIR = "../..";
            OplFactory.DebugMode = true;
            OplFactory oplF = new OplFactory();
            try
            {

                OplErrorHandler errHandler = oplF.CreateOplErrorHandler(Console.Out);
                OplRunConfiguration rc = oplF.CreateOplRunConfiguration(DATADIR + "/transp2.mod", DATADIR + "/transp2.dat");
                OplModel opl = rc.GetOplModel();
                opl.Generate();
                Console.Out.WriteLine("Verification of the computation of orig: ");

                // Get the orig, Routes, Product elements from the OplModel.
                ISymbolSetMap orig = opl.GetElement("Orig").AsSymbolSetMap();
                ITupleSet Routes = opl.GetElement("Routes").AsTupleSet();
                ISymbolSet Products = opl.GetElement("Products").AsSymbolSet();

                Console.Out.Write("Products = ");
                for (int j = 0; j <= Products.Size - 1; j++)
                {
                    Console.Out.Write(Products.GetValue(j) + " ");
                }
                Console.Out.WriteLine();

                // Iterate through the orig to see the result of the data element filtering.
                IEnumerator it1 = Products.GetEnumerator();
                while (it1.MoveNext())
                {
                    string p = ((string)it1.Current);
                    // This is the last dimension of the array (as it is a one-dimensional array), so you can use the get method directly.
                    Console.Out.WriteLine("for p = " + p + " we have " + orig.Get(p));
                }
                Console.Out.WriteLine("---------------------");

                // Iterate through the TupleSet.
                IEnumerator it2 = Routes.GetEnumerator();
                while (it2.MoveNext())
                {
                    ITuple t = ((ITuple)it2.Current);
                    // Get the string "p" from the tuple.
                    string p = t.GetStringValue("p");
                    // if "p" is in the indexer, we will try to add the "o" string to the array.
                    if (Products.Contains(p))
                    {
                        Console.Out.WriteLine("for p = " + p + " we will have " + t.GetStringValue("o") + " from " + t);
                    }
                }
                Console.Out.WriteLine("---------------------");
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

            oplF.End();
            return status;
        }

        // The purpose of this sample is to output a multidimensional array x[i][j] to illustrate how arrays and subarrays are managed.
        // To access the elements of an array, you must first access the subarrays up to  the last dimension, then you can get the values.
        //  Here, as there are two dimensions, you have to get one subarray from which you can directly get the values.
        //
        // The array of integers is indexed by two sets of strings..
        //
        // The simplified model is:
        //
        // {string} s1 = ...;
        // {string} s2 = ...;
        // {int} x[s1][s2] = ...;
        //
        static int sample2()
        {
            int status = 127;
            const string DATADIR = "../..";
            OplFactory.DebugMode = true;
            OplFactory oplF = new OplFactory();
            try
            {
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler(Console.Out);
                OplRunConfiguration rc = oplF.CreateOplRunConfiguration(DATADIR + "/iterators.mod");
                OplModel opl = rc.GetOplModel();
                opl.Generate();

                // Get the x, s1 and s2 elements from the OplModel.
                IIntMap x = opl.GetElement("x").AsIntMap();
                ISymbolSet s1 = opl.GetElement("s1").AsSymbolSet();
                ISymbolSet s2 = opl.GetElement("s2").AsSymbolSet();

                // Iterate on the first indexer.
                IEnumerator it1 = s1.GetEnumerator();
                while (it1.MoveNext())
                {
                    // Get the second dimension array from the first dimension.
                    IIntMap sub = x.GetSub((string)it1.Current);
                    // Iterate on the second indexer of x (that is the indexer of the subarray).
                    IEnumerator it2 = s2.GetEnumerator();
                    while (it2.MoveNext())
                    {
                        // This is the last dimension of the array, so you can directly use the get method.
                        Console.Out.WriteLine(it1.Current + " " + it2.Current + " " + sub.Get((string)it2.Current));
                    }
                }
                Console.Out.WriteLine("---------------------");
                status = 0;
            }
	    catch (ILOG.OPL.OplException ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;
            }
            catch (IloException ex)
            {
                Console.Out.WriteLine("### exception: " + ex.Message);
                status = 2;
            }
            catch (System.Exception ex)
            {
                Console.Out.WriteLine("### UNEXPECTED ERROR ..." + ex.Message);
                status = 3;
            }

            oplF.End();
            return status;
        }

        // The purpose of this sample is to output an array of tuples arrayT[i], 
        // to illustrate how tuple elements can be accessed.
        // The simplified model is:
        // tuple t
        // {
        //   int a;
        //   int b;
        // }
        // {string} ids={"id1","id2","id3"};
        // t arrayT[ids]=[<1,2>,<2,3>,<1,3>];

        static String getModelTextSample3()
        {
            String model = "";
            model += "tuple t{int a;int b;}";
            model += "{string} ids = {\"id1\",\"id2\", \"id3\"};";
            model += "t arrayT[ids] = [<1,2>,<2,3>,<1,3>];";
            return model;
        }
        static int sample3()
        {
            int status = 0;
            OplFactory.DebugMode = true;
            OplFactory oplF = new OplFactory();
            try
            {
                OplErrorHandler errHandler = oplF.CreateOplErrorHandler(Console.Out);
                OplSettings settings = oplF.CreateOplSettings(errHandler);
                OplModelSource src = oplF.CreateOplModelSourceFromString(getModelTextSample3(), "tuple array iterator");
                OplModelDefinition def = oplF.CreateOplModelDefinition(src, settings);
                Cplex cplex = oplF.CreateCplex();
                OplModel opl = oplF.CreateOplModel(def, cplex);
                opl.Generate();

                // get the string set used to index the array of tuples
                ITupleMap arrayT = opl.GetElement("arrayT").AsTupleMap();
                ISymbolSet ids = opl.GetElement("ids").AsSymbolSet();
                // iterate on the index set to retrieve the tuples stored in the array
                IEnumerator it = ids.GetEnumerator();
                while (it.MoveNext())
                {
                    Console.Out.Write("arrayT[" + it.Current + "] = ");
                    IMapIndexArray id = oplF.MapIndexArray(0);
                    id.Add(it.Current.ToString());
                    ITuple t = arrayT.MakeTuple();
                    arrayT.GetAt(id, t);
                    Console.Out.WriteLine(t);
                }
            }
	    catch (ILOG.OPL.OplException ex)
            {
                Console.WriteLine(ex.Message);
                status = 1;
            }
            catch (IloException e)
            {
                status = 2;
                Console.Out.WriteLine("### exception: " + e.Message);
            }
            catch (System.Exception ex)
            {
                status = 3;
                Console.Out.WriteLine("### UNEXPECTED ERROR ..." + ex.Message);
            }
            oplF.End();
            return status;
        }


        static int Main(string[] args)
        {
            int res1 = sample1();
            int res2 = sample2();
            int res3 = sample3();

            Console.WriteLine("--Press <Enter> to exit--");
            Console.ReadLine();

            return (res1 + res2 + res3);
        }
    }
}
