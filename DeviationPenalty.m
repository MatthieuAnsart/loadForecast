function Penalty = DeviationPenalty(gamma, rho, LoadPrev, LoadNow, PVPrev, PVNow, DecisionPrev, DecisionNow, NumOut)

    % Penalty = abs( DecisionNow - DecisionPrev ) ./ max( gamma*abs(LoadNow - LoadPrev) , rho ); 
    % But Yalmip doesn't take it...
    Penalty = 0;
    
    for loop = 1: NumOut - 1
    
        Penalty = Penalty + abs( DecisionNow(loop) - DecisionPrev(loop) ); 
    
    end
    
    Penalty = Penalty/(NumOut-1);
    
    Penalty = (1/rho) * Penalty/(gamma*sum(abs(LoadNow - LoadPrev) + abs(PVNow - PVPrev)) + 1);
    
    fprintf('\n****Penalty****\n')
    Penalty
    fprintf('***************\n')
    
    clear loop;
    
end