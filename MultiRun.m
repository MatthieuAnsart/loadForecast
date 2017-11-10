
for answer = 8 : 9
    clearvars -except answer
    % load('C:\Users\ansartm\Desktop\New folder\BaseEMU\sample input data file\Family1Child.mat')
    
    projectpath = genpath(pwd);
    addpath(projectpath);
    % load('C:\Users\ansartm\Desktop\New folder\BaseEMU\sample input data file\Family1Child.mat')
    load('Y:\New folder\BaseEMU\sample input data file\Noise_generated_plus.mat')
    
    % rho = 10  gives min. ratio of privacy over regularisation term of 10
    
    %                      mu          cost      rho        gamma   battsize(kWh)
    
    SimSettings = [    %     0            1       9.1        10000       6.4;
        1            1       9.1        10000       6.4;
        2            1       9.1        10000       6.4;
        3            1       9.1        10000       6.4;
        4            1       9.1        10000       6.4;
        5            1       9.1        10000       6.4;
        6            1       9.1        10000       6.4;
        7            1       9.1        10000       6.4;
        8            1       9.1        10000       6.4;
        9            1       9.1        10000       6.4;
        10           1       9.1        10000       6.4;
        12           1       9.1        10000       6.4;
        14           1       9.1        10000       6.4;
        15           1       9.1        10000       6.4;
        16           1       9.1        10000       6.4;
        18           1       9.1        10000       6.4;
        20           1       9.1        10000       6.4;
        25           1       9.1        10000       6.4;
        30           1       9.1        10000       6.4;
        %                             35           1       9.1        10000       6.4;
        %                             40           1       9.1        10000       6.4;
        %                             45           1       9.1        10000       6.4;
        %                             50           1       9.1        10000       6.4;
        %                             55           1       9.1        10000       6.4;
        %                             60           1       9.1        10000       6.4;
        %                             65           1       9.1        10000       6.4;
        %                             70           1       9.1        10000       6.4;
        %                             75           1       9.1        10000       6.4;
        %                             80           1       9.1        10000       6.4;
        %                             85           1       9.1        10000       6.4;
        %                             90           1       9.1        10000       6.4;
        %                             95           1       9.1        10000       6.4;
        %                             100          1       9.1        10000       6.4;
        ]
    
    
    clc;
    
    Out = size(SimSettings(:,1));
    
    
    cd('Y:\New folder\BaseEMU\');
    folder = 'Big test 6.4kWh 3.3kW R9.1 30days Y5'; %T12 @ 30 min = 24 Step
    k = datestr(now,'dd-mmm-yyyy HH.MM.SS');
    str = int2str(answer);
    mkdir(strcat('Noise_',str, '__',folder, '__', k));
    cd(strcat('Noise_',str, '__',folder, '__', k));
    clear k;
    
    xlswrite('OutputCompilation.xlsx', {folder}, 'Data', '2');
    xlswrite('OutputCompilation.xlsx', {'mu' , 'Total Grid Energy (kWh)', 'Total Cost (SFr)' , ...
        'Time Average Step I(Y;X)' , 'Time Average Step I(Y;X)_obj' , 'Time Average I(Y;X)' , ...
        'Cumulative I(Y;X)', 'Max abs(I(Y;X)-I(Y;X)_obj)','Cost', 'BattCap', 'rho', 'gamma'}, 'Data', '3');
    
    % prompt = 'Which kind of forecast do you want to generate? \n 1 : perfect forecast \n 2 : gaussian noise with constant deviation penalty (no trend) \n 3 : gaussian noise with growing deviation penalty (no trend) \n 4 : gaussian noise with random deviation penalty (no trend) \n 5 : gaussian noise with constant deviation penalty (with trend) \n 6 : gaussian noise with growing deviation penalty (with trend) \n 7 : gaussian noise with random deviation penalty (with trend) \n \n';
    % answer = input(prompt);
    
%     if answer == 1
%         LoadInput = Noise_generated_1;
%     elseif answer == 2
%         LoadInput = Noise_generated_2;
%     elseif answer == 3
%         LoadInput = Noise_generated_3;
%     elseif answer == 4
%         LoadInput = Noise_generated_4;
%     elseif answer == 5
%         LoadInput = Noise_generated_5;
%     elseif answer == 6
%         LoadInput = Noise_generated_6;
%     elseif answer == 7
%         LoadInput = Noise_generated_7;
%     elseif answer == 8
    if answer == 8
        LoadInput = Noise_generated_8;
    elseif answer == 9
        LoadInput = Noise_generated_9;
        
    end
    
    
    for Run = 1:Out
        
        TestName = strcat('mu=',num2str(SimSettings(Run,1)),', cost=',num2str(SimSettings(Run,2)),', battsize=',num2str(SimSettings(Run,5)),', add 1'); %add special notes here
        mkdir(TestName);
        cd(TestName);
        mu = SimSettings(Run,1);
        CostOn = SimSettings(Run,2);
        BattCap = SimSettings(Run,5);
        
        rho = SimSettings(Run,3);
        gamma = SimSettings(Run,4);
        
        fprintf('**********************************\n\n');
        disp(TestName);
        fprintf('\n**********************************\n\n');
        
        MinCostRH;
        
        clearvars -except TestNameStore Run Out SimSettings folder LoadInput CostInput PVInput Time;
        %     clc;
        
    end
    
end
%
%     mailme('jx.chin@gmail.com',folder);
%     Hibernate
%
%   rmpath(genpath('C:\APPS\IBM\ILOG\CPLEX_Studio1263_x64\cplex\matlab/x64_win64'),...
%    'C:\polybox\Base EMU');

%     pause(600);
%     MultiRunQueue;
