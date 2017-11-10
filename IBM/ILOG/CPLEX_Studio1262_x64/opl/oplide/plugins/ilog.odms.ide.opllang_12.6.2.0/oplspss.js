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

var spssHome = IloOplGetEnv("OPL_SPSS_HOME");
if ( !spssHome ) {
	var spssHomeDefault = "C:\\Program Files\\IBM\\SPSS\\Modeler\\14";
	if ( new IloOplInputFile(spssHomeDefault).exists ) {
		spssHome = spssHomeDefault;
	} else {
		fail("Environment variable 'OPL_SPSS_HOME' not set.")
	}
}
if ( !new IloOplInputFile(spssHome+"/lib/batch.jar").exists ) {
	fail("No valid SPSS Modeler installation at 'OPL_SPSS_HOME'",spssHome)
}

IloOplImportJava("oplspss.jar");

var path = thisOplModel.settings.resolverPath 
if ( path.indexOf(spssHome)==-1 ) {
	thisOplModel.settings.resolverPath = path + ";" + spssHome+"/lib";
}
IloOplImportJava("batch.jar");
IloOplImportJava("shared_resources.jar");
IloOplImportJava("serializer.jar");
IloOplImportJava("xalan.jar");
IloOplImportJava("repository-client-application.jar");
IloOplImportJava("repository-client.jar");
IloOplImportJava("pasw-cf-common.jar");
IloOplImportJava("commons-logging.jar");
IloOplImportJava("log4j.jar");

IloOplCallJava("ilog.opl.spss.IloSPSSDataHandler","register","",thisOplModel,spssHome);

// for debugging, use with care
function SetVerboseSPSS() {
	IloOplCallJava("ilog.opl.spss.IloSPSSDataHandler","setVerbose","",thisOplModel);	
}

