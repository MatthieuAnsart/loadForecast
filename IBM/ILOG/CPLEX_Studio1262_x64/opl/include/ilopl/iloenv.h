// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilopl/iloenv.h
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


#ifndef __ADVANCED_iloenvH
#define __ADVANCED_iloenvH

#ifdef _WIN32
#pragma pack(push, 8)
#endif

#ifndef __OPL_ilosysH
# include <ilopl/ilosys.h>
#endif
#ifndef __CONCERT_iloenvH
# include <ilconcert/iloenv.h>
#endif

#include <ilconcert/ilocollection.h>

#ifdef ILO_LINUX
#include <cstring>
#endif

#define IloSymbolSetI IloAnySetI

class IloDataCollectionI;
class IloEndCollectionCallbackI : public IloDestroyableI {
	ILORTTIDECL
public:
	IloEndCollectionCallbackI(IloEnvI* env) : IloDestroyableI(env) {}
	virtual ~IloEndCollectionCallbackI(){}

	virtual void shareCollection(IloRttiEnvObjectI* coll) = 0;
	virtual void endCollection(IloRttiEnvObjectI*) = 0;
	virtual IloBool isSharing(IloRttiEnvObjectI*) const = 0;
	virtual IloBool isSharingSpecial(IloRttiEnvObjectI*) const = 0;
	virtual void addDependency(IloRttiEnvObjectI* obj, IloRttiEnvObjectI* dep) = 0;
	static void Register(IloEnvI* env, IloEndCollectionCallbackI* cb);
	static void Unregister(IloEnvI* env, IloEndCollectionCallbackI* cb);
	static IloBool HasInstance(IloEnvI* env);
	static IloEndCollectionCallbackI* GetInstance(IloEnvI* env);
};


class IloIntFixedArrayI : public IloEnvObjectI {
	IloInt _size;
	IloInt* _values;
public:
	IloIntFixedArrayI(IloIntFixedArrayI* toCopy) : IloEnvObjectI(toCopy->getEnv()), _size(toCopy->getSize()), _values((IloInt*)getEnv()->alloc( sizeof(IloInt)*toCopy->getSize() )){
		memcpy((void*)_values, (const void*)toCopy->getValues(), _size*sizeof(IloInt));
	}

	IloIntFixedArrayI(IloEnv env, IloInt size) : IloEnvObjectI(env.getImpl()), _size(size), _values(0){
		if (_size) _values = (IloInt*)getEnv()->alloc( sizeof(IloInt)*size );
	}
	~IloIntFixedArrayI(){
		getEnv()->free(_values, sizeof(IloInt)*_size);
		_values = 0;
		_size = 0;
	}
	IloInt operator[] (IloInt i) const{
		IloAssert ( i>=0, "Index out of bounds operation: negative index");
		IloAssert ( i < _size, "X& IloArray::operator[] (IloInt i) : Out of bounds operation: index superior to size of array");
		return _values[i];
	}
	IloInt& operator[] (IloInt i){
		IloAssert ( i>=0, "Index out of bounds operation: negative index");
		IloAssert ( i < _size, "X& IloArray::operator[] (IloInt i) : Out of bounds operation: index superior to size of array");
		return _values[i];
	}
	void setValue(IloInt index, IloInt value){
		IloAssert ( index>=0, "Index out of bounds operation: negative index");
		IloAssert ( index < _size, "X& IloArray::operator[] (IloInt i) : Out of bounds operation: index superior to size of array");
		_values[index] = value;
	}
	IloInt getSize() const { return _size;}
	IloIntFixedArrayI* copy(){
		return new (getEnv()) IloIntFixedArrayI(this);
	}
	IloInt* getValues(){ return _values; }

	void zeroData(){
		memset(_values, 0, sizeof(IloInt)*_size);
	}
	void add(IloInt i){
		IloInt* temp = (IloInt*)getEnv()->alloc( sizeof(IloInt)*(_size+1) );
		for (IloInt j=0; j< _size; j++){
			IloInt cell = _values[j];
			temp[j] = cell;
		}
		temp[_size] = i;
		getEnv()->free(_values, sizeof(IloInt)*_size);
		_values = temp;
		_size++;
	}
	void add(IloIntFixedArrayI* array){
		IloInt* temp = (IloInt*)getEnv()->alloc( sizeof(IloInt)*(_size+ array->getSize()) );
		for (IloInt j=0; j< _size; j++){
			IloInt cell = _values[j];
			temp[j] = cell;
		}
		for (IloInt i=0; i< array->getSize(); i++){
			IloInt cell = array->getValues()[i];
			temp[_size+i] = cell;
		}
		getEnv()->free(_values, sizeof(IloInt)*_size);
		_values = temp;
		_size+=array->getSize();
	}
};

class IloIntFixedArray {
    IloIntFixedArrayI* _impl;
public:
	IloIntFixedArray(IloEnv env, IloInt size = 0) : _impl(new (env) IloIntFixedArrayI(env, size)){};
	IloIntFixedArray() : _impl(0){};
	IloIntFixedArray(IloIntFixedArrayI* impl) : _impl(impl){};
	IloIntFixedArrayI* getImpl() const{
		return _impl;
	}
	IloInt operator[] (IloInt i) const {
		return _impl->operator[](i);
	}
	IloInt& operator[] (IloInt i){
		return _impl->operator[](i);
	}
	void setValue(IloInt index, IloInt value){
		_impl->setValue(index, value);
	}

	void end(){
		if (_impl) delete _impl;
		_impl = 0;
	}
	IloInt getSize() const { return _impl->getSize(); }
	IloIntFixedArrayI* copy() const{
		return _impl->copy();
	}
	IloEnv getEnv() const{ return _impl->getEnv(); }
	void add(IloInt i){ _impl->add(i); }
	void add(IloIntFixedArray i){ _impl->add(i.getImpl()); }

	IloBool contains(IloInt val){
		for (IloInt i=0; i< getSize(); i++){
			if (operator[](i) == val) return IloTrue;
		}
		return IloFalse;
	}
};





class IloAnyFixedArrayI : public IloEnvObjectI {
	IloInt _size;
	IloAny* _values;
public:
	IloAnyFixedArrayI(IloAnyFixedArrayI* toCopy) : IloEnvObjectI(toCopy->getEnv()), _size(toCopy->getSize()), _values((IloAny*)getEnv()->alloc( sizeof(IloAny)*toCopy->getSize() )){
		memcpy((void*)_values, (const void*)toCopy->getValues(), _size*sizeof(IloAny));
	}

	IloAnyFixedArrayI(IloEnv env, IloInt size) : IloEnvObjectI(env.getImpl()), _size(size), _values(0){
		if (_size) _values = (IloAny*)getEnv()->alloc( sizeof(IloAny)*size );
	}
	~IloAnyFixedArrayI(){
		getEnv()->free(_values, sizeof(IloAny)*_size);
		_values = 0;
		_size = 0;
	}
	IloAny* getValues(){ return _values; }
	IloAny operator[] (IloInt i) const{
		IloAssert ( i>=0, "Index out of bounds operation: negative index");
		IloAssert ( i < _size, "X& IloArray::operator[] (IloAny i) : Out of bounds operation: index superior to size of array");
		return _values[i];
	}
	IloAny& operator[] (IloInt i){
		IloAssert ( i>=0, "Index out of bounds operation: negative index");
		IloAssert ( i < _size, "X& IloArray::operator[] (IloAny i) : Out of bounds operation: index superior to size of array");
		return _values[i];
	}
	IloInt getSize() const { return _size;}
	void add(IloAny i){
		IloAny* temp = (IloAny*)getEnv()->alloc( sizeof(IloAny)*(_size+1) );
		for (IloInt j=0; j< _size; j++){
			IloAny cell = _values[j];
			temp[j] = cell;
		}
		temp[_size] = i;
		getEnv()->free(_values, sizeof(IloAny)*_size);
		_values = temp;
		_size++;
	}
	void add(IloAnyFixedArrayI* array){
		IloAny* temp = (IloAny*)getEnv()->alloc( sizeof(IloAny)*(_size+ array->getSize()) );
		for (IloInt j=0; j< _size; j++){
			IloAny cell = _values[j];
			temp[j] = cell;
		}
		for (IloInt i=0; i< array->getSize(); i++){
			IloAny cell = array->getValues()[i];
			temp[_size+i] = cell;
		}
		getEnv()->free(_values, sizeof(IloAny)*_size);
		_values = temp;
		_size+=array->getSize();
	}

};

class IloAnyFixedArray {
    IloAnyFixedArrayI* _impl;
public:
	IloAnyFixedArray(IloEnv env, IloInt size = 0) : _impl(new (env) IloAnyFixedArrayI(env, size)){};
	IloAnyFixedArray() : _impl(0){};
	IloAnyFixedArray(IloAnyFixedArrayI* impl) : _impl(impl){};
	IloAnyFixedArrayI* getImpl() const{
		return _impl;
	}
	IloAny operator[] (IloInt i) const {
		return _impl->operator[](i);
	}
	IloAny& operator[] (IloInt i){
		return _impl->operator[](i);
	}
	void end(){
		if (_impl) delete _impl;
		_impl = 0;
	}
	IloInt getSize() const { return _impl->getSize(); }
	IloEnv getEnv() const{ return _impl->getEnv(); }
	void add(IloAny i){ _impl->add(i); }
	void add(IloAnyFixedArray i){ _impl->add(i.getImpl()); }
};


#if defined(ILOUSESTL) && !defined(ILO_HP)
class IloOplStringHelper {
public:
    IloOplStringHelper() {
    }
    virtual ~IloOplStringHelper() {
    }
    void printEscaped(std::ostream& os, char c) const;
    virtual void printEscaped(std::ostream& os, const char* chunk) const;
};

class IloOplMultiByteHelper: public IloOplStringHelper {
    int  _mbCurMax;
public:
    static IloOplMultiByteHelper* NewOrNull(IloEnvI* env);

    explicit IloOplMultiByteHelper(int mbCurMax);
    int getMax() const {
      return (int)_mbCurMax;
    }

    int lastCharOffset(const char* chunk, int len) const;
    int lastCharOffsetAndFixColumn(const char* chunk, int len, int& col) const;
    int charOffset(int charIndex, const char* chunk, int len) const;
    bool isMB(const char* chunk) const;
    int countChars(const char* chunk, int len =-1) const;
    bool hasMB(const char* chunk) const;
	using IloOplStringHelper::printEscaped;
    virtual void printEscaped(std::ostream& os, const char* chunk) const;
};
#endif


IloIntArray intersectAscSortedIndex(IloEnv env, IloIntArray set1, IloIntArray set2);
IloNumArray intersectAscSortedIndex(IloEnv env, IloNumArray set1, IloNumArray set2);
IloAnyArray intersectAscSortedIndex(IloEnv env, IloAnyArray set1, IloAnyArray set2);




class IloTuplePath : public IloEnvObjectI {
public:
	
	class IloTupleCell {
	public:
		union Value{
			IloInt _int;
			IloNum _num;
			IloAny _any;
		};
	private:
		IloDataCollection::IloDataType _type;
		Value _value;
	public:
		IloDataCollection::IloDataType getType() const { return _type; }
		Value getValue() const { return _value; }
		
		IloInt* getIntAddress() { return &(_value._int); }
		
		IloNum* getNumAddress() { return &(_value._num); }
		
		IloAny* getAnyAddress() { return &(_value._any); }

		
		IloBool isInt() const{ return (_type == IloDataCollection::IntDataColumn);	}
		
		IloBool isNum() const{ return (_type == IloDataCollection::NumDataColumn);	}
		
		IloBool isAny() const{ return (_type == IloDataCollection::AnyDataColumn); }

#ifdef ILO_WIN64
		IloInt getIntValue() const;
		
		IloNum getNumValue() const;
		
		IloAny getAnyValue() const;
#else
		
		IloInt getIntValue() const{
			if (this->isInt())
				return _value._int;
			else if (this->isNum() && IloNumIsInteger(_value._num))
				return IloNumToInt(_value._num);
			throw IloWrongUsage("IloTuplePath::IloTupleCell::getIntValue() type not int nor num");
			ILOUNREACHABLE(return 0;)
		}
		
		IloNum getNumValue() const{
			if (this->isNum())
				return _value._num;
			else if (this->isInt())
				return (IloNum)_value._int;
			throw IloWrongUsage("IloTuplePath::IloTupleCell::getNumValue() type not num nor int");
			ILOUNREACHABLE(return 0;)
		}
		
		IloAny getAnyValue() const{
			return _value._any;
		}
#endif

		
		IloTupleCell(IloInt val) {
			_type = IloDataCollection::IntDataColumn;
			_value._int = val;
		}
		
		IloTupleCell(IloNum val){
			_type = IloDataCollection::NumDataColumn;
			_value._num = val;
		}
		
		IloTupleCell(IloAny val){
			_type = IloDataCollection::AnyDataColumn;
			_value._any = val;
		}
		
		void setValue(IloInt val){
			_type = IloDataCollection::IntDataColumn;
			_value._int = val;
		}
		
		void setValue(IloNum val){
			_type = IloDataCollection::NumDataColumn;
			_value._num = val;
		}
		
		void setValue(IloAny val){
			_type = IloDataCollection::AnyDataColumn;
			_value._any = val;
		}
		~IloTupleCell(){}

		IloBool operator!=(const IloTupleCell& other) {
			if (_type != other.getType()) return IloTrue;
			switch(_type){
			case IloDataCollection::IntDataColumn:
				return _value._int != other._value._int;
			case IloDataCollection::NumDataColumn:
				return _value._num != other._value._num;
			default:
				return _value._any != other._value._any;
			}
		}
		IloBool operator==(const IloTupleCell& other) {
			if (_type != other.getType()) return IloFalse;
			switch(_type){
			case IloDataCollection::IntDataColumn:
				return _value._int == other._value._int;
			case IloDataCollection::NumDataColumn:
				return _value._num == other._value._num;
			default:
				return _value._any == other._value._any;
			}
		}
	};
public:
  enum IloTupleRequestType {
    Equality = 0,
    LowerBound = 32,
    UpperBound = 64
  };

private:
	IloTupleCell _cell;
	IloIntArray _path;
	IloTupleRequestType _reqType;
public:
		
	IloTupleCell& getCell() { return _cell; }
		
	IloIntArray getPath() const { return _path; }
public:
	~IloTuplePath(){ _path.end(); }
	void setValue(IloTupleCell cell){ _cell = cell;}
		
	void setValue(IloInt value){ getCell().setValue(value);	}
		
	void setValue(IloNum value){ getCell().setValue(value);	}
		
	void setValue(IloAny value){ getCell().setValue(value);	}
		
	void setValue(IloDiscreteDataCollection value){ getCell().setValue((IloAny)value.getImpl()); }
		
	IloTupleRequestType getRequestType() const {
		return _reqType;
	}
		
	IloBool isEquality() const {
		return _reqType==Equality;
	}
		
	IloBool isLowerBound() const {
		return _reqType==LowerBound;
	}
		
	IloBool isUpperBound() const {
		return _reqType==UpperBound;
	}


	IloTuplePath(IloIntArray path, IloTupleCell cell, IloTupleRequestType reqType = Equality):
	IloEnvObjectI(path.getEnv().getImpl()), _cell(cell), _path(path), _reqType(reqType) {
	}
		
	IloTuplePath(IloEnv env, IloIntArray path, IloAny value, IloTupleRequestType reqType = Equality):
	IloEnvObjectI(env.getImpl()), _cell(value), _path(path), _reqType(reqType) {
		if (!path.getImpl()) throw IloEmptyHandleException("IloTuplePath: path is empty");
	}
		
	IloTuplePath(IloEnv env, IloIntArray path, IloNum value, IloTupleRequestType reqType = Equality):
	IloEnvObjectI(env.getImpl()), _cell(value), _path(path), _reqType(reqType) {
		if (!path.getImpl()) throw IloEmptyHandleException( "IloTuplePath: path is empty");
	}
		
	IloTuplePath(IloEnv env, IloIntArray path, IloInt value, IloTupleRequestType reqType = Equality):
	IloEnvObjectI(env.getImpl()), _cell(value), _path(path), _reqType(reqType) {
		if (!path.getImpl()) throw IloEmptyHandleException( "IloTuplePath: path is empty");
	}
};

typedef IloArray<IloTuplePath*> IloTuplePathArray;

class IloTupleCellArrayI : public IloMemoryManagerObjectI {
  IloInt _size;
  IloTuplePath::IloTupleCell* _values;
public:

  IloTupleCellArrayI(IloMemoryManagerI* mmgi, const IloTupleCellArrayI* toCopy) ;
  IloTupleCellArrayI(IloMemoryManager   mmg , IloInt size);

#ifdef ILODELETEOPERATOR
  void operator delete(void*, const IloEnv&);
  void operator delete(void*, const IloEnvI*);
  void operator delete(void*, const IloMemoryManager&);
  void operator delete(void*, const IloMemoryManagerI*);
#endif

  // the actual operator delete wich dispatches delete to the mmgr.
  // simply calls IloMemoryManagerI::operator delete .
  void operator delete(void * obj, size_t size);

  ~IloTupleCellArrayI();

  inline IloInt getSize() const { return _size; }

  IloTupleCellArrayI* copy() {
    return new (getMemoryManager()) IloTupleCellArrayI(getMemoryManager(), this);
  }

  IloTupleCellArrayI* makeClone(IloMemoryManagerI* env){
    return new (env) IloTupleCellArrayI(env, this);
  }

  const IloTuplePath::IloTupleCell& operator[] (IloInt i) const{
    IloAssert ( i>=0, "Index out of bounds operation: negative index");
    IloAssert ( i < _size, "X& IloArray::operator[] (IloInt i) : Out of bounds operation: index superior to size of array");
    return _values[i];
  }
  IloTuplePath::IloTupleCell& operator[] (IloInt i) {
    IloAssert ( i>=0, "Index out of bounds operation: negative index");
    IloAssert ( i < _size, "X& IloArray::operator[] (IloInt i) : Out of bounds operation: index superior to size of array");
    return _values[i];
  }

  inline void add(IloTuplePath::IloTupleCell i){
    IloInt ss = sizeof(IloTuplePath::IloTupleCell);
    IloInt oldsize = ss*_size;
    IloTuplePath::IloTupleCell* temp = (IloTuplePath::IloTupleCell*)getMemoryManager()->alloc( ss+oldsize );
    memcpy(temp, _values, oldsize);
    temp[_size] = i;
    getMemoryManager()->free(_values, oldsize);
    _values = temp;
    _size++;
  }

  void add(IloTupleCellArrayI* array) {
    IloInt ss = sizeof(IloTuplePath::IloTupleCell);
    IloInt oldsize = ss*_size;
    IloTuplePath::IloTupleCell* temp = (IloTuplePath::IloTupleCell*)getMemoryManager()->alloc( ss*array->getSize()+oldsize );
    memcpy(temp, _values, oldsize);
    for (IloInt i=0; i< array->getSize(); i++){
      IloTuplePath::IloTupleCell cell = array->getValues()[i];
      temp[_size+i] = cell;
    }
    getMemoryManager()->free(_values, oldsize);
    _values = temp;
    _size+=array->getSize();
  }

  void setSize(IloInt size) {
    IloInt ss = sizeof(IloTuplePath::IloTupleCell);
    IloInt oldsize = ss*_size;
    IloInt newsize = ss*size;
    IloTuplePath::IloTupleCell* temp = (IloTuplePath::IloTupleCell*)getMemoryManager()->alloc( newsize );
    if (size > _size)
       memcpy(temp, _values, oldsize);
    else
       memcpy(temp, _values, newsize);
    getMemoryManager()->free(_values,oldsize);
    _values = temp;
    _size = size;
  }

  void clear();

  IloTuplePath::IloTupleCell* getValues() { return _values; }
  const IloTuplePath::IloTupleCell* getValues() const { return _values; }

}; // class IloTupleCellArrayI
// ---------------------------------

class IloTupleCollectionI;
class IloTupleSchemaI;


class IloTupleCellArray {
  IloTupleCellArrayI* _impl;
public:
  
  IloTupleCellArray(IloEnv env) : _impl(new (env) IloTupleCellArrayI(env, 0)){};
  
  IloTupleCellArray(IloEnv env, IloInt size) : _impl(new (env) IloTupleCellArrayI(env, size)){};
  IloTupleCellArray(IloMemoryManager env, IloInt size) : _impl(new (env) IloTupleCellArrayI(env, size)){};
  IloTupleCellArray(IloMemoryManager env) : _impl(new (env) IloTupleCellArrayI(env, 0)){};
  IloTupleCellArray() : _impl(0){};
  IloTupleCellArray(IloTupleCellArrayI* impl) : _impl(impl){};
  IloTupleCellArrayI* getImpl() const{
    return _impl;
  }
  
  void setSize(IloInt size) {
    _impl->setSize(size);
  }
  const IloTuplePath::IloTupleCell& operator[] (IloInt i) const {
    return _impl->operator[](i);
  }
  IloTuplePath::IloTupleCell& operator[] (IloInt i) {
    return _impl->operator[](i);
  }

  void add(IloTuplePath::IloTupleCell i){
    _impl->add(i);
  }
  void add(IloTupleCellArray array){
    _impl->add(array.getImpl());
  }
  
  void end(){
    if (_impl) delete _impl;
    _impl = 0;
  }
  
  IloInt getSize() const { return _impl->getSize(); }

  IloBool isHomogeneous() const;
  void convertTo(const IloTupleCollectionI* set, const IloTupleSchemaI* schema =0);
  void convertTo(const IloTupleCollectionI*set, IloEnvI* env);
  void display(ILOSTD(ostream)& os) const;
  IloTupleCellArray makeSlice(IloInt index, IloInt size);
  IloTupleCellArray fillSlice(IloTupleCellArray array, IloInt index, IloInt size);
  void endAll();
  IloTupleCellArrayI* copy() const{
    return _impl->copy();
  }
  IloTupleCellArrayI* makeClone(IloMemoryManagerI* env) const{
    return _impl->makeClone(env);
  }
  IloMemoryManager getMemoryManager() const{ return _impl->getMemoryManager(); }
  
  void clear() {
    _impl->clear();
  }
  
  void setIntValue(IloInt index, IloInt value);
  
  void setNumValue(IloInt index, IloNum value);
  
  void setSymbolValue(IloInt index, IloSymbol value);



  
  IloInt getIntValue(IloInt index);
  
  IloNum getNumValue(IloInt index);
  
  IloSymbol getSymbolValue(IloInt index);
  
  IloBool isIntValue(IloInt index);
  
  IloBool isNumValue(IloInt index);
  
  IloBool isSymbolValue(IloInt index);

};

#ifdef _WIN32
#pragma pack(pop)
#endif

#endif
