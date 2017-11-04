function bin = binx(Input, bin_size, bin_quant)
   
    bin = 0;
    
%     if round(Input,5) > (bin_size * bin_quant)
%         Input = (bin_size * bin_quant);
%     end
    
    assert(single(round(Input,5))<= (bin_size * bin_quant), 'Maximum Value Exceeded!!');
    
    if single(round(Input,5)) <= 0
            bin = 1;
        
    else       
            bin = ceil(single(round(Input,5))/bin_size);
    end
        
end

% discretize() does something similar