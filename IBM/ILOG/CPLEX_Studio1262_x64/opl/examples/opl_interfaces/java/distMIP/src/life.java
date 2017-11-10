/*
* Licensed Materials - Property of IBM
* 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 
* Copyright IBM Corporation 1998, 2014. All Rights Reserved.
*
* Note to U.S. Government Users Restricted Rights:
* Use, duplication or disclosure restricted by GSA ADP Schedule
* Contract with IBM Corp.
*/ 

// -------------------------------------------------------------- -*- Java -*-

import java.io.IOException;

import ilog.concert.*;
import ilog.cp.cppimpl.IloWrappedPropagatorI;
import ilog.cplex.IloCplex;
import ilog.opl.IloOplElement;
import ilog.opl.IloOplErrorHandler;
import ilog.opl.IloOplException;
import ilog.opl.IloOplFactory;
import ilog.opl.IloOplModel;
import ilog.opl.IloOplModelDefinition;
import ilog.opl.IloOplModelSource;
import ilog.opl.IloOplSettings;

public class life {

	/**
	 * @param args
	 * @throws IloException
	 */
	public static void main(String[] args) {
		String DATADIR = ".";
		int status = 127;
		try {
			IloOplFactory.setDebugMode(true);
			IloOplFactory oplF = new IloOplFactory();
			IloOplErrorHandler errHandler = oplF.createOplErrorHandler();
			IloOplModelSource modelSource = oplF.createOplModelSource(DATADIR+"/lifegameip.mod");
			IloOplSettings settings = oplF.createOplSettings(errHandler);
			IloOplModelDefinition def = oplF.createOplModelDefinition(
					modelSource, settings);
			IloCplex cplex = oplF.createCplex();
			cplex.readVMConfig(DATADIR+"/process.vmc");
			
			/*** Example of possible parameters 
			cplex.setParam( IloCplex.IntParam.RampupDuration, IloCplex.DistMIPRampupDuration.RampupDisabled );
			cplex.setParam( IloCplex.DoubleParam.RampupDettimeLim, 5 );
			cplex.setParam( IloCplex.DoubleParam.RampupWalltimeLim, 2 );
			***/
			
			IloOplModel opl = oplF.createOplModel(def, cplex);
			opl.generate();
			cplex.solve();
			System.out.println("OBJECTIVE: " + opl.getCplex().getObjValue());
			opl.postProcess();
			opl.printSolution(System.out);
			if (cplex.hasVMConfig())
				System.out.println("cplex has a VM file");
			else
				throw new IloException("cplex does not have a VM file");

			cplex.delVMConfig();
			if (cplex.hasVMConfig())
				throw new IloException("cplex has a VM file");
			else
				System.out.println("cplex does not have a VM file anymore");
			oplF.end();
			status = 0;
		} catch (IloOplException ex) {
			System.err.println("### OPL exception: " + ex.getMessage());
			ex.printStackTrace();
			status = 2;
		} catch (IloException ex) {
			System.err.println("### CONCERT exception: " + ex.getMessage());
			ex.printStackTrace();
			status = 3;
		} catch (Exception ex) {
			System.err.println("### UNEXPECTED UNKNOWN ERROR ...");
			ex.printStackTrace();
			status = 4;
		}
		System.exit(status);
	}

}
