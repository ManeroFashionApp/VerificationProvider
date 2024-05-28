# VerificationProvider
GeneerateCode triggas av service bus. 
skicka in en json som ser ut såhär i service busen.
{
  "email":"exempel@mail.com"
}

Validering av koden är ett post anrop.
tar emot:
{
  "email":"exempel@mail.com"
  "code":"123456"
}
