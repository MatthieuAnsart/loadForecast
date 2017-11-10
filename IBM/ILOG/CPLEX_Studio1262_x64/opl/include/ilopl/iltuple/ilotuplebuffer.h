// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilopl/iltuple/ilotuplebuffer.h
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



#ifndef __ADVANCED_ilotuplebufferH
#define __ADVANCED_ilotuplebufferH

#ifdef _WIN32
#pragma pack(push, 8)
#endif
#include <ilopl/ilosys.h>


#include <ilconcert/ilocollection.h>
#include <ilconcert/ilolinkedlist.h>
#include <ilopl/iloenv.h>

class IloTupleI;
class IloTupleCollectionI;
class IloTupleSchemaI;


IloBool operator==(const IloTupleCellArray cells1, const IloTupleCellArray cells2);


class IloTuplePathListI : public IloEnvObjectI {
	IloInt _size;
	IloTuplePath** _values;
public:
	IloTuplePathListI(IloEnvI* env, IloTuplePathListI* toCopy) : IloEnvObjectI(env), _size(toCopy->getSize()), _values((IloTuplePath**)env->alloc( sizeof(IloTuplePath*)*toCopy->getSize())){
		memcpy((void*)_values, (const void*)toCopy->getValues(), _size*sizeof(IloTuplePath*));
	}
	IloTuplePathListI(IloEnv env, IloInt size) : IloEnvObjectI(env.getImpl()), _size(size), _values(0){
		if (_size) {
			_values = (IloTuplePath**)getEnv()->alloc( sizeof(IloTuplePath*)*size );
			memset(_values, 0, sizeof(IloTuplePath*)*size);
		}
	}
	~IloTuplePathListI(){
		getEnv()->free(_values, sizeof(IloTuplePath*)*_size);
		_values = 0;
		_size = 0;
	}

	IloTuplePath*& operator[] (IloInt i) {
		IloAssert ( i>=0, "Index out of bounds operation: negative index");
		IloAssert ( i < _size, "X& IloArray::operator[] (IloInt i) : Out of bounds operation: index superior to size of array");
		return _values[i];
	}
	void add(IloTuplePath* i);
	void setSize(IloInt i);
	inline IloInt getSize() const {
		return _size;
	}
	IloTuplePathListI* copy(){
		return new (getEnv()) IloTuplePathListI(getEnv(), this);
	}
	void clear() {
		getEnv()->free(_values, sizeof(IloTuplePath*)*_size);
		_values = 0;
		_size = 0;
	}
	IloTuplePath** getValues(){ return _values; }
};


class IloTuplePathList {
	IloTuplePathListI* _impl;
public:
	IloTuplePathList(IloEnv env, IloInt size = 0) : _impl(new (env) IloTuplePathListI(env, size)){};
	IloTuplePathList() : _impl(0){};
	IloTuplePathList(IloTuplePathListI* impl) : _impl(impl){};
	IloTuplePathListI* getImpl() const{
		return _impl;
	}
	IloTuplePath* operator[] (IloInt i) {
		return _impl->operator[](i);
	}
	const IloTuplePath* operator[] (IloInt i) const{
		return _impl->operator[](i);
	}
	void add(IloTuplePath* i){
		_impl->add(i);
	}
	void end(){
		if (_impl) delete _impl;
		_impl = 0;
	}
	IloInt getSize() const { return _impl->getSize(); }

	IloTuplePathListI* copy() const{
		return _impl->copy();
	}
	IloEnv getEnv() const{ return _impl->getEnv(); }
	void clear() {
		_impl->clear();
	}
	void setSize(IloInt i){
		_impl->setSize(i);
	}
};


//-------------------------------------


class IloTuplePathBufferI : public IloEnvObjectI{
	friend class IloTupleBufferI;
public:
	void sort(IloTupleCollectionI* coll);
	IloTuplePath* add(IloIntArray path, IloInt value);
	IloTuplePath* add(IloIntArray path, IloNum value);
	IloTuplePath* add(IloIntArray path, IloAny value);

	IloTuplePath* modify(IloTuplePath* tuple, IloInt value);
	IloTuplePath* modify(IloTuplePath* tuple, IloNum value);
	IloTuplePath* modify(IloTuplePath* tuple, IloAny value);

protected:
	IloBool _isOrd;
	IloTuplePathList _list;
	IloTuplePath* contains(IloInt idx);
	IloTuplePath* contains(IloIntArray array);
	IloTuplePath* contains(IloInt size, IloInt* path);
public:
	IloBool isOrd() const{ return _isOrd; }
	void setIsOrd(IloBool flag) { _isOrd = flag; }
		
	IloTuplePathBufferI(IloEnv env);
	virtual ~IloTuplePathBufferI();

		
	IloBool isEmpty() const { return _list.getSize() == 0; }
		
	IloBool isSingleton() const { return _list.getSize() == 1; }

		
	IloInt getSize() const { return _list.getSize(); }
		
	IloTuplePathList& getList() { return _list; }

		
	void clear();
		
	void import(IloTuplePathBufferI* select, IloInt prefix =-1);

		
	void import(IloInt idx1, IloTupleI* value);
		
	void import(IloIntArray path, IloTupleI* value);
		
	void import(IloInt pathSize, IloInt* path, IloTupleI* value);

		
	IloTuplePath* addOnce(IloInt columnIndex, IloInt value);
		
	IloTuplePath* addOnce(IloInt columnIndex, IloNum value);
		
	IloTuplePath* addOnce(IloInt columnIndex, IloAny value);

		
	IloTuplePath* addOnce(IloIntArray path, IloBool isArrayInternal, IloInt value);
		
	IloTuplePath* addOnce(IloIntArray path, IloBool isArrayInternal, IloNum value);
		
	IloTuplePath* addOnce(IloIntArray path, IloBool isArrayInternal, IloAny value);

		
	IloTuplePath* addOnce(IloInt pathSize, IloInt* path, IloInt value);
		
	IloTuplePath* addOnce(IloInt pathSize, IloInt* path, IloNum value);
		
	IloTuplePath* addOnce(IloInt pathSize, IloInt* path, IloAny value);

		
	virtual void display(ILOSTD(ostream)& os) const;
};



class IloTuplePathBuffer {
protected:
	IloTuplePathBufferI* _impl;
public:
	
	IloEnv getEnv(){
		return _impl->getEnv();
	}

		
	IloTuplePathList& getList(){
		return _impl->getList();
	}

	
	IloTuplePathBuffer(IloTuplePathBufferI* impl) : _impl(impl){ }

	
	IloTuplePathBufferI* getImpl() const {
		return _impl;
	}

	
	void end(){
		if (_impl) {
			delete _impl; _impl = 0;
		}
	}
	
	IloTuplePathBuffer(IloEnv env);

	
	void display(ILOSTD(ostream)& os) const{
		IloAssert(getImpl() != 0, "IloTuplePathBuffer: Using empty handle");
		_impl->display(os);
	}
	
	IloInt getSize() const {
		IloAssert(getImpl() != 0, "IloTuplePathBuffer: Using empty handle");
		return _impl->getSize();
	}
};

#ifdef _WIN32
#pragma pack(pop)
#endif


#endif
