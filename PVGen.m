P_pv=zeros(288,1);

%alternatively, use repmat

i=1;
j=1;
k=1;
while i <=288,
    
    if j <= 4
        P_pv(i) = pv_init(k);
        j = j+1;
    else 
       j = 1;
       k = k+1;
       i = i-1
    end
        i = i + 1;
        
end