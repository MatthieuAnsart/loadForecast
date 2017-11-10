// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilcp/cp.h
// --------------------------------------------------------------------------
//
// Licensed Materials - Property of IBM
//
// 5724-Y48 5724-Y49 5724-Y54 5724-Y55 5725-A06 5725-A29
// Copyright IBM Corp. 1990, 2014 All Rights Reserved.
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
//
// --------------------------------------------------------------------------

#ifndef __CP_cpH
#define __CP_cpH

#ifdef _MSC_VER
#pragma pack(push,8)
#endif

#ifndef ILC_USE_GENERATED_CPO
# define ILC_USE_GENERATED_CPO
#endif

#ifndef CPPREF_GENERATION

#define IlcAbs IlcCPOAbs
#define IlcAllocationStack IlcCPOAllocationStack
#define IlcBranchSelectorI IlcCPOBranchSelectorI
#define IlcChooseIntVarI IlcCPOChooseIntVarI
#define IlcChooseIntVar IlcCPOChooseIntVar
#define IlcConstIntArray IlcCPOConstIntArray
#define IlcConstraintArray IlcCPOConstraintArray
#define IlcConstraintI IlcCPOConstraintI
#define IlcConstraint IlcCPOConstraint
#define IlcCPOFloatVarI IlcCPOFloatExpI
#define IlcDemonI IlcCPODemonI
#define IlcDemon IlcCPODemon
#define IlcExponent IlcCPOExponent
#define IlcExprI IlcCPOExprI
#define IlcExtension IlcCPOExtension
#define IlcFloatArrayI IlcCPOFloatArrayI
#define IlcFloatArray IlcCPOFloatArray
#define IlcFloatExpI IlcCPOFloatExpI
#define IlcFloatExp IlcCPOFloatExp
#define IlcFloatMax IlcCPOFloatMax
#define IlcFloatMin IlcCPOFloatMin
#define IlcFloatVarArrayI IlcCPOFloatVarArrayI
#define IlcFloatVarArray IlcCPOFloatVarArray
#define IlcFloatVarArrayIterator IlcCPOFloatVarArrayIterator
#define IlcFloatVarI IlcCPOFloatExpI
#define IlcFloatVar IlcCPOFloatVar
#define IlcGoalArray IlcCPOGoalArray
#define IlcGoalI IlcCPOGoalI
#define IlcGoal IlcCPOGoal
#define IlcIntArray IlcCPOIntArray
#define IlcIntExpI IlcCPOIntExpI
#define IlcIntExp IlcCPOIntExp
#define IlcIntExpIterator IlcCPOIntExpIterator
#define IlcIntPredicateI IlcCPOIntPredicateI
#define IlcIntPredicate IlcCPOIntPredicate
#define IlcIntSelectEvalI IlcCPOIntSelectEvalI
#define IlcIntSelectI IlcCPOIntSelectI
#define IlcIntSelect IlcCPOIntSelect
#define IlcIntSetArray IlcCPOIntSetArray
#define IlcIntSetI IlcCPOIntSetI
#define IlcIntSet IlcCPOIntSet
#define IlcIntSetIterator IlcCPOIntSetIterator
#define IlcIntSetVarArray IlcCPOIntSetVarArray
#define IlcIntSetVarDeltaIterator IlcCPOIntSetVarDeltaIterator
#define IlcIntSetVarI IlcCPOIntSetVarI
#define IlcIntSetVar IlcCPOIntSetVar
#define IlcIntSetVarIterator IlcCPOIntSetVarIterator
#define IlcIntTupleSet IlcCPOIntTupleSet
#define IlcIntTupleSetIterator IlcCPOIntTupleSetIterator
#define IlcIntVarArrayI IlcCPOIntVarArrayI
#define IlcIntVarArray IlcCPOIntVarArray
#define IlcIntVarDeltaIterator IlcCPOIntVarDeltaIterator
#define IlcIntVarI IlcCPOIntVarI
#define IlcIntVar IlcCPOIntVar
#define IlcLog IlcCPOLog
#define IlcManagerI IlcCPOManagerI
#define IlcManager IlcCPOManager
#define IlcMax IlcCPOMax
#define IlcMin IlcCPOMin
#define IlcPower IlcCPOPower
#define IlcRandomI IlcCPORandomI
#define IlcRandom IlcCPORandom
#define IlcRevAny IlcCPORevAny
#define IlcRevBool IlcCPORevBool
#define IlcRevFloat IlcCPORevFloat
#define IlcRevInt IlcCPORevInt
#define IlcSearchI IlcCPOSearchI
#define IlcSearchLimitI IlcCPOSearchLimitI
#define IlcSearchMonitorI IlcCPOSearchMonitorI
#define IlcSearchMonitor IlcCPOSearchMonitor
#define IlcStamp IlcCPOStamp
#define IloCPConstraintI IloCPOCPConstraintI
#define IloFailLimit IloCPOFailLimit
#define IloGoalFail IloCPOGoalFail
#define IloGoalI IloCPOGoalI
#define IloGoal IloCPOGoal
#define IloGoalTrue IloCPOGoalTrue
#define IloOrLimit IloCPOOrLimit
#define IloSearchLimitI IloCPOSearchLimitI
#define IloSearchLimit IloCPOSearchLimit
#define IloSolver IloCPOSolver
#define IloTimeLimit IloCPOTimeLimit
#define IlcIntSetIteratorI IlcCPOIntSetIteratorI

#endif

//----------------------------------------------------------------------
// Macros for GCC attributes

//#define ILC_GCC_VISIBILITY

#if defined(__GNUC__) && (__GNUC__ >= 4) && defined(ILC_GCC_VISIBILITY)
#  define ILCGCCHIDINGENABLED
#  define ILCGCCHIDINGON      _Pragma("GCC visibility push(hidden)")
#  define ILCGCCHIDINGOFF     _Pragma("GCC visibility pop")
#  define ILCHIDDEN           __attribute__((visibility("hidden")))
#  define ILCGCCEXPORTON      _Pragma("GCC visibility push(default)")
#  define ILCGCCEXPORTOFF     _Pragma("GCC visibility pop")
#  define ILCEXPORT           __attribute__((visibility("default")))
#else
#  define ILCGCCHIDINGON
#  define ILCGCCHIDINGOFF
#  define ILCHIDDEN
#  define ILCGCCEXPORTON
#  define ILCGCCEXPORTOFF
#  define ILCEXPORT
#endif

#if defined(__GNUC__)
#define ILCDEPRECATED __attribute__((deprecated))
#elif defined(_MSC_VER)
#define ILCDEPRECATED __declspec(deprecated)
#else
#define ILCDEPRECATED
#endif

ILCGCCHIDINGON
class IloCPI;
class IloLaExtractorI;
class IlcLaMessageStore;
ILCGCCHIDINGOFF

//----------------------------------------------------------------------

#if !defined(__CONCERT_iloalgH)
# include <ilconcert/iloalg.h>
#endif
#if !defined(__CONCERT_ilomodelH)
# include <ilconcert/ilomodel.h>
#endif
#if !defined(__CONCERT_ilotuplesetH)
# include <ilconcert/ilotupleset.h>
#endif
#if !defined(__CONCERT_ilosmodelH)
# include <ilconcert/ilosmodel.h>
#endif
#if !defined(__CONCERT_ilosatomiH)
# include <ilconcert/ilsched/ilosatomi.h>
#endif

ILCGCCEXPORTON

//----------------------------------------------------------------------

#ifdef NDEBUG

#define IlcCPOAssert(x,y)

#elif defined (USEILOASSERTFORILCASSERT)

#define IlcCPOAssert(x,y) IloAssert(x,y)

#else

inline int ilc_stop_assert() { return 0; }
void IlcBacktrace(int maxLevels = 64);
#define IlcCPOAssert(x,y) assert((x) || (IlcBacktrace(), ILOSTD(cerr) << (y) << ILOSTD(endl), ilc_stop_assert()))

#endif


//----------------------------------------------------------------------

#define ILOCPVISIBLEHANDLEMINI(Hname, Iname)                       \
public:                                                            \
                                                                \
  Hname(Iname* impl = 0) : _impl(impl) { }                         \
                                                                \
  Iname* getImpl() const { return _impl; }                         \
  Iname* getImplInternal() const                                   \
    { return (Iname*)_impl; }                                      \
protected:                                                         \
  Iname* _impl;

#ifdef ILCGCCHIDINGENABLED
#define ILOCPHIDDENHANDLEMINI(Hname, Iname)                        \
public:                                                            \
                                                                \
  Hname(Iname* impl = 0) : _impl(impl) { }                         \
                                                                \
  Iname* getImpl() const { return (Iname*)_impl; }                 \
  Iname* getImplInternal() const                                   \
    { return (Iname*)_impl; }                                      \
protected:                                                         \
  void* _impl;
#else
#define ILOCPHIDDENHANDLEMINI(Hname, Iname)                        \
        ILOCPVISIBLEHANDLEMINI(Hname, Iname)
#endif

#define ILOCPVISIBLEHANDLE(Hname, Iname)                           \
  ILOCPVISIBLEHANDLEMINI(Hname, Iname)                             \
private:                                                           \
  const char *  _getName() const;                                  \
  IloAny        _getObject() const;                                \
  void          _setName(const char * name) const;                 \
  void          _setObject(IloAny obj) const;                      \
public:                                                            \
                                                                \
  const char * getName() const {                                   \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    return _getName();                                             \
  }                                                                \
                                                                \
  IloAny getObject() const {                                       \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    return _getObject();                                           \
  }                                                                \
                                                                \
  void setName(const char * name) const {                          \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    _setName(name);                                                \
  }                                                                \
                                                                \
  void setObject(IloAny obj) const {                               \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    _setObject(obj);                                               \
  }                                                                \

#ifdef ILCGCCHIDINGENABLED
#define ILOCPHIDDENHANDLE(Hname, Iname)                            \
  ILOCPHIDDENHANDLEMINI(Hname, Iname)                              \
private:                                                           \
  const char *  _getName() const;                                  \
  IloAny        _getObject() const;                                \
  void          _setName(const char * name) const;                 \
  void          _setObject(IloAny obj) const;                      \
public:                                                            \
                                                                \
  const char * getName() const {                                   \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    return _getName();                                             \
  }                                                                \
                                                                \
  IloAny getObject() const {                                       \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    return _getObject();                                           \
  }                                                                \
                                                                \
  void setName(const char * name) const {                          \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    _setName(name);                                                \
  }                                                                \
                                                                \
  void setObject(IloAny obj) const {                               \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    _setObject(obj);                                               \
  }
#else

#define ILOCPHIDDENHANDLE(Hname, Iname) ILOCPVISIBLEHANDLE(Hname, Iname)

#endif

#define ILOCPHANDLEINLINE(Hname, Iname)                            \
  ILOCPVISIBLEHANDLEMINI(Hname, Iname)                             \
public:                                                            \
                                                                \
  const char * getName() const {                                   \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    return _impl->getName();                                       \
  }                                                                \
                                                                \
  IloAny getObject() const {                                       \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    return _impl->getObject();                                     \
  }                                                                \
                                                               \
  void setName(const char * name) const {                          \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    _impl->setName(name);                                                \
  }                                                                \
                                                                \
  void setObject(IloAny obj) const {                               \
    IlcCPOAssert(_impl != 0, ILO_STRINGIZE(hname) ": empty handle");  \
    _impl->setObject(obj);                                               \
  }                                                                \

//----------------------------------------------------------------------

// Concert classes:
class IloCumulFunctionExpr;
class IloDiff;
class IloExtensibleRttiEnvObjectI;
class IloIntArray;
class IloIntExp;
class IloIntSet;
class IloIntSetVar;
class IloIntSetVarArray;
class IloIntVar;
class IloIntVarArray;
class IloStateFunctionI;

// Internal classes (not defined neither in cp.h nor cpext.h)
ILCGCCHIDINGON
class IlcAllocationStack;
class IlcExprI;
class IlcFloatExpI;
class IlcIntExpI;
class IlcIntVarI;
class IlcManagerI;
class IlcRandom;
class IlcRecomputeExprI;
class IlcStrategyManagerI;
class IloCPI;
class IloSearchPhaseI;
class IloValueSelectorI;
class IloVarSelectorI;
ILCGCCHIDINGOFF

// External classes (defined in cp.h or cpext.h)
class IlcConstraint;
class IlcConstraintArray;
class IlcCumulElementVar;
class IlcFloatArray;
class IlcFloatExp;
class IlcFloatVar;
class IlcFloatVarArray;
class IlcGoal;
class IlcIntArray;
class IlcIntervalSequenceVar;
class IlcIntervalVar;
class IlcIntExp;
class IlcIntSet;
class IlcIntSetVar;
class IlcIntSetVarArray;
class IlcIntTupleSet;
class IlcIntVar;
class IlcIntVarArray;
class IloCP;
class IloCPHookI;
class IloGoal;
class IloSolver;

////////////////////////////////////////////////////////////////////////
//
// CUSTOM SEARCH
//
////////////////////////////////////////////////////////////////////////


class IloIntVarEvalI : public IloExtensibleRttiEnvObjectI {
 public:
  
  IloIntVarEvalI(IloEnv env):
    IloExtensibleRttiEnvObjectI(env.getImpl()){}
  
  virtual IloNum eval(IloCP cp, IloIntVar x) = 0;
  
  virtual ~IloIntVarEvalI();
  ILORTTIDECL
};


class IloIntVarEval {
  ILOCPVISIBLEHANDLEMINI(IloIntVarEval, IloIntVarEvalI)
public:

  void end();
};


class IloIntValueEvalI : public IloExtensibleRttiEnvObjectI {
 public:
  
  IloIntValueEvalI(IloEnv env) :
    IloExtensibleRttiEnvObjectI(env.getImpl()){}
  
  virtual IloNum eval(IloCP cp, IloIntVar x, IloInt value) = 0;
  
  virtual ~IloIntValueEvalI();
  ILORTTIDECL
};


class IloIntValueEval {
  ILOCPVISIBLEHANDLEMINI(IloIntValueEval, IloIntValueEvalI)
public:

  void end();
};


class IloVarSelector {
  ILOCPHIDDENHANDLEMINI(IloVarSelector, IloVarSelectorI)
public:

  void end();
};


typedef IloArray<IloVarSelector> IloVarSelectorArray;


IloVarSelector IloSelectSmallest(IloIntVarEval eval);


IloVarSelector IloSelectSmallest(IloNum minNumber, IloIntVarEval eval);


IloVarSelector IloSelectSmallest(IloIntVarEval eval, IloNum tol);


IloVarSelector IloSelectLargest(IloIntVarEval eval);


IloVarSelector IloSelectLargest(IloNum minNumber, IloIntVarEval eval);


IloVarSelector IloSelectLargest(IloIntVarEval eval, IloNum tol);


IloVarSelector IloSelectRandomVar(IloEnv env);


class IloValueSelector {
  ILOCPHIDDENHANDLEMINI(IloValueSelector, IloValueSelectorI)
public:

  void end();
};


typedef IloArray<IloValueSelector> IloValueSelectorArray;


IloValueSelector IloSelectSmallest(IloIntValueEval eval);


IloValueSelector IloSelectSmallest(IloNum minNumber, IloIntValueEval eval);


IloValueSelector IloSelectSmallest(IloIntValueEval eval, IloNum tol);


IloValueSelector IloSelectLargest(IloIntValueEval eval);


IloValueSelector IloSelectLargest(IloNum minNumber, IloIntValueEval eval);


IloValueSelector IloSelectLargest(IloIntValueEval eval, IloNum tol);


IloValueSelector IloSelectRandomValue(IloEnv env);


class IloIntVarChooserI : public IloExtensibleRttiEnvObjectI {
 public:

  IloIntVarChooserI(IloEnv env) :
    IloExtensibleRttiEnvObjectI(env.getImpl()){}

  virtual IloInt choose(IloCP cp, IloIntVarArray x) = 0;
  
  virtual ~IloIntVarChooserI();
  ILORTTIDECL
};


class IloIntVarChooser {
  ILOCPVISIBLEHANDLEMINI(IloIntVarChooser, IloIntVarChooserI)
public:

  IloIntVarChooser(IloVarSelector varSel);

  IloIntVarChooser(IloVarSelectorArray varSelArray);

  IloIntVarChooser(IloEnv env, IloVarSelector varSel);

  IloIntVarChooser(IloEnv env, IloVarSelectorArray varSelArray);

  void end();
};


class IloIntValueChooserI : public IloExtensibleRttiEnvObjectI {
 public:

  IloIntValueChooserI(IloEnv env) :
    IloExtensibleRttiEnvObjectI(env.getImpl()){}

  virtual IloInt choose(IloCP cp, IloIntVarArray x, IloInt index) = 0;
  
  virtual ~IloIntValueChooserI();
  ILORTTIDECL
};


class IloIntValueChooser {
  ILOCPVISIBLEHANDLEMINI(IloIntValueChooser, IloIntValueChooserI)
public:

  IloIntValueChooser(IloValueSelector valueSel);

  IloIntValueChooser(IloValueSelectorArray valueSelArray);

  IloIntValueChooser(IloEnv env, IloValueSelector valueSel);

  IloIntValueChooser(IloEnv env, IloValueSelectorArray valueSelArray);

  void end();
};

////////////////////////////////////
//  IloSearchPhaseI


class IloSearchPhase {
  friend class  IlcStrategyManagerI;
  ILOCPHIDDENHANDLE(IloSearchPhase, IloSearchPhaseI)
 public:
  
  void end();
  
  IloSearchPhase(IloEnv env,
                 IloIntVarArray vars,
                 IloIntVarChooser varChooser,
                 IloIntValueChooser valueChooser);

  
  IloSearchPhase(IloEnv env,
                 IloIntVarArray vars);
  
  IloSearchPhase(IloEnv env,
                 IloIntVarChooser varChooser,
                 IloIntValueChooser valueChooser);

  
  IloSearchPhase(IloEnv env, IloIntervalVarArray intervalVars);

  
  IloSearchPhase(IloEnv env, IloIntervalSequenceVarArray sequenceVars);

};

// Undocumented:
IloSearchPhase IloFixPresenceSearchPhase(IloEnv env, IloIntervalVarArray intervalVars);


typedef IloArray<IloSearchPhase> IloSearchPhaseArray;


IloIntValueEval IloExplicitValueEval(IloEnv env,
                                     IloIntArray valueArray,
                                     IloIntArray evalArray,
                                     IloNum defaultEval = 0);

IloIntValueEval IloExplicitValueEval(IloEnv env,
                                     IloIntArray valueArray,
                                     IloNumArray evalArray,
                                     IloNum defaultValue = 0);

IloIntValueEval IloValueIndex(IloEnv env,
                              IloIntArray valueArray,
                              IloInt defaultEval = -1);

IloIntValueEval IloValue(IloEnv env);

IloIntValueEval IloValueImpact(IloEnv env);

IloIntValueEval IloValueSuccessRate(IloEnv env);

IloIntValueEval IloValueLocalImpact(IloEnv env);


IloIntValueEval IloValueLowerObjVariation(IloEnv env);

IloIntValueEval IloValueUpperObjVariation(IloEnv env);

IloIntVarEval IloVarIndex(IloEnv env, IloIntVarArray x, IloInt defaultEval = -1);

IloIntVarEval IloExplicitVarEval(IloEnv env, IloIntVarArray x, IloIntArray evalArray, IloNum defaultEval = 0);

IloIntVarEval IloExplicitVarEval(IloEnv env, IloIntVarArray x, IloNumArray evalArray, IloNum defaultEval = 0);

IloIntVarEval IloDomainMin(IloEnv env);

IloIntVarEval IloDomainMax(IloEnv env);

IloIntVarEval IloDomainSize(IloEnv env);

IloIntVarEval IloVarSuccessRate(IloEnv env);

IloIntVarEval IloVarImpact(IloEnv env);

IloIntVarEval IloVarLocalImpact(IloEnv env, IloInt effort = -1);

IloIntVarEval IloImpactOfLastBranch(IloEnv env);


IloIntVarEval IloRegretOnMin(IloEnv env);

IloIntVarEval IloRegretOnMax(IloEnv env);

IloIntVarEval IloVarLowerObjVariation(IloEnv env);

IloIntVarEval IloVarUpperObjVariation(IloEnv env);

////////////////////////////////////////////////////////////////////////
//
// ILOCP
//
////////////////////////////////////////////////////////////////////////
  

class IloCP : public IloAlgorithm {
private:
  void    _ctor(const IloModel model);
  void    _ctor(const IloEnv env);
  void    _abortSearch() const;
  void    _clearAbort() const;
  void    _clearLimit() const;
  void    _exitSearch() const;
  void    _fail(IloAny label) const;
  void    _freeze() const;
  void    _unfreeze() const;
  void    _getBounds(const IloIntVar var, IloInt& min, IloInt& max) const;
  IloInt  _getDegree(const IloIntVar var) const;
  IlcAllocationStack * _getHeap() const;
  IloNum  _getImpactOfLastAssignment(const IloIntVar var) const;
  IloNum  _getImpact(const IloIntVar var) const;
  IloNum  _getImpact(const IloIntVar var, IloInt value) const;
  IloNum  _getSuccessRate(const IloIntVar var) const;
  IloNum  _getSuccessRate(const IloIntVar var, IloInt value) const;
  IloNum  _getNumberOfFails(const IloIntVar var, IloInt value) const;
  IloNum  _getNumberOfInstantiations(const IloIntVar var, IloInt value) const;
  IloNum  _getLocalImpact(const IloIntVar var, IloInt value) const;
  IloNum  _getLocalVarImpact(const IloIntVar var, IloInt depth) const;
  IlcFloatArray _getFloatArray(IloNumArray arg) const;
  IlcIntArray _getIntArray(IloIntArray arg) const;
  IlcIntArray _getIntArray(IloNumArray arg) const;
  IloInt  _getMax(const IloIntVar var) const;
  IloNum  _getMax(const IloNumVar var) const;
  IloMemoryManager _getReversibleAllocator() const;
  IloMemoryManager _getSolveAllocator() const;
  IloMemoryManager _getPersistentAllocator() const;
  IloInt  _getMin(const IloIntVar var) const;
  IloNum  _getMin(const IloNumVar var) const;
  IlcAllocationStack * _getPersistentHeap() const;
  IlcAllocationStack * _getSolveHeap() const;
  IloInt  _getRandomInt(IloInt n) const;
  IloNum  _getRandomNum() const;
  IloInt  _getReduction(const IloIntVar var) const;
  IloNum  _getValue(const IloNumVar var) const;
  IloInt  _getValue(const IloIntVar var) const;
  const char * _getVersion() const;
  void _getObjValues(IloNumArray values) const;
  IloNum _getObjValue(IloInt index) const;
  IloNum _getObjMin() const;
  IloNum _getObjMin(IloInt index) const;
  IloNum _getObjMax() const;
  IloNum _getObjMax(IloInt index) const;
  IloInt _getNumberOfCriteria() const;
  IloBool _isFixed(const IloIntVar var) const;
  IloBool _isFixed(const IloNumVar var) const;
  void    _dumpModel(const char* filename) const;
  void    _dumpModel(ILOSTD(ostream)& s) const;
  void    _exportModel(const char* filename) const;
  void    _exportModel(ILOSTD(ostream)& s) const;
  void    _importModel(const char* filename) const;
  void    _importModel(ILOSTD(istream)& s) const;
  IloIntVarArray              _getAllIloIntVars() const;
  IloIntervalVarArray         _getAllIloIntervalVars() const;
  IloStateFunctionArray       _getAllIloStateFunctions() const;
  IloIntervalSequenceVarArray _getAllIloIntervalSequenceVars() const;
  IloCumulFunctionExprArray   _getAllConstrainedIloCumulFunctionExprs() const;
  IloInt _getValue(const char* intVarName) const;
  IloBool _isPresent(const char* intervalVarName) const;
  IloBool _isAbsent(const char* intervalVarName) const;
  IloInt _getStart(const char* intervalVarName) const;
  IloInt _getEnd(const char* intervalVarName) const;
  IloInt _getSize(const char* intervalVarName) const;
  IloInt _getLength(const char* intervalVarName) const;
  IloBool _isAllFixed() const;
  IloInt  _getDomainSize(const IloNumVar var) const;
  IloBool _isInDomain(const IloIntVar var, IloInt value) const;
  void     _printInformation() const;
  void     _printInformation(ILOSTD(ostream) & o) const;
  void     _printPortableInformation() const;
  void     _printPortableInformation(ILOSTD(ostream) & o) const;
  void     _printModelInformation() const;
  void     _printModelInformation(ILOSTD(ostream) & o) const;
  IloBool _propagate(const IloConstraint ct) const;
  void    _removeValueBuffered(IloNumVarI * var, IloInt value) const;
  void    _setMinBuffered(IloNumVarI * var, IloNum min) const;
  void    _setMaxBuffered(IloNumVarI * var, IloNum max) const;
  void    _failBuffered() const;
  void    _setInferenceLevel(IloConstraint ct, IloInt level) const;
  IloInt  _getInferenceLevel(IloConstraint ct) const;
  void    _resetConstraintInferenceLevels() const;
  void    _setSearchPhases() const;
  void    _setSearchPhases(const IloSearchPhase phase) const;
  void    _setSearchPhases(const IloSearchPhaseArray phaseArray) const;
  void    _setStartingPoint(const IloSolution ws) const;
  void    _clearStartingPoint() const;
  void    _clearExplanations();
  void    _explainFailure(IloInt failIndex);
  void    _explainFailure(IloIntArray failIndexArray);
  IloBool _solve(const IloSearchPhaseArray phaseArray) const;
  IloBool _solve(const IloSearchPhase phase) const;
  IloBool _solve() const;
  void    _startNewSearch(const IloSearchPhaseArray phaseArray) const;
  void    _startNewSearch(const IloSearchPhase phase) const;
  void    _startNewSearch() const;
  IloBool  _next() const;
  void    _endSearch() const;
  IloArray<IloConstraintArray> _findDisjointConflicts(IloInt conflictLimit) const;
  IloBool _isInReplay() const;
  void    _store(IloSolution solution) const;
  IloBool _restore(IloSolution solution) const;
  void _saveValue(IloAny * ptr) const;
  void _saveValue(IloNum * ptr) const;
  void _setNodeHook(IloCPHookI * hook) const;

  void _printDomain(ILOSTD(ostream)& s, const IloNumVar var) const;
  void _printDomain(ILOSTD(ostream)& s, const IloNumVarArray vars) const;
  void _printDomain(ILOSTD(ostream)& s, const IloIntVarArray vars) const;

  IloBool _isFixed(const IloCumulFunctionExpr cumul) const;
  IloInt _getNumberOfSegments(const IloCumulFunctionExpr cumul) const;
  IloBool _isValidSegment(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloInt _getSegmentStart(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloInt _getSegmentEnd(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloInt _getSegmentValue(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloBool _isValidAbscissa(const IloCumulFunctionExpr cumul, IloInt a) const;
  IloInt _getValue(const IloCumulFunctionExpr cumul, IloInt a) const;
  IloInt _getHeightAtFoo(const IloIntervalVar var, const IloCumulFunctionExpr cumul, IloBool atStart) const;

  IloNum _getNumberOfSegmentsAsNum(const IloCumulFunctionExpr cumul) const;
  IloNum _getSegmentStartAsNum(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloNum _getSegmentEndAsNum(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloNum _getSegmentValueAsNum(const IloCumulFunctionExpr cumul, IloInt s) const;
  IloNum _getValueAsNum(const IloCumulFunctionExpr cumul, IloInt a) const;
  
  IloBool _isFixed(const IloStateFunction f) const;
  IloInt _getNumberOfSegments(const IloStateFunction f) const;
  IloBool _isValidSegment(const IloStateFunction f, IloInt s) const;
  IloInt _getSegmentStart(const IloStateFunction f, IloInt s) const;
  IloInt _getSegmentEnd(const IloStateFunction f, IloInt s) const;
  IloInt _getSegmentValue(const IloStateFunction f, IloInt s) const;
  IloBool _isValidAbscissa(const IloStateFunction f, IloInt a) const;
  IloInt _getValue(const IloStateFunction f, IloInt a) const;

  IloNum _getNumberOfSegmentsAsNum(const IloStateFunction f) const;
  IloNum _getSegmentStartAsNum(const IloStateFunction f, IloInt s) const;
  IloNum _getSegmentEndAsNum(const IloStateFunction f, IloInt s) const;
  IloNum _getSegmentValueAsNum(const IloStateFunction f, IloInt s) const;
  IloNum _getValueAsNum(const IloStateFunction f, IloInt a) const;
  
  IloInt _getNumberOfSegments(const IloStateFunctionExpr expr) const;
  IloInt _getSegmentStart(const IloStateFunctionExpr expr, IloInt s) const;
  IloInt _getSegmentEnd(const IloStateFunctionExpr expr, IloInt s) const;
  IloInt _getSegmentValue(const IloStateFunctionExpr expr, IloInt s) const;
  IloInt _getValue(const IloStateFunctionExpr expr, IloInt a) const;

  IloBool _isAllExtracted(const IloExtractableArray ex) const;
  IloBool _isAllValid(const IloExtractableArray ex) const;
  IloBool _hasObjective() const;

  IlcManagerI * _getManagerI() const;

  static void _RegisterXML(IloEnv env);
public:
  ///////////////////////////////////////////////////////////////////////////
  // Parameters
  ///////////////////////////////////////////////////////////////////////////
  
  enum IntParam {
    
    
    
    DefaultInferenceLevel = 1,
    
    
    AllDiffInferenceLevel = 2,
    
    
    DistributeInferenceLevel = 3,
    
    
    CountInferenceLevel = 4,
    
    
    SequenceInferenceLevel = 5,
    
    
    AllMinDistanceInferenceLevel = 6,
    
    
    ElementInferenceLevel = 7,
    
    
    ConstraintAggregation = 8,
    
    
    FailLimit = 9,
    
    
    ChoicePointLimit = 10,
    
    
    LogVerbosity = 11,
    
    
    LogPeriod = 12,
    
    
    SearchType = 13,
    
    
    RandomSeed = 14,
    
    
    RestartFailLimit = 15,
    
    
    MultiPointNumberOfSearchPoints = 16,
    
    
    ImpactMeasures = 18,
    
    
    PackApproximationSize = 22,
    
    
    StrictNumericalDivision = 23,
    
    
    FloatDisplay = 24,
    
    
    Workers = 25,
    
    
    PropagationLog = 27,
    
    
    BranchLimit = 28,
    
    
    AutomaticReplay = 29,
    
    
    SeedRandomOnSolve = 30,
    
    
    TraceExtraction = 31,
    
    
    DynamicProbing = 32,
    
    
    ConflictLimit = 33,
    
    
    TimeDisplay = 34,
    
    
    SolutionLimit = 35,
    
    
    PresolveLevel = 36,
    
    
    PrecedenceInferenceLevel = 38,
    
    
    IntervalSequenceInferenceLevel = 39,
    
    
    NoOverlapInferenceLevel = 40,
    
    
    CumulFunctionInferenceLevel = 41,
    
    
    StateFunctionInferenceLevel = 42,
    
    
    TimeMode = 43,
    
    
    TemporalRelaxation = 44,
    
    
    TemporalRelaxationLevel = 45,
    
    
    TemporalRelaxationRowLimit = 46,
    
    
    TemporalRelaxationIterationLimit = 47,
    
    
    SearchConfiguration = 48,
    
    
    SequenceExpressionInferenceLevel = 50,
    
    
    StateFunctionTriangularInequalityCheck = 51,
    
    
    TemporalRelaxationUsesEnergyEnvelopes = 52,
    
    
    IntervalSolutionPoolCapacity = 53,
    
    
    TTEF = 54,
    
    
    TemporalRelaxationAlgorithm = 55,
    
    
    RestartTimeMeasurement = 56,
    
    
    MultiPointMaximumFailLimit = 57,
    
    
    MultiPointDisableLamarckism = 58,
    
    
    ParallelWorkerStackSize = 59,
    
    
    Presolve = 60,
    
    
    ConflictRefinerIterationLimit = 61,
    
    
    ConflictRefinerBranchLimit = 62,
    
    
    ConflictRefinerFailLimit = 63,
    
    
    ConflictRefinerOnVariables = 64,
    
    
    ConflictRefinerOnLabeledConstraints = 65,
    
    
    ConflictRefinerAlgorithm = 66,
    
    
    TimetablingAlgorithm = 67,
    
    
    PresolveTransformers = 68,
    
    
    EliminatePresolvedModel = 69,
    
    
    ParallelMode = 70,
    
    
    ParallelSynchronizationTicks = 71,
    
    
    ParallelCommunicateSolutions = 72,
    
    
    ParallelCommunicateEachSolution = 73,
    
    
    ParallelEventQueueCapacity = 74,
    
    
    LogDisplayWorkerIdleTime = 75,
    
    
    StrongMaxTuples = 76,
    
    
    ModelAnonymizer = 77,
    
    
    FailureDirectedSearch = 78,
    
    
    FailureDirectedSearchMaxMemory = 79,
    
    
    SynchronizeByDeterministicTime = 80,
    
    
    MemoryDisplay = 81,
    
    
    MultiPointEncodeIntervalPrecedences = 82,
    
    
    MultiPointEncodeIntervalSequences = 83,
    
    
    MultiPointEncodeIntervalAlternatives = 84,
    
    
    MultiPointEncodeIntervalExecutions = 85,
    
    
    MultiPointEncodeIntervalTimes = 86,
    
    
    MultiPointEncodeObjectives = 87,
    
    
    ParallelOptimizeSingleWorker = 88,
    
    
    ParallelSkipEquivalentSolutions = 89,
    
    
    WarningLevel = 90,
    
    
    MultiPointWeightAlleleFactories = 91,
    
    
    MultiPointUseApproximateAssignment = 92,
    
    
    CPOFileCompatibility = 93,
    
    
    MultiPointUseRandomOperator = 94,
    
    
    MultiPointUseCrossoverOperator = 95,
    
    
    MultiPointUseMutationOperator = 96,
    
    
    MultiPointReduceIntervalPrecedences = 97,
    
    
    MultiPointIgnorePhases = 98,
    
    
    MultiPointConsiderSecondaryVariables = 99,
    
    
    UseFileLocations = 100,
    
    
    ConflictRefinerWriteMode = 101,
    
    
    ConflictDefinition = 102,
    
    
    MaxPBTCaptureSize = 103,
    
    
    CountDifferentInferenceLevel = 104,
    
    
    MultiPointImproveDuringInit = 105,
    
    
    PresolveIterations = 106,
    
    
    LogSearchTags = 107,
    
    
    MinPBTCaptureSize = 108,
    
    
    PrintModelDetailsInMessages = 109,
    
    
    ForbidIncludeDirective = 110,
    
    ILC_MAX_IntParam
  };
  
  enum NumParam {
    
    
    
    MultiPointEncodingPercentage = 17,
    
    
    ObjectiveLimit = 37,
    
    
    OptimalityTolerance = 1001,
    
    
    RelativeOptimalityTolerance = 1002,
    
    
    TimeLimit = 1003,
    
    
    RestartGrowthFactor = 1004,
    
    
    DynamicProbingStrength = 1005,
    
    
    LowerBoundEffort = 1006,
    
    
    RestartProofEmphasis = 1007,
    
    
    MultiPointPropagationLimitFactor = 1008,
    
    
    RestartPropagationLimitFactor = 1009,
    
    
    ParallelRestartProp = 1010,
    
    
    MultiPointLearningRatio = 1011,
    
    
    ConflictRefinerTimeLimit = 1012,
    
    
    TemporalRelaxationTimeFactor = 1013,
    
    
    StrongMinReduction = 1014,
    
    
    MultiPointRandomOperatorEncodingFactor = 1015,
    
    
    MultiPointRestartProbability = 1016,
    
    
    MultiPointRestartRatio = 1017,
    
    
    FailureDirectedSearchEmphasis = 1018,
    
    ILC_MAX_NumParam
  };
  
  enum IntInfo {
    
    
    
    NumberOfChoicePoints = 1,
    
    
    NumberOfFails = 2,
    
    
    NumberOfBranches = 3,
    
    
    NumberOfModelVariables = 4,
    
    
    NumberOfAuxiliaryVariables = 5,
    
    
    NumberOfVariables = 6,
    
    
    NumberOfConstraints = 7,
    
    
    MemoryUsage = 8,
    
    
    NumberOfConstraintsAggregated = 9,
    
    
    NumberOfConstraintsGenerated = 10,
    
    
    FailStatus = 11,
    
    
    NumberOfIntegerVariables = 12,
    
    
    NumberOfIntervalVariables = 13,
    
    
    NumberOfSequenceVariables = 14,
    
    
    NumberOfSolutions = 15,
    
    
    EffectiveWorkers = 16,
    
    
    EffectiveDepthFirstWorkers = 17,
    
    
    EffectiveMultiPointWorkers = 18,
    
    
    EffectiveRestartWorkers = 19,
    
    
    NumberOfPresolveTransformations = 20,
    
    
    NumberOfConstraintsAdded = 21,
    
    
    NumberOfConstraintsRemoved = 22,
    
    
    NumberOfCriteria = 23,
    
    ILC_MAX_IntInfo
  };
  
  enum NumInfo {
    
    
    
    SolveTime = 1001,
    
    
    ExtractionTime = 1002,
    
    
    TotalTime = 1003,
    
    
    EffectiveOptimalityTolerance = 1004,
    
    
    DepthFirstIdleTime = 1005,
    
    
    RestartIdleTime = 1006,
    
    
    MultiPointIdleTime = 1007,
    
    
    NumberOfWorkerSynchronizations = 1008,
    
    
    PresolveTime = 1009,
    
    ILC_MAX_NumInfo
  };
  
  enum ParameterValues {
    
    
    Auto = -1,
    
    Off = 0,
    
    On = 1,
    
    Default = 2,
    
    Low = 3,
    
    Basic = 4,
    
    Medium = 5,
    
    Extended = 6,
    
    Standard = 7,
    
    IntScientific = 8,
    
    IntFixed = 9,
    
    BasScientific = 10,
    
    BasFixed = 11,
    
    SearchHasNotFailed = 12,
    
    SearchHasFailedNormally = 13,
    
    SearchStoppedByLimit = 14,
    
    SearchStoppedByLabel = 15,
    
    SearchStoppedByExit = 16,
    
    SearchStoppedByAbort = 17,
    
    SearchStoppedByException = 18,
    
    UnknownFailureStatus = 19,
    
    Quiet = 20,
    
    Terse = 21,
    
    Normal = 22,
    
    Verbose = 23,
    
    DepthFirst = 24,
    
    Restart = 25,
    
    MultiPoint = 26,
    
    Diverse = 27,
    
    Focused = 28,
    
    Intensive = 29,
    
    Seconds = 30,
    
    HoursMinutesSeconds = 31,
    
    NoTime = 32,
    
    CPUTime = 33,
    
    ElapsedTime = 34,
    
    Infeasible = 40,
    
    Hard = 41,
    
    ComplementaryFeasible = 42,
    
    ILC_MAX_ParameterValues
  };

private:
  void   _setParameter(IloInt param, IloNum value) const;
  void   _setParameter(const char * name, IloNum value) const;
  void   _setParameter(const char * name, const char * value) const;

  IloNum _getParameter(IloInt param) const;
  IloNum _getParameter(const char * name) const;

  IloNum _getParameterDefault(IloInt param) const;
  IloNum _getParameterDefault(const char * name) const;

  IloNum _getInfo(IloInt info) const;
  IloNum _getInfo(const char * name) const;
public:
  
  void setIntParameter(IloCP::IntParam param, IloNum value) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _setParameter(IloInt(param), value);
  }
  
  IloNum getIntParameter(IloCP::IntParam param) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _getParameter(IloInt(param));
  }
  
  IloNum getIntParameterDefault(IloCP::IntParam param) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _getParameterDefault(IloInt(param));
  }
  
  IloNum getIntInfo(IloCP::IntInfo info) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _getInfo(IloInt(info));
  }

public:
  
  void setParameter(IloCP::IntParam param, IloInt value) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _setParameter(IloInt(param), (IloNum)value);
  }
  
  IloInt getParameter(IloCP::IntParam param) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return IloInt(_getParameter(IloInt(param)));
  }
  
  IloInt getParameterDefault(IloCP::IntParam param) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return IloInt(_getParameterDefault(IloInt(param)));
  }
  
  void setParameter(IloCP::NumParam param, IloNum value) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _setParameter(IloInt(param), value);
  }
  
  void setParameter(const char * name, IloNum value) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(name  != 0, "IloCP::setParameter - empty name");
    _setParameter(name, value);
  }
  
  void setParameter(const char * name, const char * value) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(name  != 0, "IloCP::setParameter - empty name");
    IlcCPOAssert(value != 0, "IloCP::setParameter - empty value");
    _setParameter(name, value);
  }
  
  IloNum getParameter(IloCP::NumParam param) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _getParameter(IloInt(param));
  }
  
  IloNum getParameter(const char * name) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(name  != 0, "IloCP::getParameter - empty name");
    return _getParameter(name);
  }
  
  IloNum getParameterDefault(IloCP::NumParam param) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _getParameterDefault(IloInt(param));
  }
  IloNum getParameterDefault(const char * name) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(name  != 0, "IloCP::getParameterDefault - empty name");
    return _getParameterDefault(name);
  }

  
  IloInt getInfo(IloCP::IntInfo info) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return IloInt(_getInfo(IloInt(info)));
  }
  
  IloNum getInfo(IloCP::NumInfo info) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _getInfo(IloInt(info));
  }
  IloNum getInfo(const char * name) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(name  != 0, "IloCP::getNumInfo - empty name");
    return _getInfo(name);
  }

  ///////////////////////////////////////////////////////////////////////////
  // Constructors, extraction and related methods
  ///////////////////////////////////////////////////////////////////////////

  IloCP(const IloEnv env) {
    IlcCPOAssert(env.getImpl() != 0, "IloEnv: empty handle");
    _ctor(env);
  }

  IloCP(const IloModel model) {
    IlcCPOAssert(model.getImpl() != 0, "IloModel: empty handle");
    _ctor(model);
  }
#ifdef CPPREF_GENERATION

void extract (const IloModel model) const;

 IloBool isExtracted(const IloExtractable ext) const;

 void end();
#endif
  
  IloCPI * getImpl() const {
    return (IloCPI*)_impl;
  }
  
  IloCP(IloCPI * impl=0) : IloAlgorithm((IloAlgorithmI *)impl) { }

  IloBool isAllExtracted(const IloExtractableArray ext) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(ext.getImpl() != 0, "IloExtractableArray: empty handle");
    return _isAllExtracted(ext);
  }
  IloBool isAllValid(const IloExtractableArray ext) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(ext.getImpl() != 0, "IloExtractableArray: empty handle");
    return _isAllValid(ext);
  }
  
  IloBool hasObjective() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _hasObjective();
  }
  ///////////////////////////////////////////////////////////////////////////
  // Solving
  ///////////////////////////////////////////////////////////////////////////
  
  void setSearchPhases(){
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _setSearchPhases();
  }
  
  void setSearchPhases(IloSearchPhase phase){
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(phase.getImpl() != 0, "IloSearchPhase: empty handle");
    return _setSearchPhases(phase);
  }
  
  void setSearchPhases(IloSearchPhaseArray phaseArray){
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(phaseArray.getImpl() != 0, "IloSearchPhaseArray: empty handle");
    return _setSearchPhases(phaseArray);
  }

  
  void setStartingPoint(const IloSolution sp) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(sp.getImpl() != 0, "IloSolution: empty handle");
    _setStartingPoint(sp);
  }

  
  void clearStartingPoint() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _clearStartingPoint();
  }
  
  void clearExplanations(){
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _clearExplanations();
  }
  
  void explainFailure(IloInt failIndex){
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _explainFailure(failIndex);
  }
  
  void explainFailure(IloIntArray failIndexArray){
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(failIndexArray.getImpl() != 0, "IloIntArray: empty handle");
    _explainFailure(failIndexArray);
  }

  
  IloBool solve(const IloGoal goal) const;
  
  IloBool solve() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _solve();
  }
  
  IloBool solve(const IloSearchPhaseArray phaseArray) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(phaseArray.getImpl() != 0, "IloSearchPhaseArray: empty handle");
    return _solve(phaseArray);
  }
  
  IloBool solve(const IloSearchPhase phase) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(phase.getImpl() != 0, "IloSearchPhase: empty handle");
    return _solve(phase);
  }
  
  void startNewSearch(const IloGoal goal) const;
   
  void startNewSearch() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _startNewSearch();
  }
  
  void startNewSearch(const IloSearchPhaseArray phaseArray) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(phaseArray.getImpl() != 0, "IloSearchPhaseArray: empty handle");
    _startNewSearch(phaseArray);
  }
  
  void startNewSearch(const IloSearchPhase phase) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(phase.getImpl() != 0, "IloSearchPhase: empty handle");
    _startNewSearch(phase);
  }
  
  IloBool next() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _next();
  }
  IloBool isInReplay() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _isInReplay();
  }
  
  void endSearch() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _endSearch();
  }

  ///////////////////////////////////////////////////////////////////////////
  // Model manipulation
  ///////////////////////////////////////////////////////////////////////////

  
  void dumpModel(const char* filename) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(filename != 0, "IloCP::dumpModel: empty file name ");
    _dumpModel(filename);
  }
  
  void dumpModel(ILOSTD(ostream)& s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _dumpModel(s);
  }
  
  void exportModel(const char* filename) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(filename != 0, "IloCP::exportModel: empty file name ");
    _exportModel(filename);
  }
  
  void exportModel(ILOSTD(ostream)& s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _exportModel(s);
  }

  
  void importModel(const char* filename) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(filename != 0, "IloCP::AddModel: empty file name");
    _importModel(filename);
  }

  
  void importModel(ILOSTD(istream)& s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _importModel(s);
  }

  
  IloIntVarArray getAllIloIntVars() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getAllIloIntVars();
  }

  
  IloIntervalVarArray getAllIloIntervalVars() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getAllIloIntervalVars();
  }

  
  IloStateFunctionArray getAllIloStateFunctions() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getAllIloStateFunctions();
  }

  
  IloIntervalSequenceVarArray getAllIloIntervalSequenceVars() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getAllIloIntervalSequenceVars();
  }

  
  IloCumulFunctionExprArray getAllConstrainedIloCumulFunctionExprs() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getAllConstrainedIloCumulFunctionExprs();
  }

  
  IloIntVar getIloIntVar(const char* name) const;

  
  IloIntervalVar getIloIntervalVar(const char* name) const;

  
  IloIntervalSequenceVar getIloIntervalSequenceVar(const char* name) const;

  
  IloStateFunction getIloStateFunction(const char* name) const;

  
  IloCumulFunctionExpr getIloCumulFunctionExpr(const char* name) const;

  
  IloInt getValue(const char* intVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::getValue: empty handle.");
    IlcCPOAssert(intVarName, "IloCP::getValue: null pointer argument.");
    return _getValue(intVarName);
  }

  
  IloBool isPresent(const char* intervalVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::isPresent: empty handle.");
    IlcCPOAssert(intervalVarName, "IloCP::isPresent: null pointer argument.");
    return _isPresent(intervalVarName);
  }

  
  IloBool isAbsent(const char* intervalVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::isAbsent: empty handle.");
    IlcCPOAssert(intervalVarName, "IloCP::isAbsent: null pointer argument.");
    return _isAbsent(intervalVarName);
  }

  
  IloInt getStart(const char* intervalVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::getStart: empty handle.");
    IlcCPOAssert(intervalVarName, "IloCP::getStart: null pointer argument.");
    return _getStart(intervalVarName);
  }

  
  IloInt getEnd(const char* intervalVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::getEnd: empty handle.");
    IlcCPOAssert(intervalVarName, "IloCP::getEnd: null pointer argument.");
    return _getEnd(intervalVarName);
  }

  
  IloInt getSize(const char* intervalVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::getSize: empty handle.");
    IlcCPOAssert(intervalVarName, "IloCP::getSize: null pointer argument.");
    return _getSize(intervalVarName);
  }

  
  IloInt getLength(const char* intervalVarName) const {
    IlcCPOAssert(getImpl() != 0, "IloCP::getLength: empty handle.");
    IlcCPOAssert(intervalVarName, "IloCP::getLength: null pointer argument.");
    return _getLength(intervalVarName);
  }

  
  IloModel createDummyConcertModel() const;
  
  void setLocation(IloExtractable e, const char* fileName, int lineNumber) const;

  ///////////////////////////////////////////////////////////////////////////
  // Conflict refiner
  ///////////////////////////////////////////////////////////////////////////
  
  
  enum ConflictStatus {
    
    ConflictPossibleMember =0,
    
    ConflictMember =1,
    
    ConflictExcluded =2
  };
  
  IloBool refineConflict() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _refineConflict();
  }
  
  IloBool refineConflict(IloConstraintArray csts) {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(csts.getImpl() != 0, "IloConstraintArray: empty handle");
    return _refineConflict(csts);
  }
  
  IloBool refineConflict(IloConstraintArray csts, IloNumArray prefs) {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(csts.getImpl() != 0, "IloConstraintArray: empty handle");
    IlcCPOAssert(prefs.getImpl() != 0, "IloNumArray: empty handle");
    IlcCPOAssert(csts.getSize()==prefs.getSize(), "IloCP::refineConflict: constraint and preference arrays have different size");
    return _refineConflict(csts, prefs);
  }
  
  IloBool refineConflict(IloSolution sol) {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(sol.getImpl() != 0, "IloSolution: empty handle");
    return _refineConflict(sol);
  }
  
  IloCP::ConflictStatus getConflict(IloConstraint cst) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(cst.getImpl() != 0, "IloConstraint: empty handle");
    IlcCPOAssert(_hasConflict(), "IloCP::getConflict: no available conflict");
    return _getConflict(cst);
  }
  
  IloCP::ConflictStatus getConflict(IloNumVar var) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(_hasConflict(), "IloCP::getConflict: no available conflict");
    return _getConflict(var);
  }
  
  IloCP::ConflictStatus getConflict(IloIntervalVar var) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntervalVar: empty handle");
    IlcCPOAssert(_hasConflict(), "IloCP::getConflict: no available conflict");
    return _getConflict(var);
  }
  
  IloInt getIntConflict(IloConstraint cst) const {
    return (IloInt)getConflict(cst);
  }
  
  IloInt getIntConflict(IloNumVar var) const {
    return (IloInt)getConflict(var);
  }
  
  IloInt getIntConflict(IloIntervalVar var) const {
    return (IloInt)getConflict(var);
  }
  
  typedef IloArray<IloCP::ConflictStatus> ConflictStatusArray;
  void getConflictArray(IloConstraintArray& csts, IloCP::ConflictStatusArray& statuses) const {
    IlcCPOAssert(csts.getImpl() != 0, "IloConstraintArray: empty handle");
    IlcCPOAssert(statuses.getImpl() != 0, "IloCP::ConflictStatusArray: empty handle");
    _getConflictArray(csts, statuses);
  }
  
  
  void writeConflict(ILOSTD(ostream)& str) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(_hasConflict(), "IloCP::writeConflict: no available conflict");
    return _writeConflict(str);
  }

  
  void exportConflict(ILOSTD(ostream)& str) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(_hasConflict(), "IloCP::exportConflict: no available conflict");
    return _exportConflict(str);
  }

private:
  IloBool _refineConflict() const;
  IloBool _refineConflict(IloConstraintArray csts) const;
  IloBool _refineConflict(IloConstraintArray csts, IloNumArray prefs) const;
  IloBool _refineConflict(IloSolution sol) const;
  IloCP::ConflictStatus _getConflict(IloConstraint cst) const;
  IloCP::ConflictStatus _getConflict(IloNumVar var) const;
  IloCP::ConflictStatus _getConflict(IloIntervalVar var) const;
  void _getConflictArray(IloConstraintArray& csts, IloCP::ConflictStatusArray& statuses) const;
  void _writeConflict(ILOSTD(ostream)& str) const;
  void _exportConflict(ILOSTD(ostream)& str) const;
  IloBool _hasConflict() const;
  
public:
  IloArray<IloConstraintArray> findDisjointConflicts(IloInt limit = IloIntMax) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(limit >= 0, "IloCP::findDisjointConflicts - conflict limit is negative");
    return _findDisjointConflicts(IloMax(limit, 1));
  }
  
  IloBool propagate(const IloConstraint constraint = 0) {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    return _propagate(constraint);
  }
  
  void store(IloSolution solution) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(solution.getImpl() != 0, "IloSolution: empty handle");
    _store(solution);
  }
  
  IloBool restore(IloSolution solution) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(solution.getImpl() != 0, "IloSolution: empty handle");
    return _restore(solution);
  }
  
  void printInformation() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _printInformation();
  }
  
  void printInformation(ILOSTD(ostream)& stream) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _printInformation(stream);
  }

  void printPortableInformation() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _printPortableInformation();
  }
  void printPortableInformation(ILOSTD(ostream)& stream) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _printPortableInformation(stream);
  }

  void printModelInformation() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _printModelInformation();
  }
  void printModelInformation(ILOSTD(ostream)& stream) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _printModelInformation(stream);
  }

  void printDomain(const IloNumVar var) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloNumVar: not extracted");
    _printDomain(out(), var);
  }
  void printDomain(ILOSTD(ostream)& s, const IloNumVar var) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloNumVar: not extracted");
    _printDomain(s, var);
  }
  void printDomain(ILOSTD(ostream)& s, const IloNumVarArray vars) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(vars.getImpl() != 0, "IloNumVarArray: empty handle");
    IlcCPOAssert(isAllValid(vars), "IloNumVarArray: empty element handle");
    IlcCPOAssert(isAllExtracted(vars), "IloNumVarArray: element not extracted");
    _printDomain(s, vars);
  }
  void printDomain(const IloNumVarArray vars) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(vars.getImpl() != 0, "IloNumVarArray: empty handle");
    IlcCPOAssert(isAllValid(vars), "IloNumVarArray: empty element handle");
    IlcCPOAssert(isAllExtracted(vars), "IloNumVarArray: element not extracted");
    _printDomain(out(), vars);
  }
  void printDomain(ILOSTD(ostream)& s, const IloIntVarArray vars) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(vars.getImpl() != 0, "IloIntVarArray: empty handle");
    IlcCPOAssert(isAllValid(vars), "IloIntVarArray: empty element handle");
    IlcCPOAssert(isAllExtracted(vars), "IloIntVarArray: element not extracted");
    _printDomain(s, vars);
  }
  void printDomain(const IloIntVarArray vars) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(vars.getImpl() != 0, "IloIntVarArray: empty handle");
    IlcCPOAssert(isAllValid(vars), "IloIntVarArray: empty element handle");
    IlcCPOAssert(isAllExtracted(vars), "IloIntVarArray: element not extracted");
    _printDomain(out(), vars);
  }

  class PrintDomains {
  protected:
    const void*        _cp;
    IloInt             _n;
    IloExtractableI ** _var;

    PrintDomains() { }
    PrintDomains(const IloCP cp, const IloExtractable ext);
    PrintDomains(const IloCP cp, const IloExtractableArray ext);
    PrintDomains(const PrintDomains &);
  public:
    ~PrintDomains();
  };

  class PrintNumVarDomains : public PrintDomains {
    friend class IloCP;
  private:
    void operator = (const PrintNumVarDomains &);

    PrintNumVarDomains(const IloCP cp, const IloNumVar var);
    PrintNumVarDomains(const IloCP cp, const IloNumVarArray var);
    PrintNumVarDomains(const IloCP cp, const IloIntVarArray var);
  public:
    void display(ILOSTD(ostream)& o) const;
  };


  
  PrintNumVarDomains domain(const IloNumVar var) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloNumVar: not extracted");
    return PrintNumVarDomains(*this, var);
  }
  
  PrintNumVarDomains domain(const IloNumVarArray vars) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(vars.getImpl() != 0, "IloNumVarArray: empty handle");
    IlcCPOAssert(isAllValid(vars), "IloNumVarArray: empty element handle");
    IlcCPOAssert(isAllExtracted(vars), "IloNumVarArray: element not extracted");
    return PrintNumVarDomains(*this, vars);
  }
  
  PrintNumVarDomains domain(const IloIntVarArray vars) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(vars.getImpl() != 0, "IloIntVarArray: empty handle");
    IlcCPOAssert(isAllValid(vars), "IloIntVarArray: empty element handle");
    IlcCPOAssert(isAllExtracted(vars), "IloIntVarArray: element not extracted");
    return PrintNumVarDomains(*this, vars);
  }

  ///////////////////////////////////////////////////////////////////////////
  // Hooks
  ///////////////////////////////////////////////////////////////////////////
  void setNodeHook(IloCPHookI * hook = 0) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    _setNodeHook(hook);
  }

  ///////////////////////////////////////////////////////////////////////////
  // Getting solution information
  ///////////////////////////////////////////////////////////////////////////
  // Mimic IloAlgorithm as CP Optimizer has its own getValue functions
  
  void getObjValues(IloNumArray values) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(values.getImpl() != 0, "IloNumArray: empty handle");
    return _getObjValues(values);
  }
  
  IloInt getNumberOfCriteria() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getNumberOfCriteria();
  }
  
  IloNum getObjValue(IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(i >= 0, "IloCP: Objective value index is negative");
    IlcCPOAssert(i < _getNumberOfCriteria(),
              "IloCP: Objective value index is too large");
    return _getObjValue(i);
  }
  IloNum getObjValue() const { return IloAlgorithm::getObjValue(); }
  IloNum getObjMin() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(_hasObjective(), "IloCP: No objective present");
    return _getObjMin();
  }
  IloNum getObjMin(IloInt i) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(_hasObjective(), "IloCP: No objective present");
    IlcCPOAssert(i >= 0, "IloCP: Objective value index is negative");
    IlcCPOAssert(i < _getNumberOfCriteria(),
              "IloCP: Objective value index is too large");
    return _getObjMin(i);
  }
  IloNum getObjMax() const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(_hasObjective(), "IloCP: No objective present");
    return _getObjMax();
  }
  IloNum getObjMax(IloInt i) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(_hasObjective(), "IloCP: No objective present");
    IlcCPOAssert(i >= 0, "IloCP: Objective value index is negative");
    IlcCPOAssert(i < _getNumberOfCriteria(),
              "IloCP: Objective value index is too large");
    return _getObjMax(i);
  }

  
  IloNum getValue(const IloObjective obj) const {
    return IloAlgorithm::getValue(obj);
  }
  
  IloNum getValue(const IloNumExprArg expr) const {
    return IloAlgorithm::getValue(expr);
  }

  
  IloNum getValue(const IloNumVar v) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(v.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(v), "IloNumVar: not extracted");
    IlcCPOAssert(isFixed(v), "IloNumVar: not fixed");
    return _getValue(v);
  }
  
  IloInt getValue(const IloIntVar v) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(v.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(v), "IloIntVar: not extracted");
    IlcCPOAssert(isFixed(v), "IloIntVar: not fixed");
    return _getValue(v);
  }
  // 2.0b1
  IloAny getAnyValue(const IloIntVar v) const { return (IloAny)getValue(v); }
  
  IloNum getMin(const IloNumVar v) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(v.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(v), "IloNumVar: not extracted");
    return _getMin(v);
  }
  
  IloNum getMax(const IloNumVar v) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(v.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(v), "IloNumVar: not extracted");
    return _getMax(v);
  }
  
  IloInt getMax(const IloIntVar var) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloIntVar: not extracted");
    return _getMax(var);
  }
  
  IloInt getMin(const IloIntVar var) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloIntVar: not extracted");
    return _getMin(var);
  }
  void getBounds(const IloIntVar var, IloInt& min, IloInt& max) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloIntVar: not extracted");
    _getBounds(var, min, max);
  }
   
  IloBool isInDomain(const IloNumVar var, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloNumVar: not extracted");
    IlcCPOAssert(var.getType() != ILOFLOAT, "IloNumVar: not integer");
    return _isInDomain(IloIntVar(var.getImpl()), value);
  }
  
  IloBool isInDomain(const IloIntVar var, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloIntVar: not extracted");
    return _isInDomain(var, value);
  }
  
  IloInt getDomainSize(const IloNumVar var) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNUmVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloNumVar: not extracted");
    IlcCPOAssert(var.getType() != ILOFLOAT, "IloNumVar: not integer");
    return _getDomainSize(var);
  }
  
  IloBool isFixed(const IloNumVar var) const  {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloNumVar: not extracted");
    return _isFixed(var);
  }
  
  IloBool isFixed(const IloIntVar var) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(var), "IloIntVar: not extracted");
    return _isFixed(var);
  }
  IloBool isAllFixed() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _isAllFixed();
  }

  
  class IntVarIterator {
  private:
    void*        _var;
    IloInt       _curr;
    IloBool      _ok;

    void _init(IloCP cp, IloIntVar var);
  public:
    IntVarIterator() : _var(0) {}
    
    IntVarIterator(IloCP cp, IloIntVar var) {
      IlcCPOAssert(cp.getImpl() != 0, "IloCP: empty handle");
      IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
      _init(cp, var);
    }
    
    IntVarIterator(IloCP cp, IloNumVar var) {
      IlcCPOAssert(cp.getImpl() != 0, "IloCP: empty handle");
      IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
      IlcCPOAssert(var.getType() != ILOFLOAT, "IloNumVar: not integer");
      _init(cp, IloIntVar(var.getImpl()));
    }
    
    IntVarIterator& operator++();
    
    IloInt operator*() const { return _curr; }
    // 2.0b1
    IloAny getAnyValue() const { return (IloAny)_curr; }
    
    IloBool ok() const { return _ok; }
  };
  
  IloCP::IntVarIterator iterator(IloIntVar var) {
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    return IloCP::IntVarIterator(*this, var);
  }
  
  IloCP::IntVarIterator iterator(IloNumVar var) {
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    IlcCPOAssert(var.getType() != ILOFLOAT, "IloNumVar: not integer");
    return IloCP::IntVarIterator(*this, var);
  }

  ///////////////////////////////////////////////////////////////////////////
  // Search information
  ///////////////////////////////////////////////////////////////////////////
  
  IloInt getReduction(const IloIntVar x) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getReduction(x);
  }
  
  IloNum getImpactOfLastAssignment(const IloIntVar x) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getImpactOfLastAssignment(x);
  }
  
  IloNum getImpact(const IloIntVar x) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getImpact(x);
  }
  
  IloNum getImpact(const IloIntVar x, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getImpact(x, value);
  }
  
  IloNum getSuccessRate(const IloIntVar x) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getSuccessRate(x);
  }
  
  IloNum getSuccessRate(const IloIntVar x, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getSuccessRate(x, value);
  }
  IloNum getNumberOfFails(const IloIntVar x, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getNumberOfFails(x, value);
  }
  IloNum getNumberOfInstantiations(const IloIntVar x, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getNumberOfInstantiations(x, value);
  }
  IloNum getLocalImpact(const IloIntVar x, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getLocalImpact(x, value);
  }
  IloNum getLocalVarImpact(const IloIntVar x, IloInt depth) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(x.getImpl() != 0, "IloIntVar: empty handle");
    IlcCPOAssert(isExtracted(x), "IloIntVar: not extracted");
    return _getLocalVarImpact(x, depth);
  }

  ///////////////////////////////////////////////////////////////////////////
  // Services
  ///////////////////////////////////////////////////////////////////////////
  
  IloInt getRandomInt(IloInt n) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(n >= 1, "IloCP::getRandomInt(n): n < 1");
    return _getRandomInt(n);
  }
  
  IloNum getRandomNum() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getRandomNum();
  }
  
  const char* getVersion() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getVersion();
  }
  static const char* GetVersion();

  ///////////////////////////////////////////////////////////////////////////
  // Advanced: Ilc mapping
  //           No inlining here to avoid Ilc/Ilo world crossover
  ///////////////////////////////////////////////////////////////////////////
  
  IlcIntVar getIntVar(const IloNumVar var) const;
  IlcIntVar getIntVar(const IloIntVar var) const;
  IlcIntVar getIntVar(const IloNumVarI* var) const;
  
  IlcIntervalVar getInterval(const IloIntervalVar var) const;
  
  IlcCumulElementVar getCumulElement(const IloCumulFunctionExpr f) const;
  
  IlcIntervalSequenceVar getIntervalSequence(const IloIntervalSequenceVar s) const;
  
  IlcIntArray getIntArray(const IloNumArray arg) const;
   
  IlcIntArray getIntArray(const IloIntArray arg) const;
  
  IlcFloatArray getFloatArray(const IloNumArray arg) const;
  
  IlcIntSet getIntSet(const IloIntSet arg) const;
  
  IlcIntSet getIntSet(const IloNumSet arg) const;
  
  IlcFloatVar getFloatVar(const IloNumVar var) const;
  
  IlcIntVarArray getIntVarArray(const IloIntVarArray vars) const;
  
  IlcIntVarArray getIntVarArray(const IloNumVarArray vars) const;
  IlcIntVarArray getIntVarArray(const IloIntExprArray exps) const;
   
  IlcFloatVarArray getFloatVarArray(const IloNumVarArray vars) const;

  
  ILCDEPRECATED IlcIntExp getIntExp(const IloIntExprArg expr) const;
  
  ILCDEPRECATED IlcFloatExp getFloatExp(const IloNumExprArg expr) const;
  
  IlcIntTupleSet getIntTupleSet(const IloIntTupleSet ts) const;

  ///////////////////////////////////////////////////////////////////////////
  // Advanced
  ///////////////////////////////////////////////////////////////////////////
  
  void startNewSearch(const IlcGoal goal) const;
  
  void fail(IloAny label=0) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _fail(label);
  }
  void freeze() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _freeze();
  }
  void unfreeze() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _unfreeze();
  }
  
  IloBool solve(const IlcGoal goal, IloBool restore = IloFalse) const;
  
  void add(const IlcConstraint constraint) const;
  
  void add(const IlcConstraintArray constraints) const;
  
  IlcAllocationStack * getHeap() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getHeap();
  }
  
  IlcRandom getRandom() const;
  void setInferenceLevel(IloConstraint ct, IloInt level) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(ct.getImpl() != 0, "IloConstraint: empty handle");
    _setInferenceLevel(ct, level);
  }
  IloInt getInferenceLevel(IloConstraint ct) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getInferenceLevel(ct);
  }
  void resetInferenceLevels() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _resetConstraintInferenceLevels();
  }

  ///////////////////////////////////////////////////////////////////////////
  // Low-level, advanced
  ///////////////////////////////////////////////////////////////////////////
  // No wrapping
  void saveValue(IloAny * ptr) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(ptr != 0, "IloCP: saveValue must receive non-null pointer");
    _saveValue(ptr);
  }
  // No wrapping
  void saveValue(IloInt * ptr) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(ptr != 0, "IloCP: saveValue must receive non-null pointer");
    _saveValue((IloAny*)ptr);
  }
  // No wrapping
  void saveValue(IloNum * ptr) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(ptr != 0, "IloCP: saveValue must receive non-null pointer");
    _saveValue(ptr);
  }
  
  void abortSearch() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _abortSearch();
  }
  
  void clearAbort() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _clearAbort();
  }

  
  void clearLimit() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _clearLimit();
  }

  
  void exitSearch() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _exitSearch();
  }
  
  IlcAllocationStack* getPersistentHeap() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getPersistentHeap();
  }
  
  void addReversibleAction(const IlcGoal goal) const;

  // For propagator
  void failBuffered() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    _failBuffered();
  }
  void removeValueBuffered(IloNumVarI * var, IloInt value) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var != 0, "IloNumVar: empty handle");
    IlcCPOAssert(var->getType() != ILOFLOAT, "IloNumVar: not integer");
    _removeValueBuffered(var, value);
  }
  void setMinBuffered(IloNumVarI * var, IloNum min) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var != 0, "IloNumVar: empty handle");
    _setMinBuffered(var, min);
  }
  void setMaxBuffered(IloNumVarI * var, IloNum max) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var != 0, "IloNumVar: empty handle");
    _setMaxBuffered(var, max);
  }
  IloBool isInteger(IloNumVar var) const {
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    return var.getType() != ILOFLOAT;
  }

  ///////////////////////////////////////////////////////////////////////////
  // Exceptions
  ///////////////////////////////////////////////////////////////////////////
 
 class Exception : public IloAlgorithm::Exception {
    IloInt _status;
  public:
    Exception(int status, const char* str);
    IloInt getStatus() const { return _status; }
  };
  class NoLicense : public Exception {
  public:
    NoLicense(const char* msg)
      : IloCP::Exception(-1, msg) { }
  };

  class MultipleObjException : public Exception {
    IloObjective _obj;
  public:
    MultipleObjException(IloObjective obj)
      :IloCP::Exception(-1, "IloCP can not handle more than one objective object")
      , _obj(obj) {}
    IloObjective getObj() const { return _obj; }
  };

  class GetObjValueNotSupportedException : public Exception {
    IloObjective _obj;
  public:
    GetObjValueNotSupportedException()
      :IloCP::Exception(-1, "IloCP::getValue only supported for "
                            "simple or static lexicographic objectives") { }
  };

  class MultipleSearchException : public Exception {
  public:
    MultipleSearchException()
      : IloCP::Exception(-1, "IloCP can not handle multiple searches"){
    }
  };

  class InvalidDiffException : public Exception {
    IloDiff _diff;
  public:
    InvalidDiffException(IloDiff diff)
      : IloCP::Exception(-1, "Invalid IloDiff constraint for IloCP (can only uses integer expressions)")
      , _diff(diff) {}
    IloDiff getDiff() const { return _diff; }
  };

  class InvalidSequenceConstraintException : public Exception {
    IloIntervalSequenceVar _seq;
    IloIntervalVar _itv;
  public:
    InvalidSequenceConstraintException(IloIntervalSequenceVar seq, IloIntervalVar itv)
      : IloCP::Exception(-1, "Invalid IloFirst, IloLast, IloBefore or IloPrevious constraint for IloCP (constraint must hold on interval variables of the sequence)")
      , _seq(seq), _itv(itv) {}
    IloIntervalSequenceVar getSequenceVariable() const { return _seq; }
    IloIntervalVar getIntervalVariable() const { return _itv; }
  };

  class SolverErrorException : public Exception {
    const char* _function;
    IloInt _errorType;
  public:
    SolverErrorException(const char* function, const char* str, IloInt er)
      : IloCP::Exception(-1, str), _function(function),
      _errorType(er) {}
    virtual void print(ILOSTD(ostream)& o) const;
    const char* getFunction() const { return _function; }
    IloInt getErrorType() const { return _errorType; }
  };

  class SolverErrorExceptionInt : public SolverErrorException {
    const IloInt _value;
  public:
    SolverErrorExceptionInt(const char* function,
                            const char* str,
                            IloInt e,
                            IloInt value) :
      IloCP::SolverErrorException(function, str, e), _value(value) {}

    virtual void print(ILOSTD(ostream)& o) const;
    IloInt getValue() const { return _value; }
  };
  class SolverErrorExceptionIntInt : public SolverErrorExceptionInt {
    const IloInt _value2;
  public:
    SolverErrorExceptionIntInt(const char* function,
                               const char* str,
                               IloInt e,
                               IloInt value,
                               IloInt value2) :
      IloCP::SolverErrorExceptionInt(function, str, e, value),
      _value2(value2) {}

    virtual void print(ILOSTD(ostream)& o) const;
    IloInt getValue2() const { return _value2; }
  };

  class SolverErrorExceptionFloat : public SolverErrorException {
    const IloNum _value;
  public:
    SolverErrorExceptionFloat(const char* function,
                            const char* str,
                            IloInt e,
                            IloNum value) :
      IloCP::SolverErrorException(function, str, e), _value(value) {}

    virtual void print(ILOSTD(ostream)& o) const;
    IloNum getValue() const { return _value; }
  };

  class SolverErrorExceptionAny : public SolverErrorException {
    const IloAny _value;
  public:
    SolverErrorExceptionAny(const char* function,
                            const char* str,
                            IloInt e,
                            IloAny value) :
      IloCP::SolverErrorException(function, str, e), _value(value) {}

    virtual void print(ILOSTD(ostream)& o) const;
    IloAny getValue() const { return _value; }
  };

  class SolverErrorExceptionExprI : public SolverErrorException {
    const void* _exprI;
  public:
    SolverErrorExceptionExprI(const char* function,
                              const char* str,
                              IloInt e,
                              const IlcExprI* expr) :
      IloCP::SolverErrorException(function, str, e), _exprI(expr) {}

    virtual void print(ILOSTD(ostream)& o) const;
    const IlcExprI* getExprI() const { return (IlcExprI*)_exprI; }
  };

  class SolverErrorExceptionExprsI : public SolverErrorExceptionExprI {
    const void* _exprI2;
  public:
    SolverErrorExceptionExprsI(const char* function,
                              const char* str,
                              IloInt e,
                              const IlcExprI* expr,
                              const IlcExprI* expr2) :
      IloCP::SolverErrorExceptionExprI(function, str, e, expr),
      _exprI2(expr2){}

   virtual void print(ILOSTD(ostream)& o) const;
   const IlcExprI* getExprI2() const { return (IlcExprI*)_exprI2; }
  };

  class UnimplementedFeature : public Exception {
  public:
    UnimplementedFeature(const char* message) :
      IloCP::Exception(-1, message) {}
  };

  class ObjectNotExtracted : public Exception {
  public:
    ObjectNotExtracted(const char* message) :
      IloCP::Exception(-1, message) {}
    virtual void print(ILOSTD(ostream)&) const;
  };

  class ModelNotExtracted : public Exception {
  public:
    ModelNotExtracted() : IloCP::Exception(-1, "Model is not loaded") {}
  };

  class BadParameterType : public Exception {
  public:
    BadParameterType(const char* message) :IloCP::Exception(-1, message) {}
  };

  class NumIsNotInteger : public Exception {
  public:
    NumIsNotInteger() :IloCP::Exception(-1, "IloNum is not integer") {}
  };

  class NumIsNotBoolean : public Exception {
  public:
    NumIsNotBoolean() :IloCP::Exception(-1, "IloNum is not boolean") {}
  };

  class IntegerOverflow : public Exception {
  public:
    IntegerOverflow() :IloCP::Exception(-1, "IloNum is out of integer range") {}
  };

  class MixedTypeVariableArray : public Exception {
  public:
    MixedTypeVariableArray(const char* message) :IloCP::Exception(-1, message) {}
  };

  class ArgumentOutOfRange : public Exception {
  public:
    ArgumentOutOfRange(const char* message) :
      IloCP::Exception(-1, message) {}
    virtual void print(ILOSTD(ostream)&) const;
  };

  class VariableShouldBeInteger : public Exception {
  public:
    VariableShouldBeInteger(const char* message) :
      IloCP::Exception(-1, message) {}
    virtual void print(ILOSTD(ostream)&) const;
  };

  class VariableShouldBeFloat : public Exception {
  public:
    VariableShouldBeFloat(const char* message) :
      IloCP::Exception(-1, message) {}
    virtual void print(ILOSTD(ostream)&) const;
  };

  class WrongContext : public Exception {
  public:
    WrongContext(const char* message) :
      IloCP::Exception(-1, message) {}
  };
  class WrongType : public Exception {
  public:
    WrongType(const char* message) :
      IloCP::Exception(-1, message) {}
  };
  class WrongUsage : public Exception {
  public:
    WrongUsage(const char* message) :
      IloCP::Exception(-1, message) {}
  };

  class EmptyHandle : public Exception {
  public:
    EmptyHandle(const char* message) :
      IloCP::Exception(-1, message) {}
  };

  class SizeMustBePositive : public Exception {
  public:
    SizeMustBePositive(const char* message) : IloCP::Exception(-1, message) {}
  };

  class ModelInconsistent : public Exception {
    IloExtractableI* _extractable;
  public:
    ModelInconsistent(IloExtractableI* ext) :IloCP::Exception(-1, "The loaded model is inconsistent"), _extractable(ext) {}
    IloExtractable getExtractable() const { return _extractable; }
    virtual void print(ILOSTD(ostream)&) const;
    virtual const char* getInconsistencyReason() const;
  };

  class IntervalInconsistent : public ModelInconsistent {
  public:
    enum Reason {
      StartRange,
      EndRange,
      SizeRange,
      LengthRange,
      Window
    };
    IntervalInconsistent(IloExtractableI* ext, Reason r) :IloCP::ModelInconsistent(ext), _reason(r) {}
    virtual const char* getInconsistencyReason() const;
  private:
    Reason _reason;
  };

  class StateFunctionNoTriangularInequality : public Exception {
    const IloStateFunctionI* _sf;
    IloInt _i;
    IloInt _j;
    IloInt _k;
  public:
    StateFunctionNoTriangularInequality(const IloStateFunctionI* sf, 
                                        IloInt i =-1, IloInt j =-1, IloInt k =-1)
      :IloCP::Exception(-1, "Transition distance matrix does not satisfy the triangular inequality")
      ,_sf(sf), _i(i), _j(j), _k(k) {}
    virtual void print(ILOSTD(ostream)& o) const;
    const IloStateFunctionI* getStateFunction() const { return _sf; }
    IloInt getI() const { return _i; }
    IloInt getJ() const { return _j; }
    IloInt getK() const { return _k; }
  };
  
  class UndefinedFunctionValue : public Exception {
    const void* _exprI;
  public:
    UndefinedFunctionValue(const IlcFloatExpI* exprI) 
      :IloCP::Exception(-1, "Accessing function outside its definition interval"), 
       _exprI(exprI) {}
    virtual void print(ILOSTD(ostream)& o) const;
  };
  
  class PropagatorException : public Exception {
  public:
    PropagatorException(const char* message) :
      IloCP::Exception(-1, message) {}
  };

  class ParameterCannotBeSetHereException : public Exception {
  public:
    ParameterCannotBeSetHereException(const char* message) :
      IloCP::Exception(-1, message) {}
  };

  class MetaConstraintNotAllowed : public Exception {
  public:
    MetaConstraintNotAllowed() :
      IloCP::Exception(-1, "Global constraints (for example: constraints on arrays) cannot be used in meta-constraints.") {}
  };

  
  class IncompatibleMemoryManagerException : public Exception {
    public: IncompatibleMemoryManagerException();
  };

  class NoSuchXException : public Exception {
    private:
      const char * _x;
      void _ctor(const char *);
    public:
      NoSuchXException(const NoSuchXException &ex)
        : Exception(ex) { _ctor(ex._x); }
      NoSuchXException& operator = (const NoSuchXException & ex) {
        *((Exception*)this) = (const Exception&)ex;
        _ctor(ex._x);
        return *this;
      }
      NoSuchXException(const char * what, const char * x)
        : Exception(-1, what) { _ctor(x); }
      ~NoSuchXException();
      void print(ILOSTD(ostream)&) const;
  };

  class NoSuchParameterException : public NoSuchXException {
    public:
      NoSuchParameterException(const char * param)
        : NoSuchXException("parameter", param) { }
  };
  class NoSuchParameterValueException : public NoSuchXException {
    public:
      NoSuchParameterValueException(const char * paramValue)
        : NoSuchXException("parameter value", paramValue) { }
  };
  class NoSuchInfoException : public NoSuchXException {
    public:
      NoSuchInfoException(const char * info)
        : NoSuchXException("info", info) { }
  };

  class ConflictRefinerException : public Exception {
  public:
    ConflictRefinerException(const char* msg) :IloCP::Exception(-1, msg) {}
  };
  
  class ConflictRefinerNotAddedCt : public ConflictRefinerException {
  public:
  ConflictRefinerNotAddedCt(IloConstraint ct)
  :ConflictRefinerException("Constraint was not added to the model: "), _ct(ct){}
  virtual void print(ILOSTD(ostream)&) const;
  private:
    IloConstraint _ct;
  };

  
  class PresolveException: public Exception {
   private:
    void* _store;
    char* _aggregatedMessage;

   public:
    PresolveException(IlcLaMessageStore* store):
      IloCP::Exception(-1, NULL),
      _store((void*)store),
      _aggregatedMessage(NULL)
    {}
    ~PresolveException();
    virtual const char* getMessage() const;
    
    class Iterator;
    friend class Iterator;
    class Iterator {
     private:
      void* _store;
      IloInt _position;
      IloUInt _getMsgCode() const;
      IloExtractable _getExtractable() const;
      const char* _getMessage() const;
      const char* _getCPOModelPart() const;
     public:
      Iterator(const PresolveException* exception):
        _store((IlcLaMessageStore*)exception->_store),
        _position(0)
      {}
      IloBool ok() const;
      void operator++() {
        IlcCPOAssert(ok(), "Invalid state of the iterator");
        _position++;
      }
      IloUInt getMsgCode() const {
        IlcCPOAssert(ok(), "Invalid state of the iterator");
        return _getMsgCode();
      }
      IloExtractable getExtractable() const {
        IlcCPOAssert(ok(), "Invalid state of the iterator");
        return _getExtractable();
      }
      const char* getMessage() const {
        IlcCPOAssert(ok(), "Invalid state of the iterator");
        return _getMessage();
      }
      
      const char* getCPOModelPart() const {
        IlcCPOAssert(ok(), "Invalid state of the iterator");
        return _getCPOModelPart();
      }
    };

    Iterator getIterator() const { return Iterator(this); }
  };

  class ILMTException : public IloException {
  public:
    ILMTException(const char * message) : IloException(message, IloFalse) { }
  };
  
  ///////////////////////////////////////////////////////////////////////////
  // Unclassified
  ///////////////////////////////////////////////////////////////////////////
  
  IloCP(IloMemoryManager memoryManager);
  IloMemoryManager getReversibleAllocator() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getReversibleAllocator();
  }
  IloMemoryManager getSolveAllocator() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getSolveAllocator();
  }
  IloMemoryManager getPersistentAllocator() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getPersistentAllocator();
  }
  operator IloSolver() const;
  IlcManagerI * getManagerI() const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getManagerI();
  }
  ////////////////////////////////////////////////////////////////////////
  // Run-time license
  ////////////////////////////////////////////////////////////////////////
  
  static IloBool RegisterLicense(const char *, int);

  ////////////////////////////////////////////////////////////////////////
  // XML registration
  ////////////////////////////////////////////////////////////////////////
  
  static void RegisterXML(IloEnv env) {
    IlcCPOAssert(env.getImpl() != 0, "IloEnv: empty handle");
    _RegisterXML(env);
  }
  static void UseStandardCPLEX();

  
   IloBool isFixed(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _isFixed(a);
  }
 
  IloBool isPresent(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _isPresent(a);
  }
 
  IloBool isAbsent(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _isAbsent(a);
  }
  
  IloInt getStartMin(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getStartMin(a);
  }
  
  IloInt getStartMax(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getStartMax(a);
  }
  
  IloInt getStart(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isFixed(a), "IloIntervalVar: not fixed.");
    return _getStart(a);
  }
  
  IloInt getEndMin(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getEndMin(a);
  }
  
  IloInt getEndMax(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getEndMax(a);
  }
  
  IloInt getEnd(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isFixed(a), "IloIntervalVar: not fixed.");
    return _getEnd(a);
  }
  
  IloInt getSizeMin(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getSizeMin(a);
  }
  
  IloInt getSizeMax(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getSizeMax(a);
  }
  
  IloInt getSize(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isFixed(a), "IloIntervalVar: not fixed.");
    return _getSize(a);
  }
  
  IloInt getLengthMin(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getLengthMin(a);
  }
  
  IloInt getLengthMax(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    return _getLengthMax(a);
  }
  
  IloInt getLength(const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isFixed(a), "IloIntervalVar: not fixed.");
    return _getLength(a);
  }
  void printDomain(ILOSTD(ostream)& s, const IloIntervalVar a) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl() != 0, "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    _printDomain(s, a);
  }
  void printDomain(const IloIntervalVar a) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle.");
    IlcCPOAssert(a.getImpl() != 0, "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    _printDomain(out(), a);
  }
  
  IloBool isFixed(const IloIntervalSequenceVar seq) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(seq.getImpl(), "IloIntervalSequenceVar: empty handle.");
    IlcCPOAssert(isExtracted(seq), "IloIntervalSequenceVar: not extracted.");
    return _isFixed(seq);
  }
  
  IloIntervalVar getFirst(const IloIntervalSequenceVar seq) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(seq.getImpl(), "IloIntervalSequenceVar: empty handle.");
    IlcCPOAssert(isExtracted(seq), "IloIntervalSequenceVar: not extracted.");
    IlcCPOAssert(isFixed(seq), "IloIntervalSequenceVar: not fixed.");
    return _getFirst(seq);
  }
  
  IloIntervalVar getLast (const IloIntervalSequenceVar seq) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(seq.getImpl(), "IloIntervalSequenceVar: empty handle.");
    IlcCPOAssert(isExtracted(seq), "IloIntervalSequenceVar: not extracted.");
    IlcCPOAssert(isFixed(seq), "IloIntervalSequenceVar: not fixed.");
    return _getLast(seq);
  }
  
  IloIntervalVar getNext(const IloIntervalSequenceVar seq, const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(seq.getImpl(), "IloIntervalSequenceVar: empty handle.");
    IlcCPOAssert(isExtracted(seq), "IloIntervalSequenceVar: not extracted.");
    IlcCPOAssert(isFixed(seq), "IloIntervalSequenceVar: not fixed.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isPresent(a), "IloIntervalVar: not present.");
    IlcCPOAssert(isInSequence(seq, a), "IloIntervalVar: not in sequence variable.");
     return _getNext(seq, a);
  }
  
  IloIntervalVar getPrev (const IloIntervalSequenceVar seq, const IloIntervalVar a) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    IlcCPOAssert(seq.getImpl(), "IloIntervalSequenceVar: empty handle.");
    IlcCPOAssert(isExtracted(seq), "IloIntervalSequenceVar: not extracted.");
    IlcCPOAssert(isFixed(seq), "IloIntervalSequenceVar: not fixed.");
    IlcCPOAssert(a.getImpl(), "IloIntervalVar: empty handle.");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isPresent(a), "IloIntervalVar: not present.");
    IlcCPOAssert(isInSequence(seq, a), "IloIntervalVar: not in sequence variable.");
    return _getPrev(seq, a);
  }

  
  IloBool isFixed(const IloCumulFunctionExpr f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCumulFunctionExpr: not extracted");
    return _isFixed(f);
  }

  
  IloInt getNumberOfSegments(const IloCumulFunctionExpr f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    return _getNumberOfSegments(f);
  }

  
  IloNum getNumberOfSegmentsAsNum(const IloCumulFunctionExpr f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    return _getNumberOfSegmentsAsNum(f);
  }

  
  IloInt getSegmentStart(const IloCumulFunctionExpr f, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidSegment(f, i), "IloCP: invalid cumul function expression segment");
    return _getSegmentStart(f, i);
  }
  
  IloNum getSegmentStartAsNum(const IloCumulFunctionExpr f, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidSegment(f, i), "IloCP: invalid cumul function expression segment");
    return _getSegmentStartAsNum(f, i);
  }

  
  IloInt getSegmentEnd(const IloCumulFunctionExpr f, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidSegment(f, i), "IloCP: invalid cumul function expression segment");
    return _getSegmentEnd(f, i);
  }
  
  IloNum getSegmentEndAsNum(const IloCumulFunctionExpr f, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidSegment(f, i), "IloCP: invalid cumul function expression segment");
    return _getSegmentEndAsNum(f, i);
  }

  
  IloInt getSegmentValue(const IloCumulFunctionExpr f, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidSegment(f, i), "IloCP: invalid cumul function expression segment");
    return _getSegmentValue(f, i);
  }
  
  IloNum getSegmentValueAsNum(const IloCumulFunctionExpr f, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidSegment(f, i), "IloCP: invalid cumul function expression segment");
    return _getSegmentValueAsNum(f, i);
  }

  
  IloInt getValue(const IloCumulFunctionExpr f, IloInt t) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidAbscissa(f, t), "IloCP: cumul function expression evaluated on invalid point");
    return _getValue(f, t);
  }
  
  IloInt getHeightAtStart(const IloIntervalVar var, const IloCumulFunctionExpr f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntervalVar: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(isExtracted(var), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isFixed(var), "IloIntervalVar: not fixed.");
    return _getHeightAtFoo(var, f, IloTrue);
  }
  
  IloInt getHeightAtEnd(const IloIntervalVar var, const IloCumulFunctionExpr f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntervalVar: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(isExtracted(var), "IloIntervalVar: not extracted.");
    IlcCPOAssert(isFixed(var), "IloIntervalVar: not fixed.");
    return _getHeightAtFoo(var, f, IloFalse);
  }
  
  IloNum getValueAsNum(const IloCumulFunctionExpr f, IloInt t) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloCumulFunctionExpr: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: cumul function expression not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: cumul function expression not fixed");
    IlcCPOAssert(_isValidAbscissa(f, t), "IloCP: cumul function expression evaluated on invalid point");
    return _getValueAsNum(f, t);
  }

  ////////////////////////////////////////////////////////////////////////
  // Reading State Functions at solution
  ////////////////////////////////////////////////////////////////////////

 

  enum FunctionValues {
  
        NoState = -1
  };

  
  IloBool isFixed(const IloStateFunction f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloStateFunctionFunction: not extracted");
    return _isFixed(f);
  }

  
  IloInt getNumberOfSegments(const IloStateFunction f) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    return _getNumberOfSegments(f);
  }


  
  IloInt getSegmentStart(const IloStateFunction f, IloInt s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidSegment(f, s), "IloCP: invalid state function segment");
    return _getSegmentStart(f, s);
  }
  
  IloNum getSegmentStartAsNum(const IloStateFunction f, IloInt s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidSegment(f, s), "IloCP: invalid state function segment");
    return _getSegmentStartAsNum(f, s);
  }
  
  
  IloInt getSegmentEnd(const IloStateFunction f, IloInt s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidSegment(f, s), "IloCP: invalid state function segment");
    return _getSegmentEnd(f, s);
  }
  
  IloNum getSegmentEndAsNum(const IloStateFunction f, IloInt s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidSegment(f, s), "IloCP: invalid state function segment");
    return _getSegmentEndAsNum(f, s);
  }
 
  
  IloInt getSegmentValue(const IloStateFunction f, IloInt s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidSegment(f, s), "IloCP: invalid state function segment");
    return _getSegmentValue(f, s);
  }
  
  IloNum getSegmentValueAsNum(const IloStateFunction f, IloInt s) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidSegment(f, s), "IloCP: invalid state function segment");
    return _getSegmentValueAsNum(f, s);
  }
  
  IloInt getValue(const IloStateFunction f, IloInt t) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidAbscissa(f, t), "IloCP: state function evaluated on invalid point");
    return _getValue(f, t);
  }
  
  IloNum getValueAsNum(const IloStateFunction f, IloInt t) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    IlcCPOAssert(f.getImpl() != 0, "IloStateFunction: empty handle");
    IlcCPOAssert(isExtracted(f), "IloCP: state function not extracted");
    IlcCPOAssert(_isFixed(f), "IloCP: state function not fixed");
    IlcCPOAssert(_isValidAbscissa(f, t), "IloCP: state function evaluated on invalid point");
    return _getValueAsNum(f, t);
  }
  
  ///////////////////////////////////////////////////////////////////////////
  // class IloStateFunctionExpr
  ///////////////////////////////////////////////////////////////////////////

  
  IloInt getNumberOfSegments(const IloStateFunctionExpr expr) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getNumberOfSegments(expr);
  }


  
  IloInt getSegmentStart(const IloStateFunctionExpr expr, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getSegmentStart(expr, i);
  }



  
  IloInt getSegmentEnd(const IloStateFunctionExpr expr, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getSegmentEnd(expr, i);
  }


  
  IloInt getSegmentValue(const IloStateFunctionExpr expr, IloInt i) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getSegmentValue(expr, i);
  }

  
  IloInt getValue(const IloStateFunctionExpr expr, IloInt t) const {
    IlcCPOAssert(getImpl() != 0, "IloCP: empty handle");
    return _getValue(expr, t);
  }

  ///////////////////////////////////////////////////////////////////////////
  // Printing interval domains
  ///////////////////////////////////////////////////////////////////////////

  class PrintIntervalVarDomains : public PrintDomains {
    friend class IloCP;
  private:
    void operator = (const PrintIntervalVarDomains&);
    PrintIntervalVarDomains(const IloCP cp, const IloIntervalVar var);
  public:
    void display(ILOSTD(ostream)& o) const;
  };
  
  PrintIntervalVarDomains domain(const IloIntervalVar a) const {
    IlcCPOAssert(_impl != 0, "IloCP: empty handle");
    IlcCPOAssert(a.getImpl() != 0, "IloIntervalVar: empty handle");
    IlcCPOAssert(isExtracted(a), "IloIntervalVar: not extracted");
    return PrintIntervalVarDomains(*this, a);
  }

  // ------------------------------------------------------------------------
  // Advanced
  // ------------------------------------------------------------------------
  IloBool isInSequence (const IloIntervalSequenceVar seq,
                        const IloIntervalVar a) const;
  void prettyPrintSchedule(ILOSTD(ostream)& s) const {
    IlcCPOAssert(getImpl(), "IloCP: empty handle.");
    _prettyPrintSchedule(s);
  }
 private:
  IloBool _isFixed      (const IloIntervalVar a) const;
  IloBool _isPresent    (const IloIntervalVar a) const;
  IloBool _isAbsent     (const IloIntervalVar a) const;
  IloInt  _getStartMin  (const IloIntervalVar a) const;
  IloInt  _getStartMax  (const IloIntervalVar a) const;
  IloInt  _getStart     (const IloIntervalVar a) const;
  IloInt  _getEndMin    (const IloIntervalVar a) const;
  IloInt  _getEndMax    (const IloIntervalVar a) const;
  IloInt  _getEnd       (const IloIntervalVar a) const;
  IloInt  _getSizeMin   (const IloIntervalVar a) const;
  IloInt  _getSizeMax   (const IloIntervalVar a) const;
  IloInt  _getSize      (const IloIntervalVar a) const;
  IloInt  _getLengthMin (const IloIntervalVar a) const;
  IloInt  _getLengthMax (const IloIntervalVar a) const;
  IloInt  _getLength    (const IloIntervalVar a) const;
  void    _printDomain  (ILOSTD(ostream)&, const IloIntervalVar a) const;
  IloBool _isFixed      (const IloIntervalSequenceVar seq) const;
  IloIntervalVar _getFirst(const IloIntervalSequenceVar seq) const;
  IloIntervalVar _getLast (const IloIntervalSequenceVar seq) const;
  IloIntervalVar _getNext (const IloIntervalSequenceVar seq, const IloIntervalVar a) const;
  IloIntervalVar _getPrev (const IloIntervalSequenceVar seq, const IloIntervalVar a) const;
  void _prettyPrintSchedule(ILOSTD(ostream)& s) const;

public:
  void setJavaVM(void* vm);


  
  void storeWarningsInternally(IloBool store = IloTrue);

  
  class WarningIterator {
   private:
    IloCPI* _cp;
    IloInt  _position;
    IloUInt _getMsgCode() const;
    IloExtractable _getExtractable() const;
    const char* _getMessage() const;
    const char* _getCPOModelPart() const;
   public:
    WarningIterator(IloCP cp);
    ~WarningIterator();
    IloBool ok() const;
    void operator++() {
      IlcCPOAssert(ok(), "Invalid state of the iterator");
      _position++;
    }
    IloUInt getMsgCode() const {
      IlcCPOAssert(ok(), "Invalid state of the iterator");
      return _getMsgCode();
    }
    IloExtractable getExtractable() const {
      IlcCPOAssert(ok(), "Invalid state of the iterator");
      return _getExtractable();
    }
    const char* getMessage() const {
      IlcCPOAssert(ok(), "Invalid state of the iterator");
      return _getMessage();
    }
    
    const char* getCPOModelPart() const {
      IlcCPOAssert(ok(), "Invalid state of the iterator");
      return _getCPOModelPart();
    }
  };
};

ILOSTD(ostream)& operator << (ILOSTD(ostream) & o,
                              const IloCP::PrintNumVarDomains& doms);
ILOSTD(ostream)& operator << (ILOSTD(ostream) & o,
                              const IloCP::PrintIntervalVarDomains& doms);

////////////////////////////////////////////////////////////////////////
//
// ILOCPHOOK
//
////////////////////////////////////////////////////////////////////////

class IloCPHookI : public IloEnvObjectI {
public:
  IloCPHookI(IloEnv env) : IloEnvObjectI(env.getImpl()) { }
  virtual void execute(IloCP cp) = 0;
};

////////////////////////////////////////////////////////////////////////
//
// IloSolver compat
//
////////////////////////////////////////////////////////////////////////


typedef enum {
  IlcLow=0L,
  IloLowLevel=0L,
  IlcBasic=1L,
  IloBasicLevel=1L,
  IlcMedium=2L,
  IloMediumLevel=2L,
  IlcExtended=3L,
  IloExtendedLevel=3L
} IlcFilterLevel;

typedef enum {
  IlcAllDiffCt=0L,
  IloAllDiffCt=0L,
  IlcDistributeCt=1L,
  IloDistributeCt=1L,
  IlcSequenceCt=2L,
  IloSequenceCt=2L,
  IlcAllMinDistanceCt=3L,
  IloAllMinDistanceCt=3L,
  IlcPartitionCt=4L,
  IloPartitionCt=4L,
  IlcAllNullIntersectCt=5L,
  IloAllNullIntersectCt=5L,
  IlcEqUnionCt=6L,
  IloEqUnionCt=6L,
  IlcCountCt=8L,
  IloCountCt=8L
} IlcFilterLevelConstraint;

typedef enum {
  IlcStandardDisplay = 0,
  IlcIntScientific,
  IlcIntFixed,
  IlcBasScientific,
  IlcBasFixed
} IlcFloatDisplay;

class IloSolver : public IloCP {
private:
  void _setPropagationControl(IloNumVar var) const;
  void _setPropagationControl(IloIntVar var) const;
  void _setMin(IloNumVar var, IloNum min) const;
  void _setMax(IloNumVar var, IloNum max) const;
  void _setValue(IloNumVar var, IloNum value) const;

  void _ctor(const IloModel model);
  void _ctor(const IloEnv env);
  void _setSolverConfig() const;
public:
  IloSolver(const IloModel model) : IloCP() {
    IlcCPOAssert(model.getImpl() != 0, "IloModel: empty handle");
    _ctor(model);
  }
  IloSolver(const IloEnv env) : IloCP() {
    IlcCPOAssert(env.getImpl() != 0, "IloEnv: empty handle");
    _ctor(env);
  }
  IloSolver(IloCPI * impl = 0) : IloCP(impl) { }

  enum SearchState {
    IloBeforeSearch = 0,
    IloDuringSearch,
    IloAfterSearch
  };
  typedef enum {
    searchHasNotFailed = 0,
    searchHasFailedNormally,
    searchStoppedByLimit,
    searchStoppedByLabel,
    searchStoppedByExit,
    searchStoppedByAbort,
    searchStoppedByException,
    unknownFailureStatus
  } FailureStatus;
  FailureStatus convertFailureStatus(IloInt f) const {
    switch (f) {
      case IloCP::SearchHasNotFailed:      return searchHasNotFailed;
      case IloCP::SearchHasFailedNormally: return searchHasFailedNormally;
      case IloCP::SearchStoppedByLimit:    return searchStoppedByLimit;
      case IloCP::SearchStoppedByLabel:    return searchStoppedByLabel;
      case IloCP::SearchStoppedByExit:     return searchStoppedByExit;
      case IloCP::SearchStoppedByAbort:    return searchStoppedByAbort;
      case IloCP::SearchStoppedByException: return searchStoppedByException;
      default:                             return unknownFailureStatus;
    }
    return unknownFailureStatus;
  }

  IloCP::ParameterValues filterToInferenceLevel(IlcFilterLevel fl) const {
    switch (fl) {
      case IlcLow :     return IloCP::Low;
      case IlcBasic:    return IloCP::Basic;
      case IlcMedium:   return IloCP::Medium;
      case IlcExtended: return IloCP::Extended;
    }
    IlcCPOAssert(0, "IloSolver: Invalid filter level");
    return IloCP::Basic;
  }
  IloCP::IntParam filterToInferenceCt(IlcFilterLevelConstraint fl) const {
    switch (fl) {
      case IloAllDiffCt:          return IloCP::AllDiffInferenceLevel;
      case IloDistributeCt:       return IloCP::DistributeInferenceLevel;
      case IloSequenceCt:         return IloCP::SequenceInferenceLevel;
      case IloAllMinDistanceCt:   return IloCP::AllMinDistanceInferenceLevel;
      case IloCountCt:            return IloCP::CountInferenceLevel;
      default: IlcCPOAssert(0, "IloSolver: Invalid filter constraint");
    }
    return IloCP::AllDiffInferenceLevel;
  }
  void setFilterLevel(IloConstraint ct, IlcFilterLevel fl) const {
    setInferenceLevel(ct, filterToInferenceLevel(fl));
  }
  void setDefaultFilterLevel(IlcFilterLevelConstraint ct,
                             IlcFilterLevel fl) const {
    setParameter(filterToInferenceCt(ct), filterToInferenceLevel(fl));
  }
  void setDefaultFilterLevel(IlcFilterLevel fl) const {
    setParameter(IloCP::DefaultInferenceLevel, filterToInferenceLevel(fl));
  }
  ILCDEPRECATED void setFilterLevel(IlcConstraint ct, IlcFilterLevel fl) const;
  ILCDEPRECATED IlcConstraint getConstraint(IloConstraint ct) const;
  ILCDEPRECATED IlcConstraintArray getConstraintArray(IloConstraintArray ct) const;
  IloBool isInSearch() const;
  FailureStatus getFailureStatus() const {
    IlcCPOAssert(getImpl() != 0, "IloSolver: empty handle");
    return convertFailureStatus(getInfo(IloCP::FailStatus));
  }
  void setOptimizationStep(IloNum step) const {
    setParameter(IloCP::OptimalityTolerance, step);
  }
  void setRelativeOptimizationStep(IloNum step) const {
    setParameter(IloCP::RelativeOptimalityTolerance, step);
  }
  IloNum getOptimizationStep() const {
    return getParameter(IloCP::OptimalityTolerance);
  }
  IloNum getRelativeOptimizationStep() const {
    return getParameter(IloCP::RelativeOptimalityTolerance);
  }
  void setFailLimit(IloInt fails) const {
    setParameter(IloCP::FailLimit, fails);
  }
  void setOrLimit(IloInt cps) const {
    setParameter(IloCP::ChoicePointLimit, cps);
  }
  void setTimeLimit(IloNum time) const {
    setParameter(IloCP::TimeLimit, time);
  }
  void unsetLimit() const {
    setFailLimit(IloIntMax);
    setOrLimit(IloIntMax);
    setTimeLimit(IloInfinity);
  }
  void setFloatDisplay(IlcFloatDisplay display) const {
    setParameter(IloCP::FloatDisplay,
      display - IlcStandardDisplay + IloCP::Standard
    );
  }
  IlcFloatDisplay getFloatDisplay() const {
    return IlcFloatDisplay(getParameter(IloCP::FloatDisplay)
                         - IloCP::Standard
                         + IlcStandardDisplay);
  }
  IloNum getTime() const { return getInfo(IloCP::SolveTime); }
  IloUInt getMemoryUsage() const { return getInfo(IloCP::MemoryUsage); }
  IloNum getDefaultPrecision() const;
  void setFastRestartMode(IloBool mode) const;
  IloInt getNumberOfChoicePoints() const {
    return getInfo(IloCP::NumberOfChoicePoints);
  }
  IloInt getNumberOfConstraints() const {
    return getInfo(IloCP::NumberOfConstraints);
  }
  using IloCP::getNumberOfFails;
  IloInt getNumberOfFails() const {
    return getInfo(IloCP::NumberOfFails);
  }
  IloInt getNumberOfVariables() const {
    return getInfo(IloCP::NumberOfVariables);
  }
  void setDefaultPrecision(IloNum precision) const;
  void setPackApproximationSize(IloInt size) const {
    setParameter(IloCP::PackApproximationSize, size);
  }
  void setPropagationControl(IloNumVar var) const {
    IlcCPOAssert(getImpl() != 0, "IloSolver: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    _setPropagationControl(var);
  }
  void setPropagationControl(IloIntVar var) const {
    IlcCPOAssert(getImpl() != 0, "IloSolver: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloIntVar: empty handle");
    _setPropagationControl(var);
  }
  void setMin(IloNumVar var, IloNum min) const {
    IlcCPOAssert(getImpl() != 0, "IloSolver: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    _setMin(var, min);
  }
  void setMax(IloNumVar var, IloNum max) const {
    IlcCPOAssert(getImpl() != 0, "IloSolver: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    _setMax(var, max);
  }
  void setValue(IloNumVar var, IloNum value) const {
    IlcCPOAssert(getImpl() != 0, "IloSolver: empty handle");
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    _setValue(var, value);
  }
};

////////////////////////////////////////////////////////////////////////
//
// PROPAGATORS
//
////////////////////////////////////////////////////////////////////////

class IloPropagatorI : public IloEnvObjectI {
private:
  IloNumVarArray _vars;
  IloCP          _cp;

  void _addVar(IloNumVar var);
public:
  
  IloPropagatorI(IloEnv env);
  
  virtual ~IloPropagatorI();

  IloNumVar getVar(IloInt i) const { return _vars[i]; }
  IloInt getNumVars() const { return _vars.getSize(); }

  
  void addVar(IloNumVar var) {
    IlcCPOAssert(var.getImpl() != 0, "IloNumVar: empty handle");
    _addVar(var);
  }

  void violate(IloCP cp) {
    cp.failBuffered();
  }

  
  void violate() {
    violate(_cp);
  }

  void setMax(IloCP cp, IloNumVar var, IloNum max) {
    cp.setMaxBuffered(var.getImpl(), max);
  }
  void setMax(IloCP cp, IloIntVar var, IloInt max) {
    cp.setMaxBuffered(var.getImpl(), (IloNum)max);
  }
  
  void setMax(IloNumVar var, IloNum max) {
    setMax(_cp, var, max);
  }
  void setMax(IloIntVar var, IloInt max) {
    setMax(_cp, var, max);
  }

  void setMin(IloCP cp, IloNumVar var, IloNum min) {
    cp.setMinBuffered(var.getImpl(), min);
  }
  void setMin(IloCP cp, IloIntVar var, IloInt min) {
    cp.setMinBuffered(var.getImpl(), (IloNum)min);
  }
  
  void setMin(IloNumVar var, IloNum min) {
    setMin(_cp, var, min);
  }

  void setRange(IloCP cp, IloNumVar var, IloNum min, IloNum max) {
    setMin(cp, var, min);
    setMax(cp, var, max);
  }
  void setRange(IloCP cp, IloIntVar var, IloInt min, IloInt max) {
    setMin(cp, var, min);
    setMax(cp, var, max);
  }
  
  void setRange(IloNumVar var, IloNum min, IloNum max) {
    setRange(_cp, var, min, max);
  }
  void setRange(IloIntVar var, IloInt min, IloInt max) {
    setRange(_cp, var, min, max);
  }

  void setValue(IloCP cp, IloNumVar var, IloNum value) {
    setRange(cp, var, value, value);
  }
  void setValue(IloCP cp, IloIntVar var, IloInt value) {
    setRange(cp, var, value, value);
  }
  
  void setValue(IloNumVar var, IloNum value) {
    setValue(_cp, var, value);
  }
  void setValue(IloIntVar var, IloInt value) {
    setValue(_cp, var, value);
  }

  void removeValue(IloCP cp, IloIntVar var, IloInt value) {
    cp.removeValueBuffered(var.getImpl(), value);
  }
  
  void removeValue(IloIntVar var, IloInt value) {
    removeValue(_cp, var, value);
  }

  IloNum getMin(IloCP cp, IloNumVar var) const {
    return cp.getMin(var);
  }
  IloInt getMin(IloCP cp, IloIntVar var) const {
    return cp.getMin(var);
  }
  
  IloNum getMin(IloNumVar var) const {
    return getMin(_cp, var);
  }
  IloInt getMin(IloIntVar var) const {
    return getMin(_cp, var);
  }

  IloNum getMax(IloCP cp, IloNumVar var) const  {
    return cp.getMax(var);
  }
  IloInt getMax(IloCP cp, IloIntVar var) const  {
    return cp.getMax(var);
  }
  
  IloNum getMax(IloNumVar var) const { return getMax(_cp, var); }
  IloInt getMax(IloIntVar var) const { return getMax(_cp, var); }

  IloNum getValue(IloCP cp, IloNumVar var) const { return cp.getValue(var); }
  IloInt getValue(IloCP cp, IloIntVar var) const { return cp.getValue(var); }
  
  IloNum getValue(IloNumVar var) const { return getValue(_cp, var); }
  IloInt getValue(IloIntVar var) const { return getValue(_cp, var); }

  IloInt getDomainSize(IloCP cp, IloNumVar var) const {
    return cp.getDomainSize(var);
  }
  
  IloInt getDomainSize(IloNumVar var) const {
    return getDomainSize(_cp, var);
  }
  IloBool isInDomain(IloCP cp, IloNumVar var, IloInt value) const {
    return cp.isInDomain(var, value);
  }
  
  IloBool isInDomain(IloNumVar var, IloInt value) const {
    return isInDomain(_cp, var, value);
  }
  IloBool isFixed(IloCP cp, IloNumVar var) const {
    return cp.isFixed(var);
  }
  IloBool isFixed(IloCP cp, IloIntVar var) const {
    return cp.isFixed(var);
  }
  
  IloBool isFixed(IloNumVar var) const { return isFixed(_cp, var); }
  IloBool isFixed(IloIntVar var) const { return isFixed(_cp, var); }
  IloCP::IntVarIterator iterator(IloCP cp, IloNumVar var) {
    return IloCP::IntVarIterator(cp, var);
  }
  
  IloCP::IntVarIterator iterator(IloNumVar var) {
    return iterator(_cp, var);
  }

  
  virtual void execute() = 0;
  
  virtual IloPropagatorI* makeClone(IloEnv env) const=0;
  void setCP(IloCP cp);
  friend class IlcPropagatorConstraintI;
  friend class IntVarIterator;
};


IloConstraint IloCustomConstraint(IloEnv env, IloPropagatorI * prop);

////////////////////////////////////////////////////////////////////////
//
// ILOGOAL
//
////////////////////////////////////////////////////////////////////////


class IloGoalI : public IloExtensibleRttiEnvObjectI {
public:

  IloGoalI(IloEnvI*);

  virtual ~IloGoalI();

  virtual IlcGoal extract(const IloCP cp) const=0;

  virtual void display(ILOSTD(ostream&)) const;
  ILORTTIDECL
};


class IloGoal {
  ILOCPHANDLEINLINE(IloGoal, IloGoalI)
public:
  typedef IloGoalI ImplClass;

  IloGoal(IloEnv env, IloIntVarArray vars);

  IloGoal(IloEnv env, IloIntVarArray vars,
                      IloIntVarChooser varChooser,
                      IloIntValueChooser valueChooser);

  IloEnv getEnv() const;

  void end() const;
};

ILOSTD(ostream&) operator << (ILOSTD(ostream&), const IloGoal&);


typedef IloArray<IloGoal> IloGoalArray;


IloGoal IloGoalTrue(const IloEnv);


IloGoal IloGoalFail(const IloEnv);


IloGoal operator && (const IloGoal g1, const IloGoal g2);

IloGoal IloAndGoal(const IloEnv env, const IloGoal, const IloGoal);

IloGoal operator||(const IloGoal g1, const IloGoal g2);

IloGoal IloOrGoal(const IloEnv env, const IloGoal, const IloGoal);

#ifdef ILCENABLEUSINGCPO
using namespace CPOptimizer;
#endif

#ifdef _MSC_VER
#pragma pack(pop)
#endif

ILCGCCEXPORTOFF

#endif

