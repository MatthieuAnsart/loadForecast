setpref('Internet', 'SMTP_Server',   'smtp.gmail.com');
setpref('Internet', 'E_mail',        '_@gmail.com');
setpref('Internet', 'SMTP_Username', '_@gmail.com');
setpref('Internet', 'SMTP_Password', '_');

props = java.lang.System.getProperties;
props.setProperty('mail.smtp.auth',                'true');
props.setProperty('mail.smtp.socketFactory.class', 'javax.net.ssl.SSLSocketFactory');
props.setProperty('mail.smtp.socketFactory.port',  '465');

props.setProperty('mail.smtp.startssl.enable','true');