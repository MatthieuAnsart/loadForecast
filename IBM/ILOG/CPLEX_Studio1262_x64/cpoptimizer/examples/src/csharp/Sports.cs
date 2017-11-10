// -----------------------------------------------------------------*- C# -*-
// File: ./examples/src/csharp/Sports.cs
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5725-A06 5725-A29
// Copyright IBM Corporation 1990, 2014. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// -------------------------------------------------------------------------

/* ------------------------------------------------------------

Problem Description
-------------------

The problem involves finding a schedule for a sports league. The league has 10 
teams that play games over a season of 18 weeks. Each team has a home arena and 
plays each other team twice during the season, once in its home arena and once in 
the opposing team's home arena. For each of these games, the team playing at its 
home arena is referred to as the home team; the team playing at the opponent's 
arena is called the away team. There are 90 games altogether.

Each of the 18 weeks in the season has five identical slots to which games can be 
assigned. Each team plays once a week. For each pair of teams, these two teams are 
opponents twice in a season; these two games must be scheduled in different halves 
of the season. Moreover, these two games must be scheduled at least six weeks 
apart. A team must play at home either the first or last week but not both.

A break is a sequence of consecutive weeks in which a team plays its games either 
all at home or all away. No team can have a break of three or more weeks in it. The
objective in this problem is to minimize the total number of breaks the teams play. 

------------------------------------------------------------ */

using System;
using System.IO;
using ILOG.CP;
using ILOG.Concert;

namespace Sports
{
  class Program
  {
    public static int Game(int h, int a, int n)
    {
      if (a > h)
        return h * (n - 1) + a - 1;
      else
        return h * (n - 1) + a;
    }
    public static int min(int a, int b)
    {
      return (a >= b ? b : a);
    }

    static void Main(string[] args)
    {
      try
      {
        int n = 10;
        if (args.Length > 0) 
          n = Int32.Parse(args[0]);
        if ((n % 2) == 1)
          n++;
        Console.WriteLine("Finding schedule for {0} teams", n);
        int nbWeeks = 2 * (n - 1);
        int nbGamesPerWeek = n / 2;
        int nbGames = n * (n - 1);
        CP cp = new CP();

        IIntVar[][] games = new IIntVar[nbWeeks][];
        IIntVar[][] home = new IIntVar[nbWeeks][];
        IIntVar[][] away = new IIntVar[nbWeeks][];

        for (int i = 0; i < nbWeeks; i++)
        {
          home[i] = cp.IntVarArray(nbGamesPerWeek, 0, n - 1);
          away[i] = cp.IntVarArray(nbGamesPerWeek, 0, n - 1);
          games[i] = cp.IntVarArray(nbGamesPerWeek, 0, nbGames - 1);
        }
        //
        // For each play slot, set up correspondance between game id,
        // home team, and away team
        // 

        IIntTupleSet gha = cp.IntTable(3);
        int[] tuple = new int[3];
        for (int i = 0; i < n; i++)
        {
          tuple[0] = i;
          for (int j = 0; j < n; j++)
          {
            if (i != j)
            {
              tuple[1] = j;
              tuple[2] = Game(i, j, n);
              cp.AddTuple(gha, tuple);
            }
          }
        }

        for (int i = 0; i < nbWeeks; i++)
        {
          for (int j = 0; j < nbGamesPerWeek; j++)
          {
            IIntVar[] vars = cp.IntVarArray(3);
            vars[0] = home[i][j];
            vars[1] = away[i][j];
            vars[2] = games[i][j];
            cp.Add(cp.AllowedAssignments(vars, gha));
          }
        }
        //
        // All teams play each week
        //
        for (int i = 0; i < nbWeeks; i++)
        {
          IIntVar[] teamsThisWeek = cp.IntVarArray(n);
          for (int j = 0; j < nbGamesPerWeek; j++)
          {
            teamsThisWeek[j] = home[i][j];
            teamsThisWeek[nbGamesPerWeek + j] = away[i][j];
          }
          cp.Add(cp.AllDiff(teamsThisWeek));
        }
        //
        // Dual representation: for each game id, the play slot is maintained
        // 
        IIntVar[] weekOfGame = cp.IntVarArray(nbGames, 0, nbWeeks - 1);
        IIntVar[] allGames = cp.IntVarArray(nbGames);
        IIntVar[] allSlots = cp.IntVarArray(nbGames, 0, nbGames - 1);
        for (int i = 0; i < nbWeeks; i++)
          for (int j = 0; j < nbGamesPerWeek; j++)
            allGames[i * nbGamesPerWeek + j] = games[i][j];
        cp.Add(cp.Inverse(allGames, allSlots));
        for (int i = 0; i < nbGames; i++)
          cp.Add(cp.Eq(weekOfGame[i], cp.Div(allSlots[i], nbGamesPerWeek)));
        //
        // Two half schedules.  Cannot play the same pair twice in the same half.
        // Plus, impose a minimum number of weeks between two games involving
        // the same teams (up to six weeks)
        //
        int mid = nbWeeks / 2;
        int overlap = 0;
        if (n >= 6)
          overlap = min(n / 2, 6);
        for (int i = 0; i < n; i++)
        {
          for (int j = i + 1; j < n; j++)
          {
            int g1 = Game(i, j, n);
            int g2 = Game(j, i, n);
            cp.Add(cp.Equiv(cp.Ge(weekOfGame[g1], mid), cp.Lt(weekOfGame[g2], mid)));
            // Six week difference...
            if (overlap != 0)
              cp.Add(cp.Ge(cp.Abs(cp.Diff(weekOfGame[g1], weekOfGame[g2])), overlap));
          }
        }

        //
        // Can't have three homes or three aways in a row.
        //
        IIntVar[][] playHome = new IIntVar[n][];
        for (int i = 0; i < n; i++)
        {
          playHome[i] = cp.IntVarArray(nbWeeks, 0, 1);
          for (int j = 0; j < nbWeeks; j++)
            cp.Add(cp.Eq(playHome[i][j], cp.Count(home[j], i)));
          for (int j = 0; j < nbWeeks - 3; j++)
          {
            IIntVar[] window = cp.IntVarArray(3);
            for (int k = j; k < j + 3; k++)
              window[k - j] = playHome[i][k];
            IIntExpr windowSum = cp.Sum(window);
            cp.Add(cp.Le(1, windowSum));
            cp.Add(cp.Le(windowSum, 2));
          }
        }

        //
        // If we start the season home, we finish away and vice versa.
        //
        for (int i = 0; i < n; i++)
          cp.Add(cp.Neq(playHome[i][0], playHome[i][nbWeeks - 1]));

        //
        // Objective: minimize the number of `breaks'.  A break is
        //            two consecutive home or away matches for a
        //            particular team
        IIntVar[] teamBreaks = cp.IntVarArray(n, 0, nbWeeks / 2);
        for (int i = 0; i < n; i++)
        {
          IIntExpr nbreaks = cp.Constant(0);
          for (int j = 1; j < nbWeeks; j++)
            nbreaks = cp.Sum(nbreaks, cp.IntExpr(cp.Eq(playHome[i][j - 1], playHome[i][j])));
          cp.Add(cp.Eq(teamBreaks[i], nbreaks));
        }
        IIntVar breaks = cp.IntVar(n - 2, n * (nbWeeks / 2));
        cp.Add(cp.Eq(breaks, cp.Sum(teamBreaks)));
        cp.Add(cp.Minimize(breaks));

        //
        // Catalyzing constraints
        //

        // Each team plays home the same number of times as away
        for (int i = 0; i < n; i++)
          cp.Add(cp.Eq(cp.Sum(playHome[i]), nbWeeks / 2));

        // Breaks must be even for each team
        for (int i = 0; i < n; i++)
          cp.Add(cp.Eq(cp.Modulo(teamBreaks[i], 2), 0));

        //    
        // Symmetry breaking constraints
        // 

        // Teams are interchangeable.  Fix first week.
        // Also breaks reflection symmetry of the whole schedule.
        for (int i = 0; i < nbGamesPerWeek; i++)
        {
          cp.Add(cp.Eq(home[0][i], i * 2));
          cp.Add(cp.Eq(away[0][i], i * 2 + 1));
        }

        // Order of games in each week is arbitrary.
        // Break symmetry by forcing an order.
        for (int i = 0; i < nbWeeks; i++)
          for (int j = 1; j < nbGamesPerWeek; j++)
            cp.Add(cp.Gt(games[i][j], games[i][j - 1]));

        cp.SetParameter(CP.DoubleParam.TimeLimit, 20);
        cp.SetParameter(CP.IntParam.LogPeriod, 10000);
        IVarSelector varSel = cp.SelectSmallest(cp.VarIndex(allGames)); ;
        IValueSelector valSel = cp.SelectRandomValue();

        ISearchPhase phase = cp.SearchPhase(allGames,
                                            cp.IntVarChooser(varSel),
                                            cp.IntValueChooser(valSel));
        cp.StartNewSearch(phase);
        while (cp.Next())
        {
          Console.WriteLine("Solution at {0}", cp.GetValue(breaks));
        } 
        cp.EndSearch();
      }
      catch (ILOG.Concert.Exception e)
      {
       Console.WriteLine("Error {0}", e);
      }
    }
  }
}
