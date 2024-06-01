# VerificationProvider
GenerateAndSendVerificationCode triggas av service bus. 
Skicka in en json som ser ut såhär i service busen.
{
  "email":"exempel@mail.com"
}

ValidateCode är ett post anrop.
Tar emot:
{
  "email":"exempel@mail.com"
  "code":"123456"
}

VerificationCleaner 
kollar om det ligger requests i databasen där giltighetstiden har gått ut och rensar bort dem varje minut. 
