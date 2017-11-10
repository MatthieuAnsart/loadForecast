// --------------------------------------------------------------------------
// Licensed Materials - Property of IBM
//
// 5725-A06 5725-A29 5724-Y48 5724-Y49 5724-Y54 5724-Y55
// Copyright IBM Corporation 1998, 2015. All Rights Reserved.
//
// Note to U.S. Government Users Restricted Rights:
// Use, duplication or disclosure restricted by GSA ADP Schedule
// Contract with IBM Corp.
// --------------------------------------------------------------------------

tuple OilTable_tuple {
	string name;
	float capacity;
	float price;
	float octane;
	float lead;
}
{OilTable_tuple} OilTable  = ...;


tuple GasTable_tuple {
	string name;
	float demand;
	float price;
	float octane;
	float lead;
}
{GasTable_tuple} GasTable  = ...;

execute {
	writeln("OilTable=",OilTable);
	writeln("GasTable=",GasTable);
}


{string} Gasolines = {n | <n,c,p,o,l> in GasTable};
{string} Oils = {n | <n,d,p,o,l> in OilTable};


tuple gasType {
  float demand;
  float price;
  float octane;
  float lead;
}

tuple oilType {
  float capacity;
  float price;
  float octane;
  float lead;
}
gasType Gas[Gasolines] = [ n : <c,p,o,l> | <n,c,p,o,l> in GasTable];
oilType Oil[Oils] = [ n : <d,p,o,l> | <n,d,p,o,l> in OilTable];
float MaxProduction = 14000;
float ProdCost = 4;


dvar float+ a[Gasolines];
dvar float+ Blend[Oils][Gasolines];


maximize
  sum( g in Gasolines , o in Oils )
    (Gas[g].price - Oil[o].price - ProdCost) * Blend[o][g] 
    - sum(g in Gasolines) a[g];
subject to {
  forall( g in Gasolines )
    ctDemand: 
      sum( o in Oils ) 
        Blend[o][g] == Gas[g].demand + 10*a[g];
  forall( o in Oils )
    ctCapacity:   
      sum( g in Gasolines ) 
        Blend[o][g] <= Oil[o].capacity;
  ctMaxProd:  
    sum( o in Oils , g in Gasolines ) 
      Blend[o][g] <= MaxProduction;
  forall( g in Gasolines )
    ctOctane: 
      sum( o in Oils ) 
        (Oil[o].octane - Gas[g].octane) * Blend[o][g] >= 0;
  forall( g in Gasolines )
    ctLead:
      sum( o in Oils ) 
        (Oil[o].lead - Gas[g].lead) * Blend[o][g] <= 0;
}

execute DISPLAY_REDUCED_COSTS{
  for( var g in Gasolines ) {
    writeln("a[",g,"].reducedCost = ",a[g].reducedCost);
  }
}