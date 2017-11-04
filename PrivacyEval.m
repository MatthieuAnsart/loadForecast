function I_xy = PrivacyEval(xycountrec, xcountrec, ycountrec, N_window, m, n, err1, err2, err3)

    I_xy = 0;

    for i = 1:m
        for j = 1:n

            I_xy = I_xy + (xycountrec(i,j)/(N_window + m*n*err1)) * ...
                        log2( xycountrec(i,j)/(N_window + m*n*err1) * (N_window + m*err2) * ...
                            (N_window + n*err3) / xcountrec(i,1) / ycountrec(j,1) );

        end
    end

end


%log2 base 2 for output in bits. log2 base e, then output in nats, base 10,
%bans