namespace Infrastructure.Messaging;

public static class EmailTemplates
{
    private const string RootResetLink = "https://achemeupet.com.br/redefinir-senha";

    public static string GetPasswordResetEmailTemplate(string userName, string userEmail, string resetToken)
    {
        string resetLink = $"{RootResetLink}?token={resetToken}&email={userEmail}";

        return $$"""
                 <!doctype html>
                 <html lang="pt-BR">
                 <head>
                 <meta charset="UTF-8" />
                 <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                 <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
                 <title>Redefinição de Senha - AcheMeuPet</title>
                 <link rel="preconnect" href="https://fonts.googleapis.com" />
                 <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
                 <link href="https://fonts.googleapis.com/css2?family=Onest:wght@100..900&display=swap" rel="stylesheet" />
                 <style>
                 body {
                 font-family: "Onest", sans-serif;
                 line-height: 1.6;
                 color: #333;
                 max-width: 600px;
                 margin: 0 auto;
                 padding: 20px;
                  }
                 .header {
                 background-color: #f4f4f4;
                 padding: 20px;
                 text-align: center;
                  }
                 .content {
                 padding: 20px;
                  }
                 .button {
                 display: inline-block;
                 padding: 10px 20px;
                 background-color: #635bff;
                 color: #ffffff;
                 text-decoration: none;
                 border-radius: 5px;
                  }
                 .buttonText {
                  color: #fff;
                  fontWeight: 700 !important;
                 }
                 .footer {
                 margin-top: 20px;
                 text-align: center;
                 font-size: 0.8em;
                 color: #777;
                  }
                 </style>
                 </head>
                 <body>
                 <div class="header">
                 <h1>Redefinição de Senha</h1>
                 </div>
                 <div class="content">
                 <p>Olá {{userName}},</p>
                 <p>
                  Recebemos uma solicitação para redefinir a senha da sua conta em nossa plataforma.
                 </p>
                 <p>Para redefinir sua senha, clique no botão abaixo:</p>
                 <p style="text-align: center">
                 <a href="{{resetLink}}" class="button"><span class="buttonText">Redefinir Senha</span></a>
                 </p>
                 <p>Ou copie e cole o seguinte link em seu navegador:</p>
                 <p>{{resetLink}}</p>
                 <p>Este link é válido por 3 horas. Após esse período, você precisará solicitar uma nova redefinição de senha.</p>
                 <p>
                  Por razões de segurança, o link acima contém um token único associado à sua conta. Não compartilhe este e-mail
                  ou link com ninguém.
                 </p>
                 <p>
                  Se você tiver problemas para redefinir sua senha ou não solicitou esta alteração, entre em contato com nossa
                  equipe de suporte em suporte@achemeupet.com.br.
                 </p>
                 <p>Atenciosamente,<br /><strong>AcheMeuPet</strong></p>
                 </div>
                 <div class="footer">
                 <p>Este é um e-mail automático. Por favor, não responda a esta mensagem.</p>
                 </div>
                 </body>
                 </html>
                 """;
    }
}