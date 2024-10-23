This is .NET 8.0 (MC Architecture) Web API which is used for the server-side logic for Kotlin applicatoin, where you can save your workouts. 
The only purpose of the project is to manage the data and proccess requests/respones from/to the Kotlin app.


To test the API locally:
1. Donwload the Repo and open with Visual Studio
2. In the root folder, run the commands (replace <secret_key> with 32 chars string):
	- dotnet user-secrets init
	- dotnet user-secrets set "JwtSettings:SecretKey" "<secret_key>"
3. In launchesttings.Json -> profiles -> http, set applicationUrl to "http://192.168.0.0:1111", 
   replacing "192.168.0.0" with your localhost address and "1111" with a valid port number
4. Go to https://github.com/YuTaral/FitnessApp to setup the client side application

Both applications still in development.
