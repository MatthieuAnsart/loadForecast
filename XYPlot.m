function XYPlot(type, XYData, x_bin_quant, y_bin_quant, Zmin)

    if type == 1
        
            imagesc(XYData);
            grid on;
            colorbar;
            xlabel([' Grid Load ' ; '(y bin no.)'],'FontSize',24, 'HorizontalAlignment','center');
            ylabel(['Consumer Load'; ' (x bin no.) '],'FontSize',24, 'HorizontalAlignment','center');
            ax = gca;
            ax.YDir = 'normal';
            ax.TickLength = [0 0];
            ax.GridAlpha = 1;
            %colormap('jet');
            ax.XTickMode = 'manual';
            ax.XTickLabelMode = 'manual';
            ax.XTick = 1.5:1:y_bin_quant+0.5;
            Tick = [1:1:y_bin_quant];
            ax.XTickLabel = Tick;
            ax.YTickMode = 'manual';
            ax.YTickLabelMode = 'manual';
            ax.YTick = 1.5:1:x_bin_quant+0.5;
            Tick = [1:1:x_bin_quant];
            ax.YTickLabel = Tick;
            %title('X and Y Counts')
            clear Tick;
            axis square;
            
    elseif type == 2
            
            b = bar3(XYData,0.8);
            %title('X and Y Counts')
            xlabel([' Grid Load ' ; '(y bin no.)'],'FontSize',24, 'HorizontalAlignment','center');
            ylabel(['Consumer Load'; ' (x bin no.) '],'FontSize',24, 'HorizontalAlignment','center');
            zlabel('Frequency','FontSize',24);
            ax = gca;
            %ax.YDir = 'normal';
            ax.TickLength = [0 0];
            ax.GridAlpha = 1;
            %colormap('jet');
            ax.XTickMode = 'manual';
            ax.XTickLabelMode = 'manual';
            ax.XTick = 1.5:1:y_bin_quant+0.5;
            Tick = [1:1:y_bin_quant];
            ax.XTickLabel = Tick;
            ax.YTickMode = 'manual';
            ax.YTickLabelMode = 'manual';
            ax.YTick = 1.5:1:x_bin_quant+0.5;
            Tick = [1:1:x_bin_quant];
            ax.YTickLabel = Tick;
            axis equal;
            axis square;
            ax.ZLim(1) = Zmin;
            
            for k = 1:length(b)
                zdata = b(k).ZData;
                b(k).CData = zdata;
                b(k).FaceColor = 'interp';
            end
    else
        
    end
    
        
    ax2 = gcf;
    ax2.Units = 'normalized';
    ax2.OuterPosition = [0 0 1 1];
    
    clear ax;
    
end