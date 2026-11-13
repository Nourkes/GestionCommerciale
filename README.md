# Mini-projet — Gestion commerciale
 
Application web de gestion des **clients**, **produits** et **commandes**, avec calcul automatique des totaux HT / TTC (TVA 19 %) et mise à jour du stock à la validation d'une commande.
 
**Stack :** .NET 8 (ASP.NET Core Web API, Entity Framework Core) · SQL Server · Angular · Bootstrap · JWT
 
---
 
## 1. Prérequis
 
| Outil | Version |
| --- | --- |
| SDK .NET | 8.0 |
| SQL Server | 2019 ou plus récent (Express, Developer ou LocalDB) |
| Node.js | 20 LTS ou 22 LTS |
| Angular CLI | `npm install -g @angular/cli` |
| Visual Studio 2022 *(recommandé pour le back-end)* | » |
 
---
 
## 2. Lancer le back-end
 
### a) Configurer la base de données
 
Dans `GestionCommerciale.Api/appsettings.json`, adaptez si besoin la valeur de `Server` à votre installation de SQL Server :
 
```json
"DefaultConnection": "Server=localhost;Database=GestionCommercialeDb;Trusted_Connection=True;TrustServerCertificate=True"
```
 
| Installation | Valeur de `Server` |
| --- | --- |
| Instance par défaut | `localhost` |
| SQL Server Express | `.\\SQLEXPRESS` |
| LocalDB | `(localdb)\\MSSQLLocalDB` |
 
### b) Créer la base
 
La base est créée à partir des migrations Entity Framework Core.
 
Dans Visual Studio, *Outils → Gestionnaire de package NuGet → Console du Gestionnaire de package*, projet par défaut `GestionCommerciale.Infrastructure` :
 
```powershell
Update-Database
```
 
### c) Démarrer l'API
 
Ouvrir `GestionCommerciale.sln` dans Visual Studio, vérifier que `GestionCommerciale.Api` est le projet de démarrage, puis **F5**.
 
Ou en ligne de commande :
```bash
cd GestionCommerciale.Api
dotnet run --launch-profile https
```
 
- API : **https://localhost:7001**
- Documentation des endpoints (Swagger) : **https://localhost:7001/swagger**
> Si le navigateur signale un certificat non approuvé : `dotnet dev-certs https --trust`, puis redémarrer le navigateur.
 
---
 
## 3. Lancer le front-end
 
L'API doit être démarrée. Dans un terminal :
 
```bash
cd frontend
npm install
ng serve -o
```
 
Application : **http://localhost:4200**
 
---
 
## 4. Informations de connexion
 
| Nom d'utilisateur | Mot de passe |
| --- | --- |
| `admin` | `Admin@123` |
 
Le compte est créé automatiquement au premier démarrage de l'API, à partir de la section `DefaultAdmin` de `appsettings.json`. Le mot de passe est stocké haché.
 
---
 
## 5. Architecture
 
### Back-end : architecture en couches
 
```
Api ──────────────► Application ──► Domain
 └──► Infrastructure ──┘
```
 
| Projet | Rôle |
| --- | --- |
| `GestionCommerciale.Domain` | Entités (`Client`, `Product`, `Order`, `OrderLine`, `User`), statuts, exceptions métier |
| `GestionCommerciale.Application` | DTOs, interfaces, services contenant les règles métier |
| `GestionCommerciale.Infrastructure` | EF Core (DbContext, configurations, migrations), repositories, JWT |
| `GestionCommerciale.Api` | Contrôleurs, gestion globale des erreurs, authentification, Swagger |
 
- Les contrôleurs ne contiennent aucune logique métier : ils délèguent aux services.
- Application définit les interfaces des repositories, Infrastructure les implémente : les règles métier ne dépendent pas d'EF Core.
### Front-end
 
```
frontend/src/app/
├─ core/        modèles, services HTTP, intercepteur JWT, guard, constantes
├─ layout/      barre de navigation
├─ features/    un dossier par module (auth, clients, products, orders)
└─ shared/      composants et utilitaires réutilisables
```
 
