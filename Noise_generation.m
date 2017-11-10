clear all
clc

load('C:\Users\ansartm\Desktop\New folder\BaseEMU\sample input data file\Family1Child.mat')

Number_kind_noise = 4;
Number_methods = 2 * Number_kind_noise + 1 ;
NumOut = 12;
LoadMax = 3.6;

Noise_generated_1 = zeros(length(LoadInput)-NumOut,NumOut); % perfect forecast

Noise_generated_2 = zeros(length(LoadInput)-NumOut,NumOut); % constant sigma
Noise_generated_3 = zeros(length(LoadInput)-NumOut,NumOut); % growing sigma
Noise_generated_4 = zeros(length(LoadInput)-NumOut,NumOut); % proportional sigma
Noise_generated_5 = zeros(length(LoadInput)-NumOut,NumOut); % day/night sigma

Noise_generated_6 = zeros(length(LoadInput)-NumOut,NumOut); % constant sigma
Noise_generated_7 = zeros(length(LoadInput)-NumOut,NumOut); % growing sigma
Noise_generated_8 = zeros(length(LoadInput)-NumOut,NumOut); % proportional sigma
Noise_generated_9 = zeros(length(LoadInput)-NumOut,NumOut); % day/night sigma
 
Load = LoadInput;

sig2 = 0.1 ; % constant sigma
sig3 = 0.1 ; % for growing sigma 
sig = std(LoadInput) ; % standard deviation of original value
m = mean (LoadInput) ; % average of original values
 
sig_day = 0.1; % 8h - 22 h
sig_night = 0.05; % 23h - 7h


for k = 1 : length(LoadInput)-NumOut
    
    h2 = random('norm', 0, sig2 , NumOut, 1);
    %     sig3 = gamcdf(abs(random('norm', 0.5, 0.5/3 , NumOut, 1)),5,0.5);
    %     h3 = random('norm', 0, sig3 , NumOut, 1);
    
    beta6 = zeros (1 : NumOut);
    beta7 = zeros (1 : NumOut);
    beta8 = zeros (1 : NumOut);
    beta9 = zeros (1 : NumOut);
    
    for t = 1 : NumOut
        
        %% perfect forecast
        Noise_generated_1(k,t)= Load(t+k-1); % perfect forecast
        
        %keep forecast realistic
        if Noise_generated_1(k,t) < 0
            Noise_generated_1(k,t) = 0;
        elseif Noise_generated_1(k,t) > LoadMax
            Noise_generated_1(k,t) = LoadMax;
        end
        
        %% no trend %%
        %% constant sigma
        Noise_generated_2(k,t)= Load(t+k-1)+ h2(t); % constant error
        
        %% growing sigma
        h3 = random('norm', 0, sig3*t, NumOut, 1);
        Noise_generated_3(k,t)= Load(t+k-1)+ h3(t); % growing error
        
        %         Noise_generated_4(k,t)= Load(t+k-1)+ h3(t); % gamma inverse error
        
        %% proportional sigma
        sig4 = (Load(t+k-1)*sig/m)^2 ;
        h4 = random('norm', 0, sig4 , NumOut, 1) ;
        Noise_generated_4(k,t)= Load(t+k-1)+ h4(t); % gamma dynamic
        
        %% day/night sigma
        moment = mod((t+k),24);
        if ( 7 < moment < 23)
            h5 = random('norm', 0, sig_day , NumOut , 1) ;
        else 
            h5 = random('norm', 0, sig_night , NumOut , 1) ;
        end
        Noise_generated_5(k,t)= Load(t+k-1)+ h5(t);
        
        %% keep forecast realistic
        if Noise_generated_2(k,t) < 0
            Noise_generated_2(k,t) = 0;
        elseif Noise_generated_2(k,t) > LoadMax
            Noise_generated_2(k,t) = LoadMax;
        end
        
        if Noise_generated_3(k,t) < 0
            Noise_generated_3(k,t) = 0;
        elseif Noise_generated_3(k,t) > LoadMax
            Noise_generated_3(k,t) = LoadMax;
        end
        
        if Noise_generated_4(k,t) < 0
            Noise_generated_4(k,t) = 0;
        elseif Noise_generated_4(k,t) > LoadMax
            Noise_generated_4(k,t) = LoadMax;
        end
        
        if Noise_generated_5(k,t) < 0
            Noise_generated_5(k,t) = 0;
        elseif Noise_generated_5(k,t) > LoadMax
            Noise_generated_5(k,t) = LoadMax;
        end
        
        %% with trend
        if t==1 
            Noise_generated_6(k,t)= Load(t+k-1)+ h2(t); 
            Noise_generated_7(k,t)= Load(t+k-1)+ h3(t); 
            Noise_generated_8(k,t)= Load(t+k-1)+ h4(t);
            Noise_generated_9(k,t)= Load(t+k-1)+ h5(t);
        else
            beta6(t) = Load(t-2+k) - Noise_generated_6(k,t-1)+ h2(t);
            beta7(t) = Load(t-2+k) - Noise_generated_7(k,t-1)+ h3(t);
            beta8(t) = Load(t-2+k) - Noise_generated_8(k,t-1)+ h4(t);
            beta9(t) = Load(t-2+k) - Noise_generated_5(k,t-1)+ h5(t);
             
            Noise_generated_6(k,t)= Load(t+k-1)+ beta6(t); 
            Noise_generated_7(k,t)= Load(t+k-1)+ beta7(t);
            Noise_generated_8(k,t)= Load(t+k-1)+ beta8(t);
            Noise_generated_9(k,t)= Load(t+k-1)+ beta9(t);
        end
        
        %% keep forecast realistic
        if Noise_generated_6(k,t) < 0
            Noise_generated_6(k,t) = 0;
        elseif Noise_generated_6(k,t) > LoadMax
            Noise_generated_6(k,t) = LoadMax;
        end
        
        if Noise_generated_7(k,t) < 0
            Noise_generated_7(k,t) = 0;
        elseif Noise_generated_7(k,t) > LoadMax
            Noise_generated_7(k,t) = LoadMax;
        end

        if Noise_generated_8(k,t) < 0
            Noise_generated_8(k,t) = 0;
        elseif Noise_generated_8(k,t) > LoadMax
            Noise_generated_8(k,t) = LoadMax;
        end
        
        if Noise_generated_9(k,t) < 0
            Noise_generated_9(k,t) = 0;
        elseif Noise_generated_9(k,t) > LoadMax
            Noise_generated_9(k,t) = LoadMax;
        end
        
    end
end

% save('Noise_generated_1','CostInput','Time','PVInput','Noise_generated_2','Noise_generated_3','Noise_generated_4','Noise_generated_5','Noise_generated_6','Noise_generated_7','Noise_generated_8','Noise_generated_9');
save('Noise_generated_plus','Noise_generated_1','CostInput','Time','PVInput','Noise_generated_8','Noise_generated_9');

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