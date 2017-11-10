// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilopl/ilodatasource.h
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 
// Copyright IBM Corp. 2000, 2013
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// ---------------------------------------------------------------------------

#ifndef __ADVANCED_ilodatasourceH
#define __ADVANCED_ilodatasourceH

#ifdef _WIN32
#pragma pack(push, 8)
#endif

#include <ilopl/ilosys.h>
#include <ilopl/ilsource/ilodatasourcei.h>



#ifdef ILO_MSVC
/////////////////


class IloExcelDataSource {
private:
	IloExcelDataSourceI* _impl;
public:
	
	IloExcelDataSource(IloEnv env, const char* fileName, IloBool readBolny = IloFalse);
	
	IloExcelDataSource(): _impl(0) {}

	IloExcelDataSourceI* getImpl() const { return _impl; }
	
	IloExcelDataSource(IloExcelDataSourceI* impl): _impl(impl) {}


	
	void end(){
		if (_impl){
			delete _impl;
			_impl = 0;
		}
	}

	
	IloInt getIntFromTable(const char* range, IloInt x, IloInt y);

	
	IloNum getNumFromTable(const char* range, IloInt x, IloInt y);
	
	IloSymbol getSymbolFromTable(const char* range, IloInt x, IloInt y);

	
    void readIntMap(IloMapI* result, const char* range);
    
    void readNumMap(IloMapI* result, const char* range);
    
    void readSymbolMap(IloMapI* result, const char* range);
    
    void readTupleMap(IloAbstractTupleMapI* result, const char* range);

    
    void readIntSet(IloIntSetI* result, const char* range);

    
    void readNumSet(IloNumSetI* result, const char* range);

    
    void readSymbolSet(IloAnySetI* result, const char* range);

    
    void readTupleSet(IloTupleSetI* result, const char* range);


    
    void writeIntSet(IloIntSetI* set, const char* range);

    
    void writeNumSet(IloNumSetI* set, const char* range);

    
    void writeSymbolSet(IloAnySetI* set, const char* range);

    
    void writeTupleSet(IloTupleSetI* set, const char* range);



    
    void writeInt(IloInt val, const char* range);

    
    void writeNum(IloNum val, const char* range);

    
    void writeSymbol(IloSymbolI* val, const char* range);

    
    void writeTuple(IloTupleI* tuple, const char* range);



    
    void writeIntMap(IloMapI* set, const char* range);
    
    void writeNumMap(IloMapI* set, const char* range);
    
    void writeSymbolMap(IloMapI* set, const char* range);
    
    void writeTupleMap(IloAbstractTupleMapI* set, const char* range);

	void save();
};

#endif //ILO_MSVC

#ifdef _WIN32
#pragma pack(pop)
#endif

#endif


