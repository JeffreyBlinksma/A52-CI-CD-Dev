# Area52-Entertainment

ASP.NET Core webapplicatie voor het reserverings- en annuleersysteem van het vakantiepark Area52.

## Doel van de applicatie
Deze applicatie ondersteunt het beheren van activiteiten (shows en workshops) en stelt gebruikers in staat zich in te schrijven of een reservering te annuleren.  
De applicatie is ontwikkeld als onderdeel van een groepsproject, waarbij software, hosting, security en deployment op elkaar zijn afgestemd.

## Architectuur
De applicatie is opgebouwd volgens een gelaagde architectuur:

- **Pages** – Presentatielaag (Razor Pages)
- **Services** – Applicatielaag met businesslogica
- **Domain** – Domeinmodellen en regels
- **Data / Sql** – Datatoegang en database-interactie

De UI communiceert uitsluitend met de Services-laag. Hierdoor is de domeinlogica afgeschermd en beter onderhoudbaar.

## Database
De applicatie maakt gebruik van een relationele SQL Server database voor persistente opslag.  
De databaseverbinding wordt geconfigureerd via `appsettings.json`.

In de uiteindelijke hostingomgeving wordt gewerkt met een aparte SQL-user, afgestemd met de hosting- en securitystudent.

## Samenwerking
Deze repository wordt gebruikt voor samenwerking binnen het project:
- Softwareontwikkeling
- Testen door DevOps
- Afstemming met hosting en security

Feedback en wijzigingen worden iteratief verwerkt.

## Status
Huidige status: **werkende prototype / eerste realisatie**  
Verdere testen en verbeteringen volgen in samenwerking met het team.
