function GLPlot(StartPlot, EndPlot, P_act, PVInput, LoadInput, TimeStep, ybinsize, Time, Interval)

   %figure('units','normalized','outerposition',[0 0 1 1]);
    
    plot(Time(StartPlot:EndPlot),LoadInput(StartPlot:EndPlot,1)/TimeStep,...
        Time(StartPlot:EndPlot),P_act(StartPlot:EndPlot,2)/TimeStep);
    ax = gca;
    ax.YAxisLocation = 'right';
    ax.YTick = ax.YLim(1):ybinsize*Interval/60:ax.YLim(2);
    Tick = [0:1:floor(ax.YLim(2)/ybinsize*Interval/60)];
    ax.YTickLabel = Tick;
    ax3 = ylabel('Bin No.', 'FontSize',14);
    %ax3.Position(1) = ax3.Position(1) + 0.0001;
    axes;
    hold on;
    
    plot(Time(StartPlot:EndPlot),LoadInput(StartPlot:EndPlot,1)/TimeStep,...
        Time(StartPlot:EndPlot),P_act(StartPlot:EndPlot,2)/TimeStep);
    ax4 = legend('P_{load}','P_{grid}');
    set(ax4,'FontSize',12);
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
    ax.XTickLabel = [];
    ax.XLim = [min(ax.XTick), max(ax.XTick)];
    ylabel('Power (kW)','FontSize',14);
    %title('Consumer Load and Grid Energy')
    ax2 = gcf;
    ax2.Units = 'normalized';
    ax2.OuterPosition = [0 0 1 1];
        
    clear ax ax2 ax3 Tick;
    hold off;
    
end    
    
 