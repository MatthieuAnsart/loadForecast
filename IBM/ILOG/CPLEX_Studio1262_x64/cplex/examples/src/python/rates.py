#!/usr/bin/python
# ---------------------------------------------------------------------------
# File: rates.py
# Version 12.6.2
# ---------------------------------------------------------------------------
# Licensed Materials - Property of IBM
# 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5655-Y21
# Copyright IBM Corporation 2009, 2015. All Rights Reserved.
#
# US Government Users Restricted Rights - Use, duplication or
# disclosure restricted by GSA ADP Schedule Contract with
# IBM Corp.
# ---------------------------------------------------------------------------
#
# rates.py -- modeling with semi-continuous variables
#
# Problem Description:
#
# Assume you run a power supply company.  You have several power generators
# available, each of which has a minimum and maximum production level and a
# cost per unit output.  The question is which generators to use in order to
# minimize the overall operation cost while satisfying the demand.
#
# To run this example from the command line, use
#
#    python rates.py

from __future__ import print_function

import sys

import cplex
from cplex.exceptions import CplexSolverError
from inputdata import read_dat_file


def rates(datafile):
    # Read in data file. If no file name is given on the command line
    # we use a default file name. The data we read is
    # min    -- a list/array of minimal power production of generators.
    # max    -- a list/array of maximal power production of generators.
    # cost   -- a list/array of cost for using a generator
    # demand -- the total power demand.
    # The arrays are all of equal length and each entry corresponds to
    # a generator.
    min, max, cost, demand = read_dat_file(datafile)
    generators = list(range(len(min)))  # List of generators

    # Create a new (empty) model and populate it below.
    model = cplex.Cplex()

    # Create one variable for each generator. The variables model the amount
    # of power generated by the respective generator. They are of type
    # semi-continuous because a generator g produces at least min[g] if it
    # is switched on.
    model.variables.add(obj=cost, lb=min, ub=max)
    model.variables.set_types(zip(generators,
                                  len(generators) *
                                  [model.variables.type.semi_continuous]))

    # Require that the sum of the production of all generators
    # satisfies the demand.
    totalproduction = cplex.SparsePair(ind=generators,
                                       val=[1.0] * len(generators))
    model.linear_constraints.add(lin_expr=[totalproduction],
                                 senses=["G"],
                                 rhs=[demand])

    # Our objective is to minimize cost. Cost per unit of power generated
    # was already set when variables were created.
    model.objective.set_sense(model.objective.sense.minimize)

    # Eport the model to disk and solve.
    try:
        model.write("rates.lp")
        model.solve()
    except CplexSolverError as e:
        print("Exception raised during solve: " + e)
    else:
        # Solve succesful, dump solution.
        print("Solution status = ", model.solution.get_status())
        for g in generators:
            print("  generator " + str(g) + ": " +
                  str(model.solution.get_values(g)))
        print("Total cost = " + str(model.solution.get_objective_value()))

if __name__ == "__main__":
    datafile = "../../../examples/data/rates.dat"
    if len(sys.argv) < 2:
        print("Default data file : " + datafile)
    else:
        datafile = sys.argv[1]
    rates(datafile)
