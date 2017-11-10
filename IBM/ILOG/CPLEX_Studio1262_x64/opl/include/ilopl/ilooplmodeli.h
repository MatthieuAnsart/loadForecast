// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilopl/ilooplmodeli.h
// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55 
// Copyright IBM Corp. 1998, 2013
//
// US Government Users Restricted Rights - Use, duplication or
// disclosure restricted by GSA ADP Schedule Contract with
// IBM Corp.
// ---------------------------------------------------------------------------

#ifndef __OPL_ilooplmodeliH
#define __OPL_ilooplmodeliH

#ifdef _WIN32
#pragma pack(push, 8)
#endif


//#include <ilopl/ilooplmodeldefinitioni.h>
//#include <ilopl/ilooplsettingsi.h>
#include <ilopl/ilooplelement.h>

#ifndef __CONCERT_ilostringH
# include <ilconcert/ilostring.h>
#endif


#include <iostream>
//using namespace std;

class IloCplex;
class IloCPI;


class IloOplErrorHandlerI;
class IljVirtualMachine;
class IljIde;
class IloOplExecutionControllerI;
class IloOplRefCounterI;

class IloOplModelSourceI;
class IloOplModelI;

class IloOplElementTable;
class IloOplLoader;
class IloOplScripting;
class IloOplAst;
class IloOplDataSourceI;
class IloOplResultPublisherI;

class IloOplProfilerI;
class IloOplProgressListenerI;

class IloOplElementDefinitionI;
class IloOplElementDefinitionTable;
class IloOplElementDefinitionIteratorI;

class IloOplLabelCallbackI;
class IloOplCustomDataHandlerI;

class IloOplDecisionExprCallbackI;
class IloOplDecisionExprSolutionCallbackI;

class IloOplSettingsI;
class IloOplDataElementsI;
class IloOplScriptExpressionI;

class IloModelEvaluatorSolutionGetterI;

class IloOplTuningCallbackI;

class ILOOPL_EXPORTED IloOplSolutionGetterI: public IloRttiEnvObjectI {
    ILORTTIDECL
private:
    IloAlgorithmI* _algorithm;
    const IloOplModelI* _opl;
    IloModelEvaluatorSolutionGetterI* _bridge;
    IloInt _depth;

protected:
    IloOplSolutionGetterI(IloEnvI* env) :
    IloRttiEnvObjectI(env), _algorithm(0), _opl(0), _bridge(0), _depth(0) {
    }

    IloOplSolutionGetterI(IloEnvI* env, const IloOplSolutionGetterI& other) :
    IloRttiEnvObjectI(env), _algorithm(other._algorithm), _opl(other._opl), _bridge(0), _depth(0) {
    }

    inline IloOplSolutionGetterI(IloEnvI* env, const IloOplModelI& opl);

    IloBool isValid() const;

public:
    const IloOplModelI& getOplModel() const {
        ASSERT_IMPL_OBJ( _opl );
        return *_opl;
    }
    IloAlgorithmI* getAlgorithm() const {
        return _algorithm;
    }

    virtual IloBool getWarnObsolete() const {
        return IloFalse;
    }        

    void checkSolutionAvailable() const;

    virtual IloBool isSolutionAvailable() const =0;
    virtual IloBool isIntermediateSolution() const =0;

    virtual IloInt getIntValue(IloIntVar dvar) const =0;
    virtual IloNum getNumValue(IloNumVar dvar) const =0;

    virtual IloBool hasSlack(IloConstraint ct) const =0;
    virtual IloNum getSlack(IloConstraint ct) const;

    virtual IloBool hasDual(IloConstraint ct) const =0;
    virtual IloNum getDual(IloConstraint ct) const;

    virtual IloBool hasReducedCost(IloNumVar ct) const =0;
    virtual IloNum getReducedCost(IloNumVar ct) const;

    virtual IloBool hasObjValue() const =0;
    virtual IloNum getObjValue() const;    
    virtual IloInt getNumberOfCriteria() const;
    virtual IloNum getObjValue(IloInt index) const;

    virtual void printSolutionTitle(std::ostream& os, IloBool verbose =IloFalse) const;

    IloInt evalIntExpr(IloIntExprArg expr) const;
    IloNum evalNumExpr(IloNumExprArg expr) const;

    inline IloBool isConvertAllIntVars() const;

    void activate() const;
    void desactivate() const;
    IloBool isActivated() const {
        return _bridge!=0;
    }

    class ActivationScope {
        const IloOplSolutionGetterI* _getter;
    public:
        explicit ActivationScope(const IloOplSolutionGetterI& getter):_getter(&getter) {
            _getter->activate();
        }
        explicit ActivationScope(const IloOplSolutionGetterI* getter):_getter(getter) {
            if ( _getter ) _getter->activate();
        }
        ~ActivationScope() {
            if ( _getter ) _getter->desactivate();
        }
    };

	virtual IloInt getIntervalValue( const IloIntervalVar dvar, const IloModelEvaluatorSolutionGetterBaseI::IntervalField field ) const {
		return 0;
	}
	
	virtual IloIntervalVar getSequenceValue( const IloIntervalSequenceVar seq, const IloIntervalVar a, const IloModelEvaluatorSolutionGetterBaseI::SequenceDirection direction = IloModelEvaluatorSolutionGetterBaseI::LeftToRight ) const {
		return 0;
	}
	virtual IloInt getCumulValue( const IloCumulFunctionExprArg cumulFct, const IloModelEvaluatorSolutionGetterBaseI::FunctionField field, const IloInt pos = -1 ) const {
	  return 0;
	}
	virtual IloInt getStateValue( const IloStateFunctionExprArg stateFct, const IloModelEvaluatorSolutionGetterBaseI::FunctionField field, const IloInt pos = -1 ) const {
	  return 0;
	}
};

class ILOOPL_EXPORTED IloOplOverrideSolutionGetterI: public IloOplSolutionGetterI {
    const IloOplSolutionGetterI* _delegate;

public:
    IloOplOverrideSolutionGetterI(IloEnvI* env, const IloOplSolutionGetterI& deleg):
      IloOplSolutionGetterI(env,deleg), _delegate(&deleg) {
    }

    const IloOplSolutionGetterI& getDelegate() const {
        return *_delegate;
    }

    virtual IloBool isSolutionAvailable() const {
        return _delegate->isSolutionAvailable();
    }
    virtual IloBool isIntermediateSolution() const {
        return _delegate->isIntermediateSolution();
    }
    virtual IloBool getWarnObsolete() const {
        return _delegate->getWarnObsolete();
    }
    virtual IloInt getIntValue(IloIntVar var) const { 
        return _delegate->getIntValue(var); 
    }
    virtual IloNum getNumValue(IloNumVar var) const { 
        return _delegate->getNumValue(var); 
    }       
    virtual IloBool hasSlack(IloConstraint ct) const { 
        return _delegate->hasSlack(ct); 
    }       
    virtual IloNum getSlack(IloConstraint ct) const { 
        return _delegate->getSlack(ct); 
    }       
    virtual IloBool hasDual(IloConstraint ct) const { 
        return _delegate->hasDual(ct); 
    }       
    virtual IloNum getDual(IloConstraint ct) const { 
        return _delegate->getDual(ct); 
    }       
    virtual IloBool hasReducedCost(IloNumVar var) const { 
        return _delegate->hasReducedCost(var); 
    }       
    virtual IloNum getReducedCost(IloNumVar var) const { 
        return _delegate->getReducedCost(var); 
    }       
	
	virtual IloBool hasObjValue() const { 
        return _delegate->hasObjValue(); 
    }       
    virtual IloNum getObjValue() const { 
        return _delegate->getObjValue(); 
    }          

    virtual void printsolutionTitle(std::ostream& os, IloBool verbose) const { 
        _delegate->printSolutionTitle(os,verbose); 
    }       

	virtual IloInt getIntervalValue( const IloIntervalVar dvar, const IloModelEvaluatorSolutionGetterBaseI::IntervalField field ) const {
		return _delegate->getIntervalValue(dvar,field);
	}
	
	virtual IloIntervalVar getSequenceValue( const IloIntervalSequenceVar seq, const IloIntervalVar a, const IloModelEvaluatorSolutionGetterBaseI::SequenceDirection direction = IloModelEvaluatorSolutionGetterBaseI::LeftToRight ) const {
		return _delegate->getSequenceValue( seq, a, direction );
	}
	virtual IloInt getCumulValue( const IloCumulFunctionExprArg cumulFct, const IloModelEvaluatorSolutionGetterBaseI::FunctionField field, const IloInt pos = -1 ) const {
	  return _delegate->getCumulValue( cumulFct, field, pos );
	}
	virtual IloInt getCumulValue( const IloStateFunctionExprArg stateFct, const IloModelEvaluatorSolutionGetterBaseI::FunctionField field, const IloInt pos = -1 ) const {
	  return _delegate->getStateValue( stateFct, field, pos );
	}
};


class IloOplLabelCallbackI;
class IloOplConflictIteratorI;
class IloOplRelaxationIteratorI;
class IloOplTuningCallbackI;

class IloAdvancedColumnExtractor;
class IloAdvancedDefaultLPExtractor;
class IloAdvancedExprParser;
class IloAdvancedLogicalExtractor;

class IloOplDisposeListenerI;
class IloOplPostProcessListenerI;


class ILOOPL_EXPORTED IloOplModelI : public IloDestroyableI {
  ILORTTIDECL
    private:
  IloOplModelDefinitionI* _definition;
  IloAlgorithmI* _algorithm;
  IloOplSettingsI* _settings;
  IloOplSolutionGetterI* _defaultSolutionGetter;
  IloOplSolutionGetterI* _defaultSolutionPoolGetter;
  IloOplSolutionGetterI* _zeroSolutionGetter;
  IloOplSolutionGetterI* _statusSolutionGetter;
  IloOplSolutionGetterI* _callbackSolutionGetter;
  IloOplSolutionGetterI* _solutionGetter;
  IloOplDataSourceI* _zeroDataSource;
  IloOplLabelCallbackI* _labelCb;
  IloOplConflictIteratorI* _conflictIterator;
  IloOplRelaxationIteratorI* _relaxationIterator;
  IloNotifiedPlugIn* _changeListener;
  IloArray<IloOplDisposeListenerI*> _disposeListener;
  IloOplPostProcessListenerI* _postProcessListener;
  const IloOplModelI* _outerModel;
  IloOplElementTable* _table;
  IloModelI* _model;
  IloBool _extracted;
  IloOplScripting* _scripting;
  IloBool _isOk;
  IloBool _delayExtraction;
  IloBool _inMain;
  IloBool _mainConstructed;
  IloBool _didWarnNeverUsed;
  IloModelI* _conversionModel;
  const char* _name;
  const char* _path;

  friend class IloOplLoader;
  friend class IloOplPrinterI;
  typedef IloLinkedList<IloOplDataSourceI> DataSources;
  DataSources _dataSources;

  typedef IloLinkedList<IloOplResultPublisherI> Publishers;
  Publishers _publishers;

  //transient
  IloOplLoader* _loader;

  typedef IloStringHashTable<IloOplElementI*> ElementCache;
  ElementCache _cache;

  void init();
  void publishResults();

  void endElementsAndModel();

  const char* _toString;
  const char* toString() const;

  typedef IloStringHashTable<IloOplCustomDataHandlerI*> CustomDataHandlerRegistry;
  CustomDataHandlerRegistry* _customDataHandlerRegistry;
  const char* getPath() const{ return _path; }

  public:
  IloOplModelI(IloEnvI* env, IloOplModelDefinitionI& definition, IloAlgorithmI& algorithm); 
  IloOplModelI(IloEnvI* env, IloOplModelDefinitionI& definition, IloAlgorithmI& algorithm, IloOplSettingsI& settings); 
  ~IloOplModelI();

  IloBool isTemplateModel() const {
    return _outerModel!=0;
  }
  void setPath(const char* p);

  const char* getName() const;
  const char* getSourceName() const;

  const char* resolvePath(const char* name) const;
  std::istream* resolveStream(const char* name) const;

  void setMainConstructed();

  IloOplSettingsI& getSettings() const;
  void setSettings(IloOplSettingsI& settings);

  IloInt printRelaxation(std::ostream& os) const;
  IloInt printConflict(std::ostream& os) const;
  private:
  void makeConflictIterator();
  void makeRelaxationIterator();
  public:
  IloOplConflictIteratorI& getConflictIterator() const;
  IloOplRelaxationIteratorI& getRelaxationIterator() const;

  const IloOplLocationI* findLocation(IloExtractableI* e) const;

  IloOplErrorHandlerI& getErrorHandler() const;
  const IloOplModelDefinitionI& getModelDefinition() const {
    return *_definition;
  }

  void addDataSource(IloOplDataSourceI& source);
  void addSettings(IloOplSettingsI& settings);

  void addResultPublisher(IloOplResultPublisherI* publisher);

  IloBool hasPublishers() const {
    return _publishers.getFirst()!=0;
  }

  void registerCustomDataHandler(const char* customId, IloOplCustomDataHandlerI& handler);
  void unregisterCustomDataHandler(const char* customId);
  IloBool hasCustomDataHandler(const char* customId) const;
  IloOplCustomDataHandlerI& getCustomDataHandler(const char* customId) const;
  void getCustomDataHandlerIds(IloStringArray& ids) const;

  void clearElementCache(IloBool reset = IloTrue);
  void clearElementCache(const char* name);

  void setSolutionGetter(IloOplSolutionGetterI& getter) {
    _solutionGetter = &getter;
    clearElementCache();
  }

  IloBool setPoolSolution(IloInt id);

  void resetSolutionGetter();

  const IloOplSolutionGetterI& getSolutionGetter() const {
    if ( !_defaultSolutionGetter ) {
      const_cast<IloOplModelI*>(this)->_defaultSolutionGetter = newDefaultSolutionGetter();
    }
    return _solutionGetter ? *_solutionGetter : *_defaultSolutionGetter;
  }

  virtual IloCplexI& getCplex() const;

  virtual IloBool hasCplex() const {
    return IloFalse;
  }

  virtual IloCPI& getCP() const;

  virtual IloBool hasCP() const {
    return IloFalse;
  }

  IloBool isUsingCplex() const {
    return hasCplex();
  }
  IloBool isUsingCP() const {
    return hasCP();
  }
  IloBool isGenerated() const {
    return _extracted;
  }

  void warnCplexParams(const char* rcName =0) const;
  IloBool tuneParam(IloCplex::ParameterSetI& fixedSet, IloCplex::ParameterSetI* resultSet, IloOplTuningCallbackI* cb =0);    

  void generate();
  void generate(IloOplLabelCallbackI& callback);
  void loadDataOnly();

  void processDecisionExpr(IloNumExprI* expr, IloOplDecisionExprCallbackI& callback);
  void processDecisionExprSolution(const char* name, IloOplDecisionExprSolutionCallbackI& callback);

  IloBool evaluateConstraint(const IloMapIndexArray& indexArray, const IloMapIndexArray& valueArray, IloConstraintI* ct, IloNum* left, IloNum* mid, IloNum* right);
  IloNum evaluateConstraintLeft(const IloMapIndexArray& indexArray, const IloMapIndexArray& valueArray, IloConstraintI* ct);
  IloNum evaluateConstraintMid(const IloMapIndexArray& indexArray, const IloMapIndexArray& valueArray, IloConstraintI* ct);
  IloNum evaluateConstraintRight(const IloMapIndexArray& indexArray, const IloMapIndexArray& valueArray, IloConstraintI* ct);

  IloModelI& getModel();
  IloModelI& getOuterModel();
  IloObjectiveI* getObjective();
  IloOplElementIteratorI* makeElementIterator(IloBool ownElements =IloTrue);
  IloOplElementI& getElement(const char* name);
  IloBool hasElement(const char* name) const;
  IloOplElementI& getElementWithoutLoad(const char* name);
  void endElement(const char* name);
  IloOplDataElementsI* makeDataElements() const;
  void addToDataElements(IloOplDataElementsI&) const;

  void endModelElements();

  IloStringArray getElementNamesInPostProcessing() const;

  void setZeroSolutionGetter();
  void setStatusSolutionGetter(IloBool status, IloInt warnObsolete =-1);
  void setMIPInfoSolutionGetter(IloCplex::MIPInfoCallbackI& cb);

  void setZeroDataSource();

  void convertAllIntVars();
  void unconvertAllIntVars();

  IloBool isConvertAllIntVars() const {
    return _conversionModel!=0;
  }

  IloOplScriptExpressionI* makeScriptExpression(const char* name, const IloStringArray& paramNames, const char* code);

  IloConstraintI* getConstraintElementItem(const char* name, const IloOplDataElementsI& indices);

  void postProcess();
  void warnNeverUsedElements();

  IloBool hasMain();
  IloInt main();

  
  void printExternalData();
  void printInternalData();
  void printExternalData(std::ostream& os, const char* title =0);
  void printInternalData(std::ostream& os, const char* title =0);
  void printSolution(std::ostream& os, const char* title =0);

  IloExtractableI& findExtractable(const IloOplLocationI& location) const;

  void internalLoad();
  void internalExtract();

  void clearAllCaches();

  public:
  IloBool hasElementTable() const {
    return _table!=0;
  }
  IloOplElementTable& getElementTable() const;

  IloBool hasAlgorithm() const {
    return _algorithm!=0;
  }
  IloAlgorithmI& getAlgorithm() const;
  protected:
  friend class ChangeListenerNotifierPlugIn;
  virtual void removeAlgorithm(IloAlgorithmI* alg);
  public:

  const IloOplLoader& getLoader() const;
  IloOplScripting& getScripting() const;

  private:
  void checkSolutionAvailable() const;

  protected:
  IloOplModelI(IloEnvI* env, IloOplModelDefinitionI& definition);

  virtual IloOplSolutionGetterI* newDefaultSolutionGetter() const;
  virtual IloOplSolutionGetterI* newDefaultSolutionPoolGetter() const;

  void load(IloBool dataOnly =IloFalse);

  DONT_COPY_OPL(IloOplModelI)
    public:
    IloEnvI* getAlgoEnv() const;
  virtual void removePrinter(IloAlgorithmI* alg){}
  void registerDisposeListener(IloOplDisposeListenerI* d);
  void unregisterDisposeListener(IloOplDisposeListenerI* d);
  void registerPostProcessListener(IloOplPostProcessListenerI* d);
  void unregisterPostProcessListener(IloOplPostProcessListenerI* d);
  private:
  void detectWarnings();
  public:
  IloBool isDelayExtraction() { return _delayExtraction; }
  void setDelayExtraction() { _delayExtraction = IloTrue; }
  void unsetDelayExtraction() { _delayExtraction = IloFalse; }
  IloBool internalSolve(); 	
  IloOplLabelCallbackI* getOplLabelCallback() { return _labelCb; }
  void setOplLabelCallback(IloOplLabelCallbackI* c) { _labelCb = c; }
  void setName(const char* name);

  private:
  IloStringArray _opsPaths;
  public:
  void applySettings(const char* projectName) ;
  static void ApplyProjectSettings(const IloStringArray& opsPaths, const IloOplModelI& model, const char* base);
  static void ApplyProjectSettings(std::istream& opsStream, const char* name, const IloOplModelI& model);

  static void ReadProjectSettings(std::istream& opsStream, const char* name, IloCplexI& cplex, IloCplex::ParameterSet parameterSet, IloOplSettingsI& settings);
  static void WriteProjectSettings(IloCplexI& cplex, IloCplex::ParameterSet parameterSet, std::ostream& opsStream, const char* name);
	
  void setOpsPaths( const IloStringArray& opsPaths );
  void applyOplSettings( IloOplSettingsI& settings, const char* projetFilePath );

  void applyOpsSettings(const char* baseDir, const char* opsFile);
  void checkForMaaS(); 

  void saveResultsInText(const char* filePath);
  void exportResultsInText(std::ostream &os);

  ///
  void setProgressListener(IloOplProgressListenerI*);
  void removeProgressListener(IloOplProgressListenerI*);
  IloOplProgressListenerI* getProgressListener() const;


  IloBool isRunOnServer() const;
}; // class IloOplModelI

///////////////////////////////////////////////////////

class ILOOPL_EXPORTED IloOplCplexModelI: public IloOplModelI {
public:
    IloOplCplexModelI(IloEnvI* env, IloOplModelDefinitionI& definition, IloCplexI& cplex);
    IloOplCplexModelI(IloEnvI* env, IloOplModelDefinitionI& definition, IloCplexI& cplex, IloOplSettingsI& settings); 
    virtual ~IloOplCplexModelI();

    IloCplexI& getCplex() const;

    virtual IloBool hasCplex() const {
        return IloTrue;
    }

protected:
    virtual IloOplSolutionGetterI* newDefaultSolutionGetter() const;
    virtual IloOplSolutionGetterI* newDefaultSolutionPoolGetter() const;
};

class ILOOPL_EXPORTED IloOplCpModelI: public IloOplModelI {
    void* _prettyPrinter;
public:
    IloOplCpModelI(IloEnvI* env, IloOplModelDefinitionI& definition, IloCPI& cp);
    IloOplCpModelI(IloEnvI* env, IloOplModelDefinitionI& definition, IloCPI& cp, IloOplSettingsI& settings);
    virtual ~IloOplCpModelI();

    IloCPI& getCP() const;

    virtual IloBool hasCP() const {
        return IloTrue;
    }
	virtual void removeAlgorithm(IloAlgorithmI* alg);

protected:
    virtual IloOplSolutionGetterI* newDefaultSolutionGetter() const;
    virtual IloOplSolutionGetterI* newDefaultSolutionPoolGetter() const;
public:
	virtual void removePrinter(IloAlgorithmI* alg);
};


class ILOOPL_EXPORTED IloOplNopModelI: public IloOplModelI {
public:
    IloOplNopModelI(IloEnvI* env, IloOplModelDefinitionI& definition);
};

#include <ilconcert/ilxml/iloxml.h>

//class XmlBaseConstI: public IloRttiEnvObjectI {
class XmlBaseConstI: public IloDestroyableI {
    ILORTTIDECL

    IloXmlDocument _document;
    const char* _name;

    const char* _lastAttributeValue;

protected:
    XmlBaseConstI(IloEnvI* env, const char* name);

public:
    virtual ~XmlBaseConstI();

    const char* getSourceName() const { 
        return _name; 
    }

  void read(std::istream& opsStream);

    IloXmlElement* getRootElement() const;

    const char* getAttributeValue(IloXmlElement* element, const char* name) const;
	static const char* XML_MODEL;
	static const char* XML_NAME;
	static const char* XML_DATA;
	static const char* XML_SETTING;
	static const char* XML_OPL;
	static const char* XML_CONFIG;
	static const char* XML_TRUE;
	static const char* XML_FALSE;
	static const char* XML_CPLEX;
	static const char* XML_CP;
	static const char* XML_LANGUAGE;
	static const char* XML_SETTINGS;
	static const char* XML_CATEGORY;
	static const char* XML_BOOL;
	static const char* XML_DOUBLE;
	static const char* XML_INT;
	static const char* XML_CHOICE;
	static const char* XML_STRING;
	static const char* XML_TYPE;
	static const char* XML_VALUE;
	static const char* XML_CONFIGS;
	static const char* XML_FOLDER;
	static const char* XML_REF;
	static const char* XML_DEFAULT;
	static const char* XML_RUN;
};


class XmlProjectConstI: public XmlBaseConstI {
    ILORTTIDECL

private:
    IloXmlElement* getElement(const char* path) const;

public:
    XmlProjectConstI(IloEnvI* env, const char* name);
    virtual ~XmlProjectConstI();

    virtual IloXmlElement* getConfigs() const;
    virtual IloXmlElement* getConfig(const char* name) const;
    virtual IloXmlElement* getDefaultConfig() const;

    typedef IloArray<IloXmlElement*> ElementArray;
    virtual void getReferencesByType(IloXmlElement* config, const char* tname, ElementArray& result);
};

class XmlProject2ConstI: public XmlProjectConstI {
    ILORTTIDECL

public:
    XmlProject2ConstI(IloEnvI* env, const char* name);
    virtual ~XmlProject2ConstI();

    virtual IloXmlElement* getConfigs() const;
    virtual IloXmlElement* getConfig(const char* name) const;
    virtual IloXmlElement* getDefaultConfig() const;

    virtual void getReferencesByType(IloXmlElement* config, const char* tname, ElementArray& result);
};


class IloOplScripting;
class IljCommandThunk;
class IljHValuePtr;

class ILOOPL_EXPORTED IloOplScriptThunkI: public IloRttiEnvObjectI {
    ILORTTIDECL
    
	const char* _name;
	const char* _code;
    const IloOplLocationI* _location;
    IljCommandThunk* _thunk;
    IloBool _compiled;
	const IloOplModelI* _opl;
    const char* _ownString;

	void compile() const;

private:
	friend class IloOplScripting;
	IljCommandThunk* getThunk() const;

protected:
	const IloOplModelI& getOplModel() const;

	virtual IloBool usesSharedContext() const {
		return IloFalse;
	}
	virtual IloBool hasParameterNames() const {
		return IloFalse;	
	}
	virtual const IloStringArray getParamterNames() const {
		return 0;
	}

public:
    IloOplScriptThunkI(IloEnvI* env, const char* name, const char* code);
    IloOplScriptThunkI(IloEnvI* env, const char* name, const char* code, const IloOplLocationI& location);
	virtual ~IloOplScriptThunkI();

	const char* getName() const {
		return _name;
	}
	const char* getCode() const {
		return _code;
	}
	const IloOplLocationI& getLocation() const {
        return _location ? *_location : IloOplLocationI::UNKNOWN;
	}

	void attachTo(const IloOplModelI& opl);
	void detachFrom(const IloOplModelI& opl);

	void execute() const;
    const char* eval() const;
    IloInt evalInt() const;
    IloNum evalNum() const;
    IloOplObject evalObject() const;
	void evalInternal(IljHValuePtr& res) const;

	void updateSharedContext(const IloStringArray& names, const IloMapIndexArray& values, IloBool reset) const;
};


class ILOOPL_EXPORTED IloOplScriptExpressionI: public IloOplScriptThunkI {
    ILORTTIDECL
    
	const IloStringArray _paramNames;

	virtual IloBool usesSharedContext() const {
		return IloTrue;
	}
	virtual IloBool hasParameterNames() const {
		return _paramNames.getImpl()!=0 && _paramNames.getSize()>0;	
	}
	virtual const IloStringArray getParamterNames() const {
		return _paramNames;
	}

	void setParameterContext(const IloOplDataElementsI& params) const;
	void resetParameterContext(const IloOplDataElementsI& params) const;

    void setParameterContext(const IloStringArray& paramNames, const IloMapIndexArray& values) const;
    void resetParameterContext(const IloStringArray& paramNames) const;

public:
    IloOplScriptExpressionI(IloEnvI* env, const char* name, const char* code, const IloStringArray& paramNames);
    virtual ~IloOplScriptExpressionI();

    const char* eval(const IloOplDataElementsI& params) const;
    IloInt evalInt(const IloOplDataElementsI& params) const;
    IloNum evalNum(const IloOplDataElementsI& params) const;

    const char* eval(const IloStringArray& paramNames, const IloMapIndexArray& values) const;
    IloInt evalInt(const IloStringArray& paramNames, const IloMapIndexArray& values) const;
    IloNum evalNum(const IloStringArray& paramNames, const IloMapIndexArray& values) const;
    IloOplObject evalObject(const IloStringArray& paramNames, const IloMapIndexArray& values) const;

	using IloOplScriptThunkI::execute;
	using IloOplScriptThunkI::eval;
	using IloOplScriptThunkI::evalInt;
	using IloOplScriptThunkI::evalNum;
	using IloOplScriptThunkI::evalObject;
};

class IloOplElementPairListI;
class ILOOPL_EXPORTED IloOplRelaxationIteratorI: public IloRttiEnvObjectI {
    ILORTTIDECL

      IloOplModelI& _model;

    friend class IloOplElementPairConflictVisitor;
    IloOplElementPairListI* _pairs;
    virtual void visitPairs() = 0;
    virtual void add(IloConstraintI*, IloNum) = 0;

    typedef IloAddressHashTable<IloAnyArray> ExpandedForAllCache;
    ExpandedForAllCache _expandedForAllCache;

public:
    IloOplRelaxationIteratorI(IloOplModelI& model);
    virtual ~IloOplRelaxationIteratorI();

    IloOplModelI& getOplModel() const {
      return _model;
    }

    IloArray<IloObjectBase> getOrMakeItems(IloForAllRangeI* f);
    void cleanForAllCache();
    virtual IloBool hasPossibleConstraints() const =0;

    virtual void recalculate() = 0;

    virtual void firstRelaxed() =0;
    virtual IloBool hasRelaxed() =0;
    virtual void nextRelaxed() =0;

    virtual IloExtractable currentRelaxed() const = 0;
    virtual const char* currentName() const =0;
    virtual IloBool isCurrentRanged()const =0;
    virtual IloNum currentInfeas()const =0;
    virtual IloNum currentLB()const =0;
    virtual IloNum currentUB()const =0;
    virtual IloNum currentRelaxedLB() const = 0;
    virtual IloNum currentRelaxedUB() const = 0;
    virtual IloConstraint currentRelaxedConstraint() const = 0;

	IloInt getIndexerSize();
	IloMapItem getIndexerValue(IloInt i);

        IloInt printRelaxation(std::ostream& os);

    void attach(IloConstraintMap, IloNumMap);
    void attach(IloConstraintMap, IloIntMap);
    void attach(IloConstraint, IloNum);

	void attach(IloIntVarMap intVarMap, IloIntMap intMap);
	void attach(IloIntVarMap intVarMap, IloNumMap numMap);
	void attach(IloNumVarMap numVarMap, IloIntMap intMap);
	void attach(IloNumVarMap intVarMap, IloNumMap numMap);
	void attach(IloNumVar numVar, IloNum numValue);
};

class ILOOPL_EXPORTED IloOplCplexFeasOptIteratorI: public IloOplRelaxationIteratorI {
    ILORTTIDECL

    IloRangeArray _ranges;
    IloNumArray _rangesLB;
    IloNumArray _rangesUB;
    IloForAllRangeArray _forallRanges;
    IloNumArray _forallRangesLB;
    IloNumArray _forallRangesUB;
    IloNumVarArray _dvars;
    IloNumArray _dvarsLB;
    IloNumArray _dvarsUB;
    IloConstraintArray _others;
    IloNumArray _othersPrios;
	IloBool _possible;

    IloBool _calculated;
    IloBool _feasOptStatus;
    IloInt _currentArray;
    IloInt _currentSize;
    IloInt _currentIndex;

    void fillArrays();
    void calculate();

    IloBool hasCurrent() const;
    void nextCurrent();

    virtual void visitPairs();
    virtual void add(IloConstraintI*, IloNum);
    virtual void add(IloNumVarI*, IloNum);

protected:
    IloBool isCurrentRelaxed();

protected:
    

public:
    IloOplCplexFeasOptIteratorI(IloOplModelI& model);
    virtual ~IloOplCplexFeasOptIteratorI();

    virtual IloBool hasPossibleConstraints() const;

    virtual void recalculate();

    virtual void firstRelaxed();
    virtual IloBool hasRelaxed();
    virtual void nextRelaxed();

    virtual IloExtractable currentRelaxed() const;
    virtual const char* currentName() const;
    virtual IloBool isCurrentRanged()const;
    virtual IloNum currentInfeas()const;
    virtual IloNum currentLB()const;
    virtual IloNum currentUB()const;
    virtual IloNum currentRelaxedLB() const;
    virtual IloNum currentRelaxedUB() const;
    virtual IloConstraint currentRelaxedConstraint() const;
};

class IloOplElementPairListI;
class ILOOPL_EXPORTED IloOplConflictIteratorI: public IloRttiEnvObjectI {
    ILORTTIDECL

    IloOplModelI& _model;

    friend class IloOplElementPairConflictVisitor;
    IloOplElementPairListI* _pairs;

    virtual void reattach() = 0;
    virtual void recalculate() = 0;
    virtual void visitPairs() = 0;
    virtual void add(IloConstraintI*, IloNum) = 0;
    virtual void exclude(IloConstraintI*) = 0;

public:

	enum ConflictStatus {
		ConflictExcluded       = 0,
		ConflictPossibleMember = 1,
		ConflictMember         = 2
	};

    IloOplConflictIteratorI(IloOplModelI& model);
    virtual ~IloOplConflictIteratorI();

    IloOplModelI& getOplModel() const {
        return _model;
    }

	virtual IloBool hasPossibleConstraints() const =0;
    virtual IloBool hasFoundConflict() =0;    

    virtual IloInt printConflict(std::ostream& os) = 0;

    virtual void firstInConflict() = 0;
    virtual IloBool isInConflict() const = 0;
    virtual void nextInConflict() = 0;
    virtual IloExtractable currentInConflict() const = 0;
    virtual ConflictStatus currentConflictStatus() const = 0;

    void clearAttachments();
    void attach(IloConstraintMap, IloNumMap);
    void attach(IloConstraintMap, IloIntMap);
    void attach(IloConstraint, IloNum);

    virtual void excludeConflict() = 0;
};

class ILOOPL_EXPORTED IloOplCplexConflictIteratorI: public IloOplConflictIteratorI {
    ILORTTIDECL

    IloBool _attached;
    IloBool _calculated;
    IloBool _refineConflictStatus;

    IloConstraintArray _cts;
    IloNumArray _prios;
    IloCplex::ConflictStatusArray _cstat;
    IloInt _currentIndex;
    IloInt _ctsSize;

    void prepare();
    void refineConflict();

    virtual void reattach() {
        _attached = IloFalse;
        recalculate();
    }
    virtual void recalculate() {
        _calculated = IloFalse;
    }

    virtual void visitPairs();
    virtual void add(IloConstraintI*, IloNum);

    IloInt findIndex(IloConstraintI*) const;
    void exclude(IloConstraintI*);

public:
    IloOplCplexConflictIteratorI(IloOplModelI& model);
    virtual ~IloOplCplexConflictIteratorI();

	virtual IloBool hasPossibleConstraints() const;
    virtual IloBool hasFoundConflict();    

    virtual IloInt printConflict(std::ostream& os);
    void excludeConflict();

    virtual void firstInConflict();
    virtual IloBool isInConflict() const;
    virtual void nextInConflict();
    virtual IloExtractable currentInConflict() const;
    virtual ConflictStatus currentConflictStatus() const;

private:
	ConflictStatus toOplConflictStatus( const IloCplex::ConflictStatusArray& cstat, const IloInt idx ) const;
};

// PHL: 
// 
// - IloOplConflictIteratorI (which is I guess independent from the engine) 
//   uses IloCplex::ConflictStatus (engine related). IloCP uses an IloCP::ConflictStatus, 
//   currently, it is an equvalent enum so I'm reusing IloCplex::ConflictStatus here
//   but that needs to be cleaned
// 

class ILOOPL_EXPORTED IloOplCPConflictIteratorI: public IloOplConflictIteratorI {
    ILORTTIDECL
	
	typedef IloArray<IloCP::ConflictStatus> ConflictStatusArray;

    IloBool _attached;
    IloBool _calculated;
    IloBool _refineConflictStatus;

    IloConstraintArray _cts;
    IloNumArray _prios;
    ConflictStatusArray _cstat;
    IloInt _currentIndex;
    IloInt _ctsSize;

    void prepare();
    void refineConflict();

    virtual void reattach() {
        _attached = IloFalse;
        recalculate();
    }
    virtual void recalculate() {
        _calculated = IloFalse;
    }
    virtual void visitPairs();
    virtual void add(IloConstraintI*, IloNum);

    IloInt findIndex(IloConstraintI*) const;
    void exclude(IloConstraintI*);

public:
    IloOplCPConflictIteratorI(IloOplModelI& model);
    virtual ~IloOplCPConflictIteratorI();

	virtual IloBool hasPossibleConstraints() const;
    virtual IloBool hasFoundConflict();    

    virtual IloInt printConflict(std::ostream& os);
    void excludeConflict();

    virtual void firstInConflict();
    virtual IloBool isInConflict() const;
    virtual void nextInConflict();
    virtual IloExtractable currentInConflict() const;
    virtual ConflictStatus currentConflictStatus() const;

private:
	ConflictStatus toOplConflictStatus( const ConflictStatusArray& cstat, const IloInt idx ) const;
};

class IloOplElementPairListI;
class ILOOPL_EXPORTED IloOplCplexBasisI: public IloEnvObjectI {
    int _status;
    int _nr;
    int _nc;
    int* _rstat;
    int* _cstat;

    void reset();

    friend class IloOplElementPairBasisVisitor;
    IloOplElementPairListI* _pairs;

    IloBool getDefaultBasis(IloCplexI* cplex);
    IloBool setDefaultBasis(IloCplexI* cplex);
    IloBool getPairsBasis(IloCplexI* cplex);
    IloBool setPairsBasis(IloCplexI* cplex);

public:
    explicit IloOplCplexBasisI(IloEnvI* env);
    virtual ~IloOplCplexBasisI();

    IloBool getBasis(IloCplexI* cplex);
    IloBool setBasis(IloCplexI* cplex);

    IloInt getStatus() const {
        return _status;
    }
    
    IloInt getNrows() const {
        return _nr;
    }
    
    IloInt getNcols() const {
        return _nc;
    }

    void attach(IloNumVarMap, IloIntMap);
};

class IloOplElementPairListI;
class ILOOPL_EXPORTED IloOplCplexVectorsI: public IloEnvObjectI {
    int _status;
    int _nr;
    int _nc;
    int _n;
    int* _indices;
    double* _value;

    void reset();
    friend class IloOplElementPairVectorVisitor;
    IloOplElementPairListI* _pairs;
    
    IloBool getDefaultVectors(IloCplexI* cplex);
    IloBool setDefaultVectors(IloCplexI* cplex);
    IloBool getPairsVectors(IloCplexI* cplex);
    IloBool setPairsVectors(IloCplexI* cplex);

public:
    explicit IloOplCplexVectorsI(IloEnvI* env);
    virtual ~IloOplCplexVectorsI();

    IloBool getStart(IloCplexI* cplex);
    IloBool setStart(IloCplexI* cplex);

    IloInt getStatus() const {
        return _status;
    }
    
    IloInt getSize() const {
        return _n;
    }

    IloInt getNrows() const {
        return _nr;
    }
    
    IloInt getNcols() const {
        return _nc;
    }

    void attach(IloNumVarMap, IloNumMap);
    void attach(IloIntVarMap, IloIntMap);
};


class IloAdvLabelCallbackI;
class IloOplLabelCallbackContext;

class ILOOPL_EXPORTED IloOplLabelCallbackI: public IloRttiEnvObjectI {
	ILORTTIDECL

	IloAdvLabelCallbackI* _bridge;
	IloBool _enableScripting;
	IloBool _notifyForall;
	IloOplLabelCallbackContext* _context;
    IloOplElementTable* _table;
    
    typedef IloStringHashTable<IloInt*> CurrentPositionTable;
    CurrentPositionTable _positions;
    IloInt _currentPosition;

public:
	virtual ~IloOplLabelCallbackI();

	virtual void main(const char* label) =0;

	IloConstraint currentConstraint() const;
	IloBool isInForAll() const;
	IloBool isLogical() const;
	IloForAllRange currentForAllRange() const;
	IloOplObject evaluate(IloOplObject) const;

    IloBool hasChangedIndexArray() const;
    IloMapIndexArray currentIndexArray() const;

	IloBool hasChangedIndexNameArray() const;
	IloStringArray currentIndexNameArray() const;

	IloBool hasChangedIndexValueArray() const;
	IloMapIndexArray currentIndexValueArray() const;

    IloInt currentPosition() const {
        return _currentPosition;
    }

	const char* evaluate(const IloOplScriptExpressionI& expr) const;
	IloNum evaluateNum(const IloOplScriptExpressionI& expr) const;
	IloInt evaluateInt(const IloOplScriptExpressionI& expr) const;
	IloOplObject evaluateObject(const IloOplScriptExpressionI& expr) const;

    void internalMain(const char* label);

    IloOplLabelCallbackContext& getContext() const {
        return *_context;
    }
    void resolveConstraints(IloOplElementTable& table);
    
protected:
	IloOplLabelCallbackI(IloEnvI* env, IloBool enableScripting, IloBool notifyForall =IloFalse);

private:
    IloInt incrementPosition(const char* label);

private:
	friend class IloOplModelI;
public:
    void activate(IloAlgorithmI& algorithm);
	void deactivate();
private:
	IloAdvLabelCallbackI& getBridge() const;
};

class IloOplDataHandlerI;

class ILOOPL_EXPORTED IloOplCustomDataHandlerI: public IloRttiEnvObjectI {
  ILORTTIDECL

  const char* _customId;
  IloOplDataHandlerI* _handler;
  const char* _raisedError;

  private:
  friend class IloOplDataElementLoader;

  void setDataHandler(IloOplDataHandlerI& handler) {
    _handler = &handler;
  }
  void resetDataHandler() {
    _handler = 0;
  }
  void throwRaisedError();

  private:
  friend class IloOplModelI;
  void setCustomId(const char* id) {
    _customId = id;
  }

  protected:
  IloOplCustomDataHandlerI(IloEnvI* env);

  const char* getUnresolvedStringElementName(const char* name) const;

  public:
  virtual ~IloOplCustomDataHandlerI();

  virtual IloBool supportsUnresolvedStrings() const;

  virtual void handleConnection(const char* connId, const char* subId, const char* spec) =0;
  virtual void handleReadElement(const char* connId, const char* name, const char* spec) =0;
  virtual void handlePublishElement(const char* connId, const char* name, const char* spec) =0;
  virtual IloBool handleInvoke(const char* name, const char* funcname) =0;
  virtual void closeConnections();

  IloOplDataHandlerI& getDataHandler() const;
  void raiseError(const char* message);

};// class IloOplCustomDataHandlerI

class IloNumExprEvalCallbackI;
class IloOplDecisionExprCallbackContext;

class ILOOPL_EXPORTED IloOplDecisionExprCallbackI: public IloRttiEnvObjectI {
	ILORTTIDECL

	IloNumExprEvalCallbackI* _bridge;
	IloOplDecisionExprCallbackContext* _context;

public:
	virtual ~IloOplDecisionExprCallbackI();

    virtual void process(const IloOplModelI& opl, IloNumExprI* dexpr);

    virtual IloBool startDecisionExpr(const char* dexprName, IloNum coef, IloNumExprArg expr) =0;
    virtual IloBool startSum(IloNum coef, IloNumExprArg expr) =0;
    virtual void addTerm(IloNum coef, IloNumExprArg expr) =0;
    virtual void endSum() =0;
    virtual void endDecisionExpr(const char* dexprName) =0;

    IloOplDecisionExprCallbackContext& getContext() const {
		return *_context;
	}

	IloMapIndexArray currentIndexArray() const;
	IloOplObject evaluate(IloOplObject) const;

    IloBool hasChangedIndexNameArray() const;
	IloStringArray currentIndexNameArray() const;

	IloBool hasChangedIndexValueArray() const;
	IloMapIndexArray currentIndexValueArray() const;

	const char* evaluate(const IloOplScriptExpressionI& expr) const;
	IloNum evaluateNum(const IloOplScriptExpressionI& expr) const;
	IloInt evaluateInt(const IloOplScriptExpressionI& expr) const;
	IloOplObject evaluateObject(const IloOplScriptExpressionI& expr) const;

protected:
	IloOplDecisionExprCallbackI(IloEnvI* env, IloBool enableScripting);
};

class ILOOPL_EXPORTED IloOplDecisionExprSolutionCallbackI: public IloOplDecisionExprCallbackI {
	ILORTTIDECL

    const IloOplSolutionGetterI* _getter;

public:
	virtual ~IloOplDecisionExprSolutionCallbackI();

    virtual void process(const IloOplModelI& opl, IloNumExprI* dexpr);

    virtual IloBool startDecisionExpr(const char* dexprName, IloNum coef, IloNumExprArg expr);
    virtual IloBool startSum(IloNum coef, IloNumExprArg expr);
    virtual void addTerm(IloNum coef, IloNumExprArg dexpr);

    virtual IloBool startDecisionExpr(const char* dexprName, IloNum value) =0;
    virtual IloBool startSum(IloNum value) =0;
    virtual void addTerm(IloNum value) =0;
    virtual void endSum() =0;
    virtual void endDecisionExpr(const char* dexprName) =0;

protected:
	IloOplDecisionExprSolutionCallbackI(IloEnvI* env, IloBool enableScripting);
};


class IloOplModel;

class ILOOPL_EXPORTED IloOplTuningCallbackI: public IloRttiEnvObjectI {
	ILORTTIDECL

    IloTimer _timer;
    IloNum _generateProgress;
    IloNum _tuneProgress;

public:
    explicit IloOplTuningCallbackI(IloEnvI* env);
    ~IloOplTuningCallbackI();

    IloNum getProgress() const;
    IloNum getRemaining() const;

protected:
    virtual void notifyGenerate(IloOplModel opl, const char* rcName) =0;
    virtual void notifyStart(IloCplex cplex) =0;
    virtual void notifyProgress() =0;
    virtual void notifyEnd(IloCplex cplex) =0;

private:
    friend class IloOplModelI;
    friend class IloOplProjectI;
    friend class IloOplTuningCallbackBridgeI;

    void setGenerateProgress(IloNum progress);
    void setTuneProgress(IloNum progress);
};


class IloOplTuningCallbackBridgeI: public IloCplex::TuningCallbackI {
    IloOplTuningCallbackI* _cb;
public:
    IloOplTuningCallbackBridgeI(IloEnv env, IloOplTuningCallbackI& cb);
    void main();
    IloCplex::CallbackI* duplicateCallback() const;
};

// ---------------------------------------------------------------------------

inline IloOplSolutionGetterI::IloOplSolutionGetterI(IloEnvI* env, const IloOplModelI& opl) 
:IloRttiEnvObjectI(env), _algorithm(0), _opl(&opl), _bridge(0), _depth(0) {
    _algorithm = _opl->hasAlgorithm() ? &_opl->getAlgorithm() : 0;
}

inline IloBool IloOplSolutionGetterI::isConvertAllIntVars() const {
    return _opl && _opl->isConvertAllIntVars();
}



#ifdef _WIN32
#pragma pack(pop)
#endif

#endif

