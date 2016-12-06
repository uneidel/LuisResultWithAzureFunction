# LuisResultWithAzureFunction

Use Azure Functions to reproduce SpeechRecognitionServiceFactory.CreateDataClientWithIntent  without the Eventhandler.


use Powershell like 
measure-Command {Invoke-WebRequest -Uri http://function.azurewebsites.net/api/LuisTrigger/ -InFile (Get-Item .\whatstheweatherlike3.wav) -Method Post}

Note: currently luis intent is returned as pure string.
