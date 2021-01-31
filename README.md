#### 1. Ensure you have an instance of PostgresSQL running locally

You can;

1. Install PostgreSQL
1. Use Docker to run PostgreSQL

To use Docker to run PostgreSQL locally, use the following command:

```bash
docker run -p 5432:5342 -e POSTGRES_USER=bakhtawar -e POSTGRES_PASSWORD=Bakhtawar123 -d bakhtawar
```

#### 2. Create an empty database called bakhtawar

If you used the Docker command in the previous step, you can skip this step.

#### 3. Create a user called bakhtawar

If you used the Docker command in the previous step, you can skip this step.

Remember to:

- set the password for this user to `Bakhtawar123`
- grant all privileges on database `bakhtawar` to user `bakhtawar`

#### 4. Ensure you have .NET 5.0 installed

To install .NET 5.0, please visit [https://dotnet.microsoft.com/download/dotnet/5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

#### 5. Ensure you have Node.js installed

To install Node.js 14.x, please visit [https://nodejs.org/en/](https://nodejs.org/en/)

#### 6. Clone this repository

From your work folder:

```bash
git clone git@github.com:khawajaumarfarooq/Bakhtawar.git Bakhtawar
cd Bakhtawar
```

This will bring you to the repository root.

#### 7. Build the solution

From the repository root:

```bash
dotnet restore
dotnet build
```

#### 8. Run the DatabaseApp utility

From the repository root:

```bash
dotnet run --project Projects/Bakhtawar.Apps.DatabaseApp/Bakhtawar.Apps.DatabaseApp.csproj
```

This will run the database migrations and create all the tables necessary for the solution

#### 9. Generate the certificates

From the repository root:

```bash
cd Scripts
./generate-ca-certificate.sh
./generate-localhost-certificate.sh
```

This will generate the a certificate in `~/Keys/huntingdonresearch.crt` 

Add this certificate to your machine's CA root. You will have to do it for your machine, as well as for your browser.

This will also create a certificate in `Keys/localhost.crt` which will be used as the SSL certificate by the 3 applications.

#### 10. Run the solution

From the repository root (in three different bash shells):

```bash
cd Projects/Bakhtawar.Apps.GatewayApp
cat dev.json | dotnet user-secrets set 
dotnet run
```

```bash
cd Projects/Bakhtawar.Apps.WebFrontendApp
cat dev.json | dotnet user-secrets set 
dotnet run
```

```bash
cd Projects/Bakhtawar.Apps.BackendApp
cat dev.json | dotnet user-secrets set 
dotnet run
```

This will run the 3 applications on the following ports:

- GatewayApp on port 7443
- WebFrontendApp on port 8443
- BackendApp on port 9443

