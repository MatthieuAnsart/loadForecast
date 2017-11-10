// -------------------------------------------------------------- -*- C++ -*-
// File: ./include/ilopl/ilooplprofileri.h
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

#ifndef __OPL_ilooplprofileriH
#define __OPL_ilooplprofileriH

#ifdef _WIN32
#pragma pack(push, 8)
#endif

#ifndef __OPL_ilosysH
# include <ilopl/ilosys.h>
#endif
#ifndef __OPL_ilooplexceptionH
# include <ilopl/ilooplexception.h>
#endif

#ifndef __CONCERT_iloenvH
# include <ilconcert/iloenv.h>
#endif

#include <iostream>

///////////////////////////////////////////////////////

class IloOplLocationI;
class IloCplexI;
class IloCPI;

class IloOplProgressListenerI;
///////////////////////////////////////////////////////


class ILOOPL_EXPORTED IloOplProfilerI: public  IloRttiEnvObjectI {
  ILORTTIDECL

  IloBool _ignoreUserSection;
  void* _attachedTo;
  void* _attachedToCP;

  public:

  
  typedef enum {
    SECTION_UNDEFINED= -1, // dummy value.
    SECTION_READ_DEFINITION,
    SECTION_LOAD_MODEL,
    SECTION_LOAD_DATA,
    SECTION_PRE_PROCESSING,
    SECTION_ASSERT,
    SECTION_EXTRACT,
    SECTION_OBJECTIVE,
    SECTION_POST_PROCESSING,
    SECTION_PUBLISH_RESULTS,
    SECTION_FORCE_USAGE,
    SECTION_USER,
    SECTION_CPLEX,
    SECTION_CP,
    SECTION_ODM,
    SECTION_END,
    SECTION_OTHER,
    INIT,
    EXECUTE,
    ROOT
  } Section;


  
  class InfoProvider {
  protected:
    InfoProvider() {}

  public:
    virtual ~InfoProvider() {}
    virtual IloNum getCurrentTime() =0;
    virtual void getCurrentMemory(IloInt& peak, IloInt& used) =0;
    virtual void pause() =0;
    virtual void resume() =0;

    virtual IloInt getTotalPausedMemory() const {
      return 0;
    }
  };

  private:
  typedef IloArray<InfoProvider*> ProviderStack;
  ProviderStack _providerStack;
  IloInt _providerStackTop;

  protected:
  explicit IloOplProfilerI(IloEnvI* env);

  IloBool isIgnoreSection(Section section) const;

  virtual IloBool isInSection(Section section) const =0;

  void attachToAlgorithm(IloAlgorithmI& algorithm);
  void detachFromAlgorithm(IloAlgorithmI& algorithm);

  public:
  virtual ~IloOplProfilerI();

  void setIgnoreUserSection(IloBool ignore) {
    _ignoreUserSection = ignore;
  }
  IloBool isIgnoreUserSection() const {
    return _ignoreUserSection;
  }

  virtual void printReport(std::ostream& os) =0;
    
  virtual void enterSection  (Section section, const char* name) =0;
  virtual void reenterSection(Section section, const char* name) =0;
  virtual void exitSection   (Section section) =0;

  virtual void enterInit(const char* nam) =0;
  virtual void exitInit (const char* name) =0;

  virtual void enterExecute(const char* name) =0;
  virtual void exitExecute (const char* name) =0;
    
  virtual void setLocation(const IloOplLocationI& location) =0;

  void enterSectionIfNeeded(Section section, const char* name);
  void exitSectionIfNeeded(Section section);
  void exitSectionUserIfNeeded()   { exitSectionIfNeeded(SECTION_USER); }
  void exitSectionOtherIfNeeded()  { exitSectionIfNeeded(SECTION_OTHER);}

  void attachTo(IloCplexI& cplex);
  void detachFrom(IloCplexI& cplex);

  void attachTo(IloCPI& cp);
  void detachFrom(IloCPI& cp);

  IloBool isAttachedTo(void* other) {
    return _attachedTo && _attachedTo == other;
  }

  void pushProvider(InfoProvider& provider);
  void popProvider(InfoProvider& provider);
  InfoProvider& getProvider() const {
    return *_providerStack[_providerStackTop];
  }

  void pauseProvider() const;
  void resumeProvider() const;

  static IloInt GetCurrentProcessMemory();
  static IloNum GetCurrentProcessTime();
  static IloNum GetCurrentUserTime();

  static const char* GetSectionPropertyName(Section section);
  static std::string GetSectionLocalizedString(Section section);

  virtual IloBool acceptsListener() const;
  virtual void setListener(IloOplProgressListenerI*, IloBool ownership);
  virtual IloOplProgressListenerI* getListener() const;
  virtual IloBool ownsListener() const;

  IloBool hasListener() const { return 0 != getListener();}
  void clearListener() { 
    setListener(0, IloFalse);
  }

  private:
  DONT_COPY_OPL(IloOplProfilerI)
};

typedef IloOplProfilerI::Section IloOplProfilerSection;

class ILOOPL_EXPORTED IloOplDefaultProfilerI: public IloOplProfilerI {
    ILORTTIDECL

public:
  class Node: public IloEnvObjectI {
  protected:
    Node(IloEnvI* env):IloEnvObjectI(env) {
    }
    virtual ~Node() {
    }
  public:
    virtual Section getSection() const =0;
    virtual const char* getName() const =0;

    virtual IloBool hasLocation() const =0;
    virtual const IloOplLocationI& getLocation() const =0;

    virtual IloNum getTime() const =0;
    virtual IloNum getSelfTime() const =0;
    virtual IloInt getPeakMemory() const =0;
    virtual IloInt getLocalMemory() const =0;
    virtual IloInt getCount() const =0;
    virtual IloInt getNodes() const =0;

    virtual IloBool isRoot() const =0;
    virtual const Node& getParent() const =0;
    virtual IloBool isLeaf() const =0;
    virtual const Node& getFirstChild() const =0;
    virtual IloBool isLast() const =0;
    virtual const Node& getNext() const =0;


    const char* getSectionPropertyName() const;

  };

protected:
    Node* _root;
    Node* _current;
    std::ostream* _traceStream;
    InfoProvider* _envProvider;

    void open();
    void close();


protected:
    virtual void traceEnter(Section section, const char* qualifier);
    virtual void traceExit (Section section, const char* qualifier);

    std::ostream& getTraceStream() const;

    virtual IloBool isInSection(Section section) const;

    void enterSectionInternal(Section section, const char* name);
    void exitSectionInternal (Section section, const char* name=0);

public:
    explicit IloOplDefaultProfilerI(IloEnvI* env);
    IloOplDefaultProfilerI(IloEnvI* env, std::ostream& traceStream);
    virtual ~IloOplDefaultProfilerI();

    virtual void printReport(std::ostream& os);
    IloBool hasRoot() const;
    const Node& getRoot() const;

    virtual void enterSection(Section section, const char* name);
    virtual void reenterSection(Section section, const char* name);
    virtual void exitSection(Section section);

    virtual void enterInit(const char* name);
    virtual void exitInit (const char* name);

    virtual void enterExecute(const char* name);
    virtual void exitExecute(const char* name);

    virtual void setLocation(const IloOplLocationI& location);
};

///


class IloOplProgressListenerI  {
  // for warppers, BEURKKK
public:
  IloOplProgressListenerI();


public:
  virtual ~IloOplProgressListenerI();

  
  virtual void notifyEnterSection(IloOplProfilerI::Section section, const char* qualifier, IloNum time);

  
  virtual void notifyExitSection (IloOplProfilerI::Section section, const char* qualifier, IloNum time);
  
};//abstract class IloOplProgressListenerI



class IloOplProgressListener {
  IloOplProgressListenerI* _impl;
public:
  
  IloOplProgressListener(IloOplProgressListenerI* impl);

  
  void end();

  
  IloOplProgressListenerI* getImpl() const { return _impl;}
  
  void setImpl(IloOplProgressListenerI* impl) { _impl= impl;}

  
  void notifyEnterSection(IloOplProfilerI::Section section, const char* qualifier, IloNum time);

  
  void notifyExitSection(IloOplProfilerI::Section section, const char* qualifier, IloNum time);
};




///

class IloOplProgressListenerLoggerI : public IloOplProgressListenerI {
  std::ostream&  _os;
  IloBool        _doPrintExitLines;
  int            _indentLevel; // do not indent if <= 0
  const char*    _timeFormat;
  std::string    _enterString;
  std::string    _exitString;

  IloUInt _totalCounter;
  IloUInt _numberOfEnters;
  IloInt  _level;

  
protected:
  static IloNum RoundTime(IloNum time, int nbDigits=3);
  void indent(std::ostream& os, IloUInt depth) const;

  IloUInt getNumberOfEnters() const;
  IloUInt getNumberOfExits() const;

  static const char* ToString(IloOplProfilerSection section);


  /////////////////////////////////
  struct LogEntry {
    IloBool isEnter;
    IloOplProfilerSection section;
    IloUInt counter;
    IloUInt depth;
    const char* qualifier; // can be 0
    IloNum time; // -1 if absent.

    LogEntry(IloBool isEnter_, IloOplProfilerSection section_, IloUInt counter_, IloUInt depth_, const char* qualifier_, IloNum time_)
      : isEnter(isEnter_)
      , section(section_)
      , counter(counter_)
      , depth(depth_)
      , qualifier(qualifier_)
      , time(time_)
    {}
    
    const char* toString() const { return ToString(section);}

  }; // nested struct LogEntry
  /////////////////////////////////

  void reset();
  void printLogEntry(const LogEntry& entry) const;
  IloBool doIndent() const {
    return _indentLevel >0;
  }

  static const char* DEFAULT_TIME_FORMAT;
  static const char* DEFAULT_ENTER_SYMBOL;
  static const char* DEFAULT_EXIT_SYMBOL;

public:
  /////////////////////////////////
  struct TimeFormatter {
    const char* _format;
    IloNum      _time;
    TimeFormatter(const char* fmt, IloNum atime)
      : _format(fmt), _time(atime) {}
    void display(std::ostream& os) const;
  };// nested struct to wrap time format.
  /////////////////////////////////

  IloOplProgressListenerLoggerI();
  IloOplProgressListenerLoggerI(std::ostream& os, IloBool printExits, int indent=0);
  ~IloOplProgressListenerLoggerI();

  void setPrintExitLines(IloBool printExits);
  void setIndent(IloBool indent);
  void setEnterString(const char* enter);
  void setExitString(const char* exit);
  
  virtual void notifyEnterSection(IloOplProfilerI::Section section, const char* qualifier, IloNum time);
  virtual void notifyExitSection (IloOplProfilerI::Section section, const char* qualifier, IloNum time);

};

/////////////////////////////////


class IloOplProfilerProgressAdapterI : public IloOplDefaultProfilerI {
  ILORTTIDECL
  IloInt  _numberOfReenters;
  IloOplProgressListenerI* _listener;
  IloBool _ownsListener;

protected:
  void clearListener();
  IloNum getTime() const;

  virtual void traceEnter(Section section, const char* qualifier);
  virtual void traceExit (Section section, const char* qualifier);

  explicit IloOplProfilerProgressAdapterI(IloEnvI* env);

public:
  ~IloOplProfilerProgressAdapterI();

  static IloOplProfilerProgressAdapterI* New(IloEnv);
  
  virtual void reenterSection(Section section, const char* name);
  virtual void printReport(std::ostream& os);

  virtual IloBool acceptsListener() const;

  
  virtual void setListener(IloOplProgressListenerI* listener, IloBool ownership);

  
  virtual IloOplProgressListenerI* getListener() const;

  virtual IloBool ownsListener() const { return _ownsListener;}

};

//
// specialized class which automtically connects a logger listener
// and owns it.
class IloOplProfilerLoggerI : public IloOplProfilerProgressAdapterI {
  ILORTTIDECL
  explicit IloOplProfilerLoggerI(IloEnvI* env);
public:
  static IloOplProfilerLoggerI* New(IloEnv);
  
};


#ifdef _WIN32
#pragma pack(pop)
#endif

#endif

