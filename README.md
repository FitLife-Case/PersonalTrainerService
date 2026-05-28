# PersonalTrainerService

PersonalTrainerService håndterer administration af personlige trænere, træningsplaner og ernæringsplaner i FitLife Digital. Servicen giver admins mulighed for at oprette og administrere trænerprofiler, og giver trænere mulighed for at oprette planer til deres klienter.

## Funktionalitet

- **Trænerprofiler** – admins kan oprette, redigere og slette trænere
- **Træningsplaner** – trænere kan oprette og administrere træningsplaner for medlemmer
- **Ernæringsplaner** – trænere kan oprette og administrere ernæringsplaner for medlemmer
- **Booking** – medlemmer kan booke en personlig træner session via BookingService

## Routing via nginx
/personaltrainer              → Razor Page for admins (CRUD)
/api/trainers                 → REST API endpoints for trænere
/api/trainingplans            → REST API endpoints for træningsplaner
/api/nutritionplans           → REST API endpoints for ernæringsplaner

## Inter-service kommunikation

BookingService kalder PersonalTrainerService for at validere at en træner eksisterer ved booking:
GET http://haav-personaltrainer-service:8080/api/trainers/{id}

## Adgangsstyring

| Rolle | Adgang |
|-------|--------|
| `Admin` | Fuld adgang til Razor Pages og API |
| `Member` | Ingen adgang til Razor Pages, kan se trænere via BookingList |

## Miljøvariabler

| Variabel | Beskrivelse |
|----------|-------------|
| `Vault__Url` | URL til HashiCorp Vault |
| `Vault__Token` | Token til Vault autentificering |
| `GatewayUrl` | URL til nginx API gateway |
| `Loki__Url` | URL til Grafana Loki til logging |

## Vault Secrets

| Secret | Beskrivelse |
|--------|-------------|
| `Mongo__ConnectionString` | MongoDB forbindelsesstreng |
| `Mongo__DatabaseName` | Database navn  |
| `Jwt__Secret` | JWT signerings nøgle |
| `Jwt__Issuer` | JWT udsteder |
| `Jwt__Audience` | JWT målgruppe |

## API Kontrakt

Swagger dokumentation tilgængelig på `/swagger` når servicen kører.
Den fulde OpenAPI kontrakt kan findes i `Models/ApiKontrakter/swagger.json`.
