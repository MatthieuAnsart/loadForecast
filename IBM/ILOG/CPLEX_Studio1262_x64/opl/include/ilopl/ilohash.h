// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilconcert/ilohash.h
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5725-A06 5725-A29 5724-Y47 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5799-CPX
// Copyright IBM Corp. 2000, 2013
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// ---------------------------------------------------------------------------

#ifndef __ADVANCED_ilohashH
#define __ADVANCED_ilohashH

#ifdef _WIN32
#pragma pack(push, 8)
#endif

#include <ilconcert/ilosys.h>
#include <ilconcert/iloenv.h>
#include <ilconcert/ilohash.h>




class IloSetTable : public IloEnvObjectI {
public:
	enum Status {ILOHASHOK=0, ILOHASHNOTFOUND=1, ILOHASHDUPLICATEDKEY=2};

private:
	IloInt          _size;
	IloInt          _mod;

public:
	class Item {
		Item*   _next;
		IloAny _key;
	public:
		Item(IloAny key)
			: _next(0), _key(key){}
		Item*     getNext()  const          { return (_next); }
		IloAny   getKey()   const          { return (_key); }
		void setNext(Item* next)            { _next  = next; }
		void setKey(IloAny key)            { _key   = key; }
	};

private:
	Item** _table;

	void create() {
		if (!_table) {
			_table = (Item**) getMemoryManager()->alloc(sizeof(Item*)*_mod);
			IloInt i;
			for (i=0; i<_mod; i++) {
				_table[i] = 0;
			}
		}
	}

public:
	IloSetTable(IloEnv env)	: IloEnvObjectI(env.getImpl()),	_size(0), _mod(31), _table(0){
		create();
	}

	virtual ~IloSetTable() { clear(); }

	IloInt getMod() const         { return (_mod); }
	IloInt getSize() const        { return (_size); }
	Item** getTable() const       { return (_table); }

	void clear();
	Status addWithoutCheck(IloAny key);
	Status add(IloAny key);
	Status find(IloAny key) const;
	Status remove(IloAny key);
	void reMod(IloInt newMod);

	class Iterator {
		IloSetTable* _hash;
		Item* _item;
		IloInt _index;

	public:
		Iterator(IloSetTable* hash)	: _hash(hash), _item(0), _index(-1) {
				if (_hash->getTable()) {
					while (!_item && ++_index<_hash->getMod()) {
						_item = _hash->getTable()[_index];
					}
				}
		}
		IloBool ok() const  { return ((_hash != 0) && (_item!=0)); }
		void operator++ () {
			if (_item->getNext())
				_item = _item->getNext();
			else {
				_item = 0;
				while (!_item && ++_index<_hash->getMod())
					_item = _hash->getTable()[_index];
			}
		}
		IloAny   getKey() const    { return (_item->getKey()); }
	};
#ifdef ILODELETEOPERATOR
	void operator delete(void *, const IloEnvI *) { }
	void operator delete(void *, const IloEnv&) { }
#endif
	void operator delete(void * obj, size_t size) {
		IloEnvObjectI::operator delete(obj, size);
	}
};


#ifdef _WIN32
#pragma pack(pop)
#endif


#endif
