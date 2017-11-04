

clear all;

% rho = 10  gives min. ratio of privacy over regularisation term of 10

%               mu          cost    rho         gamma       battsize(kWh)
        
SimSettings = [ 0            1       9.1         10000       12.8;
                1            1       9.1         10000       12.8;
                2            1       9.1         10000       12.8;
                3            1       9.1         10000       12.8;
                4            1       9.1         10000       12.8;
                5            1       9.1         10000       12.8;
                6            1       9.1         10000       12.8;
                7            1       9.1         10000       12.8;
                8            1       9.1         10000       12.8;
                9            1       9.1         10000       12.8;
                10           1       9.1         10000       12.8;
                12           1       9.1         10000       12.8;
                14           1       9.1         10000       12.8;
                15           1       9.1         10000       12.8;
                16           1       9.1         10000       12.8;
                18           1       9.1         10000       12.8;
                20           1       9.1         10000       12.8;
                25           1       9.1         10000       12.8;
                30           1       9.1         10000       12.8;
                35           1       9.1         10000       12.8;
                40           1       9.1         10000       12.8;
                45           1       9.1         10000       12.8;
                50           1       9.1         10000       12.8;
                55           1       9.1         10000       12.8;
                60           1       9.1         10000       12.8;
                65           1       9.1         10000       12.8;
                70           1       9.1         10000       12.8;           
                75           1       9.1         10000       12.8;
                80           1       9.1         10000       12.8;                 
                85           1       9.1         10000       12.8;
                90           1       9.1         10000       12.8;
                95           1       9.1         10000       12.8;
                100          1       9.1         10000       12.8;
              ]

      
clc;

Out = size(SimSettings(:,1));

cd('D:\BaseEMU\IEEE Trans Data\');
folder = 'Family1Child T12 Base2 12.8kWh 3.3kW R9.1 30days Y5';
k = datestr(now,'dd-mmm-yyyy HH.MM.SS');
mkdir(strcat(folder, '__', k));
cd(strcat(folder, '__', k));
clear k;

xlswrite('OutputCompilation.xlsx', {folder}, 'Data', '2');
xlswrite('OutputCompilation.xlsx', {'mu' , 'Total Grid Energy (kWh)', 'Totat Cost (SFr)' , ...
            'Time Average Step I(Y;X)' , 'Time Average Step I(Y;X)_obj' , 'Time Average I(Y;X)' , ...
                'Cumulative I(Y;X)', 'Max abs(I(Y;X)-I(Y;X)_obj)','Cost', 'BattCap', 'rho', 'gamma'}, 'Data', '3');

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
    
    clearvars -except TestNameStore Run Out SimSettings folder;
    clc;
    
end

    mailme('jx.chin@gmail.com',folder);
%     Hibernate
    
%     pause(600);
%     MultiRunQueue2; 

