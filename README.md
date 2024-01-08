# CurrencyServer

CurrencyServer is a simple REST API created in ASP.NET utilizing MVC pipeline for the routing. The purpose of the project is to handle currency exchange values, this project currently only supports checking the delta value between one currency and a set of other currencies. Created for Genero code test. The program was created in .NET Core 6.0.417

## Running Locally

To run this project locally, follow these steps:

### Step 1: Clone the Repository

```bash
git clone https://github.com/jeppe9821/CurrencyServer.git
```

### Step 2: Insert API Key
Insert your API key from ExchangeRatesAPI (https://exchangeratesapi.io/) into the Dockerfile, replace API_KEY_HERE with your actual API key. The dockerfile can be found under CurrencyServer/Dockerfile

```bash
ENV API_KEY=API_KEY_HERE
```

### Step 3: Build the program
Open a command prompt at the root folder and run:

```bash
dotnet build
```

This should generate a DLL under \bin\Debug\net6.0\CurrencyServer.dll

### Step 4: Build Docker Image
Navigate to the root folder in the terminal and run the following command to build the docker image:

```bash
docker build -f "Dockerfile" --force-rm -t currencyserver:dev --target base --build-arg "BUILD_CONFIGURATION=Debug" "."
```

### Step 5: Start Docker Container
Replace PATH_TO_REPO with the path to the git repository and then from the root folder run the following command:

```bash
docker run -dt -v "PATH_TO_REPO\CurrencyServer:/app" -v "PATH_TO_REPO:/src/" -p 8080:80 --name CurrencyServer currencyserver:dev
```

Now take the ID of the docker container and use it for the next step

### Step 6: Run the CurrencyServer
Replace CONTAINER_ID with the id of the container

```bash
docker exec -i -e ASPNETCORE_URLS="http://+:80" -w "/app" CONTAINER_ID sh -c ""dotnet" \"/app/bin/Debug/net6.0/CurrencyServer.dll\"
```

Now the container should be running the program on port 8080 and can be interacted with using CURL or other API calls

## Alternative build
Instead of running the commands manually, you can also just run this batch script in the git repository root folder (api key needs to be changed in dockerfile before, see step 2)
```bash
@echo off

echo.
echo.
echo \======Building the program======\
REM Step 2: Build the program
cd CurrencyServer
dotnet build
cd ..

echo.
echo.
echo \======Stopping and removing previous docker container======\
docker stop CurrencyServer
docker rm CurrencyServer

echo.
echo.
echo \======Building the docker image======\
docker build -f "CurrencyServer\Dockerfile" --force-rm -t currencyserver:dev --target base --build-arg "BUILD_CONFIGURATION=Debug" "CurrencyServer"

echo.
echo.
echo \======Start docker container======\
docker run -dt -v "C:\Dev\CurrencyServer-main\CurrencyServer:/app" -v "C:\Dev\CurrencyServer-main:/src/" -p 8080:80 --name CurrencyServer currencyserver:dev

echo.
echo.
echo \======Insert the ID from above======\
set /p CONTAINER_ID=Enter the ID of the Docker container: 

echo.
echo.
echo \======Run CurrencyServer.dll from the docker container======\
echo The API can now be interacted with by localhost:8080
echo.
docker exec -i -e ASPNETCORE_URLS="http://+:80" -w "/app" %CONTAINER_ID% sh -c "dotnet /app/bin/Debug/net6.0/CurrencyServer.dll"
```


## Usage
```bash
curl --location --request POST "http://localhost:8080/currencydelta" --header "Content-Type: application/json" --data-raw "{\"baseline\": \"GBP\",\"currencies\": [\"USD\", \"SEK\"],\"fromDate\": \"2021-09-01\",\"toDate\": \"2023-08-30\"}"
```

### API Endpoints
/currencydelta => Returns a list of currency exchange values based on the delta between the baseline currency to the selected currencies

Example body:
```bash
{
    "baseline": "GBP",
    "currencies": ["usd", "SEK"],
    "fromDate": "2021-09-01",
    "toDate": "2023-08-30"
}
```

