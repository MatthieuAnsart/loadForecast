function p_loss = PrivacyFunc_corrected(PAlgorithm, xcount, ycountrec, xycountrec, xycountc, N_window, x_bin_quant, y_bin_quant, err1, err2, err3)

    p_loss = 0;

    if PAlgorithm == 1

        for i = 1 : x_bin_quant

            for j = 1 : y_bin_quant

               p_loss = p_loss + ((xycountrec(i,j)+ xycountc(i,j))/(N_window + x_bin_quant*y_bin_quant*err1)) * ...
                                        (log2((xycountrec(i,j)/(N_window + x_bin_quant*y_bin_quant*err1)) * ((N_window + x_bin_quant*err2)/xcount(i,1)) * ...
                                            ((N_window + y_bin_quant*err3)/ycountrec(j,1))) + (1/log(2))*(xycountc(i,j)/xycountrec(i,j)) - (1/log(2))*(sum(xycountc(:,j))/ycountrec(j,1))); %
            end

        end

    elseif PAlgorithm == 2 %both CPlex and Gurobi cannot solve the non-linear problem
        
        for i = 1:x_bin_quant
            for j = 1:y_bin_quant

                p_loss = p_loss + (xycountrec(i,j)+ xycountc(i,j))/(N_window + x_bin_quant*y_bin_quant*err1) * ...
                                log2( (xycountrec(i,j)+ xycountc(i,j))/(N_window + x_bin_quant*y_bin_quant*err1) * (N_window + x_bin_quant*err2) * ...
                                    (N_window + y_bin_quant*err3) / (xcount(i,1) * (ycountrec(j,1))+sum(xycountc(:,j))));

            end
        end
    
    else
        
    end

end


%log2 base 2 for output in bits. log2 base e, then output in nats, base 10,
%bans