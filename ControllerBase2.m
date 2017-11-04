%% Privacy counter setup %%

temp1 = binvar(1,1);

%%%%%%%%%%%%%%%% bruit

for t = 1 : length(LoadInput)
    if LoadInput(t) > xbinsize * m
        LoadInput(t) = xbinsize * m;
    elseif LoadInput(t)<0
        LoadInput(t)=0;        
    end
end

clear t;

xcount = xcountrec; % x count across prediction horizon, includes predicted values to be purged
xycountc(m,n) = temp1; % changes to xy count

for t = 1 : NumOut
    %     temp = binx(E_load(t,1), xbinsize, m);
    temp = binx(LoadInput(act+t,1), xbinsize, m);
    % pas sur pour le act
    xycountc(temp,:) = xycountc(temp,:) + Z_ijt(t,:);   %to change temporal series to count space. i.e. a particular xycount may occur twice in the horizon
    xcount(temp,1) = xcount(temp,1) + 1;
end

xycountc(m,n) = xycountc(m,n) - temp1;

clear temp temp1;
%% Optimisation %%

bound = zeros(n,1);

for j = 1:n
    bound(j,1) = ybinsize * (j-1);
end

bound(1) = - 0.0000009;

constraints = [ 0 <= E_batt <= BattCap,...
    0 <= P_battD <= BattLimD*(1-BattOp),...
    0 <= P_battC <= BattLimC*BattOp,...
    E_grid == (E_load - P_battD*TimeStep + P_battC*TimeStep - P_pv*TimeStep),...
    E_batt(1) == P_act(act,5),... % initial battery value
    ];

ObjectiveFun = 0;
constraints2 = [];
constraints3 = [];
constraints4 = [];

for t = 1 : NumOut
    
    constraints2 = [constraints2, E_batt(t+1) ==  (E_batt(t) - BattEffD*P_battD(t)*TimeStep + BattEffC*P_battC(t)*TimeStep)];
    
    constraints3 = [constraints3, Z_ijt(t,:)*(bound+0.0000009) <= E_grid(t,1) <= Z_ijt(t,:)*(bound+ybinsize)]; % the subtraction is to deal with Yalmip limitations on binary variables
    
    constraints4 = [constraints4, sum(Z_ijt(t,:)) == 1];
    
    ObjectiveFun = ObjectiveFun + CostOn*(E_load(t) - P_battD(t)*TimeStep + P_battC(t)*TimeStep - P_pv(t)*TimeStep)*cost(t);% + 1e-8*P_battC(t)^2 ;%battery smoothing needed for 0 privacy case
    
end

%     ObjectiveFun = CostOn*ObjectiveFun;

if act == 1
    ObjectiveFun = (1/NumOut)*ObjectiveFun + ...
        mu*PrivacyFuncBase2(1, xcount, ycountrec, xycountrec, xycountc, N_window, m, n, err1, err2, err3);
else
    ObjectiveFun = (1/NumOut)*ObjectiveFun + ...
        mu * (PrivacyFuncBase2(1, xcount, ycountrec, xycountrec, xycountc, N_window, m, n, err1, err2, err3) + ...
        DeviationPenalty(gamma, rho, P_out(2:NumOut,6,act-1), E_load(1:NumOut-1), P_out(2:NumOut, 7,act-1), P_pv(1:NumOut-1), ...
        P_out(2:NumOut,4,act-1), (P_battC(1:NumOut-1)-P_battD(1:NumOut-1)), NumOut));
end

[model,recoverymodel,diagnostic,internalmodel] = export([constraints constraints2 constraints3 constraints4], ObjectiveFun, options);


sol = optimize([constraints constraints2 constraints3 constraints4], ObjectiveFun, options);

%% Results and Environment Update %%

% P_out(1:NumOut,1,act) = E_load(:) - double(P_battD(:))*TimeStep + double(P_battC(:))*TimeStep - P_pv(:)*TimeStep;
P_out(1:NumOut,1,act) = LoadInput(act:NumOut+act-1) - double(P_battD(:))*TimeStep + double(P_battC(:))*TimeStep - P_pv(:)*TimeStep;
P_out(1:NumOut,2,act) = double(P_battD(:));
P_out(1:NumOut,3,act) = double(P_battC(:));
% P_out(1:NumOut,6,act) = E_load(:);
P_out(1:NumOut,6,act) = LoadInput(act:NumOut+act-1);
P_out(1:NumOut,7,act) = P_pv(:);

for k = 1 : NumOut
    
    if round(P_out(k,3,act),6) == 0
        
        P_out(k,4,act) = -P_out(k,2,act); %Discharge denoted by negative. View seen from grid
        
    elseif round(P_out(k,2,act),6) == 0
        
        P_out(k,4,act) = P_out(k,3,act);
        
    else
        
        display('non-zero solution for both charge and discharge');
        P_out(k,4,act) = P_out(k,3,act) - P_out(k,2,act);
        assert(or(round(P_out(k,2,act),4)==0,round(P_out(k,3,act),4) == 0), 'non-zero solution for both charge and discharge');
        %break;
        
    end
    
end


P_out(1:NumOut,5,act) = double(E_batt(1:NumOut));

P_act(act,1) = act;
% P_act(act,2) = round(P_out(1,1,act),6);     % Needs to be revised if predictions ~= realisations
P_act(act,2) = P_out(1,1,act);
P_act(act,3) = LoadInput(act - 1 + StartTime);
% P_act(act,4) = P_out(1,4,act);
P_act(act,4) = P_out(1,3,act) - P_out(1,2,act);

if act < round(Period*24*60/Interval)
    
    P_act(act+1,5) = round(P_act(act,5) - BattEffD*P_out(1,2,act)*TimeStep + BattEffC*P_out(1,3,act)*TimeStep,6);
        
    if P_act(act+1,5) < 0
        fprintf('\nNext Time Step Battery State (Pre-Round):')
        disp(P_act(act+1,5));
        P_act(act+1,5) = round(P_act(act+1,5),5);
    end
    
end

%% Privacy Sanity Check %%
fprintf('\n****************************************************************************\n ');
fprintf('\nStep I(Y;X): ');

ycount = [];

for j = 1 : n
    ycount(j,1) = ycountrec(j,1) + sum(xycountc(:,j));
end

%%%%%%%%%%%%%%%%%%%comprends pas ce que l on fait au dessus, pourquoi
%%%%%%%%%%%%%%%%%%%rajouter xycountc

P_act(act,6) = PrivacyEval(xycountrec+value(xycountc), xcount, value(ycount), N_window, m, n, err1, err2, err3);
disp(P_act(act,6));
fprintf('\nStep I(Y;X)_obj: ');
PFunc = PrivacyFuncBase2(1, xcount, ycountrec, xycountrec, value(xycountc), N_window, m, n, err1, err2, err3);
disp(PFunc);

%%%%%%%%%%%%
%     assert(PrivacyEval(xycountrec+value(xycountc), xcount, value(ycount), N_window, m, n, err1, err2, err3)...
%         == value(PrivacyEval(xycountrec+value(xycountc), xcount, ycount, N_window, m, n, err1, err2, err3)),...
%             'Privacy Evaluation Error');
%
%     assert(round(PrivacyFuncBase2(1, xcount, ycountrec, xycountrec, value(xycountc), N_window, m, n, err1, err2, err3),8)...
%         == round(value(PrivacyFuncBase2(1, xcount, ycountrec, xycountrec, xycountc, N_window, m, n, err1, err2, err3)),8),...
%             'Privacy Function Error');
%%%%%%%%%%%%

% bins cannot be reextracted due to the small delta that was imposed as
% a constraint for strictly less than unless we make a hack

P_act(act,7) = value(PFunc);

% clear PFunc ycount;

%figure();
%XYPlot(1,value(xycountc)+ xycountrec,m,n);

%% History Update %%

% Remove last value
xycountrec(xhist(1,1),yhist(1,1)) = xycountrec(xhist(1,1),yhist(1,1)) - 1;
ycountec(yhist(1,1),1) = ycountrec(yhist(1,1),1) - 1;
xcountrec(xhist(1,1),1) = xcountrec(xhist(1,1),1) - 1;

% for i = 1:n
%     
%     if round(value(Z_ijt(1,i))) == 1
%         y_controller(act) = i;
%         break;
%     end
% end


%Add latest value
if P_act(act,2) > ybinsize * n
    P_act(act,2) = xbinsize * m;
end

tempx = binx(LoadInput(act + StartTime - 1), xbinsize, m);
tempy = binx(P_act(act,2), ybinsize, n); % use this if x is not perfectly known
% tempy = y_controller(act);      % only true for perfect prediction case


xycountrec(tempx,tempy) = xycountrec(tempx,tempy) + 1;
ycountrec(tempy,1) = ycountrec(tempy,1) + 1;
xcountrec(tempx,1) = xcountrec(tempx,1) + 1;

%Update History Logs
temphist = xhist(1,2:HistWin);
xhist(1,1:HistWin-1) = temphist;
xhist(1,HistWin) = tempx;
temphist = yhist(1,2:HistWin);
yhist(1,1:HistWin-1) = temphist;
yhist(1,HistWin) = tempy;

%% Privacy Evaluation Update %%

%Add Latest Value to Global Records
% seems like the first values of x and y global are wasted
xglobal(N_window+act) = tempx;
yglobal(N_window+act) = tempy;

% Remove last value
xycounteval(xglobal(act),yglobal(act)) = xycounteval(xglobal(act),yglobal(act)) - 1;
ycounteval(yglobal(act),1) = ycounteval(yglobal(act),1) - 1;
xcounteval(xglobal(act),1) = xcounteval(xglobal(act),1) - 1;

%Add latest values
xycounteval(tempx,tempy) = xycounteval(tempx,tempy) + 1;
ycounteval(tempy,1) = ycounteval(tempy,1) + 1;
xcounteval(tempx,1) = xcounteval(tempx,1) + 1;

fprintf('\n----------------------------------------------------------------------------\n');

fprintf('\nI(Y;X): ');
P_act(act,8) = PrivacyEval(xycounteval, xcounteval, ycounteval, N_window, m, n, err1, err2, err3);
disp(P_act(act,8));
%P_act(act,8) = PEval_act;

if act >= CumulativeStart
    
    %Add latest values
    xycount_cumulative(tempx,tempy) = xycount_cumulative(tempx,tempy) + 1;
    ycount_cumulative(tempy,1) = ycount_cumulative(tempy,1) + 1;
    xcount_cumulative(tempx,1) = xcount_cumulative(tempx,1) + 1;
    
    P_act(act,9) = PrivacyEval(xycount_cumulative, xcount_cumulative, ycount_cumulative, (Win_Cumu + act - CumulativeStart + 1), m, n, err1, err2, err3);
    fprintf('\nCumulative I(Y;X): ');
    disp(P_act(act,9));
    
else
    
    P_act(act,9) = NaN;
    
end


if act >= SMAWindow
    
    P_act(act,10) = mean(P_act(act-SMAWindow+1:act,8));
    fprintf('\nSMA of I(Y;X) : ');
    disp(P_act(act,10));
    
else
    
    P_act(act,10) = NaN;
    fprintf('\nSMA Window greater than history');
    
end


fprintf('\n****************************************************************************\n');

Z_ijtHist(:,:,act) = value(Z_ijt);
xycountc_store(:,:,act) = value(xycountc);

% clear xycountc tempx tempy temphist;