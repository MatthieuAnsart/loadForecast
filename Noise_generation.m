clear all
clc

load('C:\Users\ansartm\Desktop\New folder\BaseEMU\sample input data file\Family1Child.mat')

Number_methods = 7;
NumOut = 12;
Noise_generated_1 = zeros(length(LoadInput),NumOut);
Noise_generated_2 = zeros(length(LoadInput),NumOut);
Noise_generated_3 = zeros(length(LoadInput),NumOut);
Noise_generated_4 = zeros(length(LoadInput),NumOut);
Noise_generated_5 = zeros(length(LoadInput),NumOut);
Noise_generated_6 = zeros(length(LoadInput),NumOut);
Noise_generated_7 = zeros(length(LoadInput),NumOut);

Load = LoadInput;
sig1 = 0.1 ;


for k = 1 : length(LoadInput)-NumOut
    
    h1 = random('norm', 0, sig1 , NumOut, 1);
    sig2 = gamcdf(abs(random('norm', 0.5, 0.5/3 , NumOut, 1)),5,0.5);
    h3 = random('norm', 0, sig2 , NumOut, 1);
    beta5 = zeros (1 : NumOut);
    beta6 = zeros (1 : NumOut);
    beta7 = zeros (1 : NumOut);
    
    
    for t = 1 : NumOut
        Noise_generated_1(k,t)= Load(t+k); % perfect forecast
        
        %no trend
        Noise_generated_2(k,t)= Load(t+k)+ h1(t); % constant error
        h2 = random('norm', 0, sig1*t, NumOut, 1);
        Noise_generated_3(k,t)= Load(t+k)+ h2(t); % growing error
        Noise_generated_4(k,t)= Load(t+k)+ h3(t); % gamma inverse error
        
        
        %with trend
        if t==1
            Noise_generated_5(k,t)= Load(t+k)+ h1(t); % constant error
            Noise_generated_6(k,t)= Load(t+k)+ h2(t); % growing error
            Noise_generated_7(k,t)= Load(t+k)+ h3(t); % gamma inverse error
        else
            beta5(t) = Load(t-1+k) - Noise_generated_5(k,t-1)+ h1(t);
            beta6(t) = Load(t-1+k) - Noise_generated_6(k,t-1)+ h2(t);
            beta7(t) = Load(t-1+k) - Noise_generated_7(k,t-1)+ h3(t);
            
            Noise_generated_5(k,t)= Load(t+k)+ beta5(t)+ h1(t); % constant error
            Noise_generated_6(k,t)= Load(t+k)+ beta6(t)+ h2(t); % growing error
            Noise_generated_7(k,t)= Load(t+k)+ beta7(t)+ h3(t); % gamma inverse error
        end
    end
end

save('Noise_generated','Noise_generated_1','CostInput','Time','PVInput','Noise_generated_2','Noise_generated_3','Noise_generated_4','Noise_generated_5','Noise_generated_6','Noise_generated_7');

%%%%%%%%%%%%%%%%% bruit avec esp constant %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% version 1
%
% Load = LoadInput;
% LoadInput = zeros(length(LoadInput),1);
% sig = 0.1 ;
% eps = random('norm', 0, sig , length(Load), 1);
%
% LoadInput(1)=Load(1);
%
% for t = 1:length(LoadInput)-1
%     LoadInput(t+1) = Load(t+1) - (Load(t)-LoadInput (t))-eps(t+1);
% end


% version 2

% Load = LoadInput;
% LoadInput = zeros(length(LoadInput),1);
% sig = LoadMax/3; % 3 sig de chaque cote de la moyenne represente 99.73% de proba dans le cas d'une loi normale
% eps = random('norm', 0, sig , length(Load), 1);
%
% LoadInput(1)=Load(1);
%
% for t = 1:length(LoadInput)-1
%     LoadInput(t+1) = Load(t+1) - (Load(t)-LoadInput (t))-eps(t+1);
% end

%%%%%%%%%%%%%%%% bruit avec eps random %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Load = LoadInput;
% LoadInput = zeros(length(LoadInput),1);
% sig = gamcdf(abs(random('norm', 0.5, 0.5/3 , length(Load), 1)),5,0.5);
% eps = random('norm', 0, sig , length(Load), 1);
%
% LoadInput(1)=Load(1);
%
% for t = 1:length(LoadInput)-1
%     LoadInput(t+1) = Load(t+1) - (Load(t)-LoadInput (t))-eps(t+1);
% end

% for act = 1 :  round(Period*24*60/Interval)
%
%     fprintf('\nTime from Start: ');
%     disp(act);
%     [P_pv, E_load, cost] = EnvironmentSense(1, P_pv, E_load, cost, PVInput, LoadInput, CostInput, StartTime, act, NumOut, Interval);
%     ControllerBase2; %Default
%     CPlexOut(act) = sol;
%
%     if act == 1
%         options = sdpsettings('solver','CPlex','verbose',1,'showprogress',1,'cplex.solutiontarget',3, 'cplex.mip.display', 'on', 'saveyalmipmodel', 1,...
%             'savesolveroutput',1);
%     end
%
% end



% % %%%%%%%%%%%%%%% bruit avec eps croissant %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Load = LoadInput;
% LoadInput = zeros(length(LoadInput),1);
% LoadInput(1)=Load(1);
%
% sig = 0.2;
%
% % for t = 1:xbinsize
% %     eps = random('norm', 0, sig * t ,1, 1);
% %     LoadInput(t+1) = Load(t+1) - (Load(t)-LoadInput (t))-eps;
% % end
%
% for act = 1 :  round(Period*24*60/Interval)
%
%     fprintf('\nTime from Start: ');
%     disp(act);
%     for t = 1  : m
%         eps = random('norm', 0, sig * t ,1, 1);
%         LoadInput(t+act) = Load(t+act) - (Load(t+act-1)-LoadInput (t+act-1))-eps;
%     end
%     [P_pv, E_load, cost] = EnvironmentSense(1, P_pv, E_load, cost, PVInput, LoadInput, CostInput, StartTime, act, NumOut, Interval);
%     ControllerBase2; %Default
%     CPlexOut(act) = sol;
%
%     if act == 1
%         options = sdpsettings('solver','CPlex','verbose',1,'showprogress',1,'cplex.solutiontarget',3, 'cplex.mip.display', 'on', 'saveyalmipmodel', 1,...
%             'savesolveroutput',1);
%     end
%
% end