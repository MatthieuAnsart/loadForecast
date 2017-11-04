function CostRec = CostRec(CostType,StartTime,Period,Interval,CostInput)

    CostRec = zeros(Period*24*60/Interval,1);

    if CostType == 1    % two tier pricing

        for j = 1:(Period*24*60/Interval)
            
            if floor((StartTime + j - 1)/(24*60/Interval)) == (StartTime + j - 1)/(24*60/Interval)    
                
                CostRec(j) = CostInput(StartTime + j - 1 - (24*60/Interval)*(floor((StartTime + j - 1)/(24*60/Interval)) - 1));
            
            else
                
                CostRec(j) = CostInput(StartTime + j - 1 - (24*60/Interval)*floor((StartTime + j - 1)/(24*60/Interval)));
            
            end 
            
        end

    elseif CostType == 2    %dynamic pricing

        CostRec(:) =  CostInput(StartTime:StartTime+Period*24*60/Interval-1);

    else

    end
    
end
