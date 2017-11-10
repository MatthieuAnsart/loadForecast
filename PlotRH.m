function PlotRH(StartPlot, EndPlot, P_act, PVInput, LoadInput, TimeStep, ybinsize, Time, Interval)

subplot(3,1,1);
plot(Time(StartPlot:EndPlot),LoadInput(StartPlot:EndPlot,1)/TimeStep,...
    Time(StartPlot:EndPlot),P_act(StartPlot:EndPlot,2)/TimeStep);
legend('P_{load}','P_{grid}');
ylabel('Power (kW)');
ax = gca;
ax.GridAlpha = 0.3;
%ax.GridLineStyle = '--';
ax.YGrid = 'on';
ax.XMinorGrid = 'on';
ax.MinorGridLineStyle = ':';
ax.MinorGridAlpha = 0.3;
ax.YAxisLocation = 'left';
ax.YTickMode = 'auto';
ax.YTick = ax.YLim(1):ybinsize*Interval/60:ax.YLim(2);
ax.XLim = [min(ax.XTick), max(ax.XTick)];

subplot(3,1,2)
plot(Time(StartPlot:EndPlot),PVInput(StartPlot:EndPlot,1),...
    Time(StartPlot:EndPlot),LoadInput(StartPlot:EndPlot,1)/TimeStep,...
    Time(StartPlot:EndPlot),P_act(StartPlot:EndPlot,4));
grid on;
ax = gca;
ax.XLim = [min(ax.XTick), max(ax.XTick)];
legend('P_{PV}','P_{load}','P_{batt}');
ylabel('Power (kW)');

subplot(3,1,3)
plot(Time(StartPlot:EndPlot),P_act(StartPlot:EndPlot,4),...
    Time(StartPlot:EndPlot),P_act(StartPlot:EndPlot,5));
grid on;
legend('P_{batt}','E_{batt}');
ylabel('Power (kW) / Energy (kWh)');
ax = gca;
ax.XLim = [min(ax.XTick), max(ax.XTick)];
ax2 = gcf;
ax2.Units = 'normalized';
ax2.OuterPosition = [0 0 1 1];

clear ax ax2
end