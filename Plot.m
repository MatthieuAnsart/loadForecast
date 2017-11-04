
StartPlot = StartTime;
EndPlot = round(NumOut);

figure();

subplot(3,1,1)
plot(Time(StartPlot:EndPlot),P_out(StartPlot:EndPlot,1),...
    Time(StartPlot:EndPlot),P_out(StartPlot:EndPlot,1)/TimeStep)
grid on
legend('E_{grid}','P_{grid}')


subplot(3,1,2)
plot(Time(StartPlot:EndPlot),P_pv(StartPlot:EndPlot,1),...
    Time(StartPlot:EndPlot),E_load(StartPlot:EndPlot,1)/TimeStep,...
        Time(StartPlot:EndPlot),P_out(StartPlot:EndPlot,4))
grid on
legend('P_{pv}','P_{load}','P_{batt}')

subplot(3,1,3)
plot(Time(StartPlot:EndPlot),P_out(StartPlot:EndPlot,4),...
    Time(StartPlot:EndPlot),P_out(StartPlot:EndPlot,5))
grid on
legend('P_{batt}','E_{batt}')