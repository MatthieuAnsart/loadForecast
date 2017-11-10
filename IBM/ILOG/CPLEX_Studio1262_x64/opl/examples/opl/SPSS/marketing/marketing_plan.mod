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

tuple offers_tuple {
   int customer_id;
   string Product_1;
   float Confidence_1;
   string Product_2;
   float Confidence_2;
}
{offers_tuple} offers  = ...;

tuple channel {
   key string name;
   float cost;
   float factor;
};
{channel} channels = ...;

{string} products = ...;
float productValue[products] = ...;
float budgetShare[products] = ...;

float availableBudget = ...;

dvar int channelVar[offers][products][channels] in 0..1;
dvar int+ totaloffers;
dvar float budgetSpent;

maximize
// Expected return
   sum(o in offers, p in products, c in channels : o.Product_1 == p)
      channelVar[o,p,c] * c.factor * productValue[p] * o.Confidence_1 +
   sum(o in offers, p in products, c in channels : o.Product_2 == p)
      channelVar[o,p,c] * c.factor * productValue[p] * o.Confidence_2;
  
subject to {
// Only 1 product is offered to each customer     
   forall(o in offers)
      ctLimitoffers: 
      sum(p in products, c in channels) channelVar[o,p,c] <= 1;
     
   totaloffers == sum(o in offers, p in products, c in channels) channelVar[o,p,c];
   budgetSpent == sum(o in offers, p in products, c in channels) channelVar[o,p,c]*c.cost;

// Balance the offers among products   
   forall(p in products)
      ctShare:
      sum(o in offers, c in channels) channelVar[o,p,c] <= budgetShare[p] * totaloffers;
            
// Do not exceed the budget
   ctBudget:
   sum(o in offers, p in products, c in channels) channelVar[o,p,c]*c.cost <= availableBudget;          

};

tuple reportT{
	int customer_id;
	string product;
	string channel;
}
{reportT} report = {<o.customer_id, p, c.name> | o in offers, p in products, c in channels : channelVar[o][p][c]==1};

assert card(report) == totaloffers;
execute {
   writeln();
   writeln();
   writeln("Marketing plan has ", totaloffers, " offers costing ", budgetSpent);
   writeln();
   writeln("Selected offers =");
   writeln(report);
}


