
function TotalCost = CostCalc(StartPeriod,EndPeriod,SumVect,Cost)

    TotalCost = Cost(StartPeriod:EndPeriod)'*SumVect(StartPeriod:EndPeriod);


    %for c = StartPeriod:EndPeriod

    %    TotalCost = TotalCost + SumVect(c)*Cost(c);

    %end

    disp(TotalCost);

end