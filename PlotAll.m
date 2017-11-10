%%   Plot; % calls plot.m

PeriodStart = 1;
PeriodEnd = Period*24*60/Interval;
WindowStart = 200;
WindowEnd = WindowStart + Period*24*60/Interval - 1;

fprintf('\nTotal Grid Energy(kWh): '); 
disp(sum(P_act(PeriodStart:PeriodEnd,2))); %%
fprintf('Total Cost (Rp): ');
CostCalc(PeriodStart,PeriodEnd,P_act(:,2),CostRec(:)); %%

figure('visible','off');
PlotRH(PeriodStart, PeriodEnd, P_act, PVInput(StartTime:StartTime+(Period*24*60/Interval)-1), ...
    LoadInput(StartTime:StartTime+(Period*24*60/Interval)-1,1), TimeStep, ybinsize, ...
        Time(StartTime:StartTime+(Period*24*60/Interval)-1), Interval); %%
print('Combined Plots','-djpeg100','-r200');
saveas(gcf,'Combined Plots','fig');
    
figure('visible','off');
XYPlot(1,xycountrec,m,n);
print('XYPlot','-djpeg100','-r200');
saveas(gcf,'XYPlot','fig');

figure('visible','off');
XYPlot(2,xycountrec,m,n,err1);
print('XYPlot3D','-djpeg100','-r200');
saveas(gcf,'XYPlot3D','fig');

figure('visible','off');
GLPlot(PeriodStart, PeriodEnd, P_act, PVInput(StartTime:StartTime+(Period*24*60/Interval)-1), ...
    LoadInput(StartTime:StartTime+(Period*24*60/Interval)-1,1), TimeStep, ybinsize, ...
        Time(StartTime:StartTime+(Period*24*60/Interval)-1), Interval); %%
print('Load vs Grid','-djpeg100','-r200');
saveas(gcf,'Load vs Grid','fig');
    
figure('visible','off');
plot(Time(StartTime+PeriodStart-1:StartTime+PeriodEnd-1),P_act(PeriodStart:PeriodEnd,6),...
        Time(StartTime+PeriodStart-1:StartTime+PeriodEnd-1),P_act(PeriodStart:PeriodEnd,7));
ylabel('I(Y;X)');
grid on;
ax3 = legend('I(Y;X)','I(Y;X)_{Obj}');
set(ax3, 'FontSize',14);
title('Step I(Y;X)','FontSize',20);
    ax2 = gcf;
    ax2.Units = 'normalized';
    ax2.OuterPosition = [0 0 1 1];
    clear ax2 ax3
print('IXYStep','-djpeg100','-r200');
saveas(gcf,'IXYStep','fig');

figure('visible','off');
subplot(3,1,1);
plot(Time(StartTime+PeriodStart-1: StartTime+PeriodEnd-1),P_act(PeriodStart : PeriodEnd,8));
title('Rolling Window I(Y;X)');
ylabel('I(Y;X)');
    ax1 = gca;
    ax1.YLim = [0 1];
grid on;
subplot(3,1,2);
plot(Time(StartTime+PeriodStart-1: StartTime+PeriodEnd-1),P_act(PeriodStart : PeriodEnd,9));
title('Cumulative I(Y;X)');
ylabel('I(Y;X)');
    ax1 = gca;
    ax1.YLim = [0 1];
grid on;
subplot(3,1,3);
plot(Time(StartTime+PeriodStart-1: StartTime+PeriodEnd-1),P_act(PeriodStart : PeriodEnd,10));
title('SMA I(Y;X)');
ylabel('I(Y;X)');
grid on;
    ax2 = gcf;
    ax2.Units = 'normalized';
    ax2.OuterPosition = [0 0 1 1];
    ax1 = gca;
    ax1.YLim = [0 1];
    clear ax1 ax2
print('IXY','-djpeg100','-r200');
saveas(gcf,'IXY','fig');

% %% Window %%%
% 
% fprintf('\nGrid Energy in Window (kWh): '); 
% disp(sum(P_act(WindowStart:WindowEnd,2))); %%
% fprintf('Window Cost (Rp): ');
% CostCalc(WindowStart,WindowEnd,P_act(:,2),CostRec(:)); %%
% 
% figure('visible','off');
% PlotRH(WindowStart, WindowEnd, P_act, PVInput(StartTime:StartTime+(Period*24*60/Interval)-1), ...
%     LoadInput(StartTime:StartTime+(Period*24*60/Interval)-1,1), TimeStep, ybinsize, ...
%         Time(StartTime:StartTime+(Period*24*60/Interval)-1), Interval); %%
% print('Combined Plots Window','-djpeg100','-r200');
% saveas(gcf,'Combined Plots Window','fig');
%     
% figure('visible','off');
% GLPlot(WindowStart, WindowEnd, P_act, PVInput(StartTime:StartTime+(Period*24*60/Interval)-1), ...
%    LoadInput(StartTime:StartTime+(Period*24*60/Interval)-1,1), TimeStep, ybinsize, ...
%         Time(StartTime:StartTime+(Period*24*60/Interval)-1), Interval); %%
% print('Load vs Grid Window','-djpeg100','-r200');
% saveas(gcf,'Load vs Grid Window','fig');
%     
%     
% figure('visible','off');
% plot(Time(StartTime+WindowStart-1: StartTime+WindowEnd-1),real(P_act(WindowStart: WindowEnd,6)),...
%         Time(StartTime+WindowStart-1: StartTime+WindowEnd-1),real(P_act(WindowStart: WindowEnd,7)));
% ylabel('I(Y;X)');
% grid on;
% ax3 = legend('I(Y;X)','I(Y;X)_{Obj}');
% set(ax3, 'FontSize',14);
% title('Step I(Y;X)', 'FontSize', 20);
%     ax2 = gcf;
%     ax2.Units = 'normalized';
%     ax2.OuterPosition = [0 0 1 1];
%     clear ax2 ax3
% print('IXY Step Window','-djpeg100','-r200');
% saveas(gcf,'IXY Step Window','fig');
% 
% figure('visible','off');
% subplot(3,1,1);
% plot(Time(StartTime+WindowStart-1: StartTime+WindowEnd-1),P_act(WindowStart : WindowEnd,8));
% title('Rolling Window I(Y;X)');
% ylabel('I(Y;X)');
%     ax1 = gca;
%     ax1.YLim = [0 1];
% grid on;
% subplot(3,1,2);
% plot(Time(StartTime+WindowStart-1: StartTime+WindowEnd-1),P_act(WindowStart : WindowEnd,9));
% title('Cumulative I(Y;X)');
% ylabel('I(Y;X)');
%     ax1 = gca;
%     ax1.YLim = [0 1];
% grid on;
% subplot(3,1,3);
% plot(Time(StartTime+WindowStart-1: StartTime+WindowEnd-1),P_act(WindowStart : WindowEnd,10));
% title('SMA I(Y;X)');
% ylabel('I(Y;X)');
%     ax1 = gca;
%     ax1.YLim = [0 1];
% grid on;
%     ax2 = gcf;
%     ax2.Units = 'normalized';
%     ax2.OuterPosition = [0 0 1 1];
%     clear ax1 ax2
% print('IXY Window','-djpeg100','-r200');
% saveas(gcf,'IXY Window','fig');

%%

% figure('visible','off');
% plot([PeriodStart, PeriodEnd],Noise_generated_1(PeriodStart : PeriodEnd,1))
% hold on
% grid on
% plot([PeriodStart, PeriodEnd], LoadInput(PeriodStart : PeriodEnd,1))
% legend('-b','X','-r','Forecast')

% pause(5);

close all;

% pause(2);