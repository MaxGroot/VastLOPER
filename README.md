# VastLOPER
C# project for school


Classes en hun functies:

- Kaart.cs :            Rijksdriehoekfuncties
- KaartDetView.cs :     De view van de kaart zelf, dus niet de knoppen eromheen.
- KaartInterface.cs:    De interface rondom de kaart, roept ook de KaartView aan en geeft deze weer.
- KaartMainMenu.cs:     Het hoofdmenu wat je te zien krijgt als je de app opstart.
- Loadinterface.cs:     Het menu waarin de gebruiker een track kan kiezen om te laden
- Trackmanager.cs:      Verzameling functies om tracks te analyseren, coderen en te decoderen. Wordt in andere klassen gebruikt.
- analyzeinterface.cs:  De interface waarin een gegeven track een analyse interface geeft, waarin de gebruiker statistieken krijgt te zien
- saveload.cs:          Verzameling functies om tracks op te slaan en te laden.
- sqlite.cs:            Externe library om sqlite te kunnen gebruiken. Afblijven.
- Trackanalyzer.cs:     Dubbel van trackmanager. Even kijken welke vd twee gebruikt moet worden, dit is weird.
