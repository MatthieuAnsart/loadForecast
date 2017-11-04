function mailme(Recipient,message2)

    setup_sendmail
    notsent = true;
    while notsent
        try
            message = sprintf('Simulation on %s finished', getenv('COMPUTERNAME'));

            sendmail(Recipient, message, message2)
            notsent = false;
        catch e
            fprintf('Sending failed.\n')
            pause(15)
        end
    end
    
end