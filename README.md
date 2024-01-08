# CurrencyServer

CurrencyServer is a simple REST API created in ASP.NET utilizing MVC pipeline for the routing. The purpose of the project is to handle currency exchange values, this project currently only supports checking the delta value between one currency and a set of other currencies. Created for Genero code test 

## Running Locally

To run this project locally, follow these steps:

### Step 1: Clone the Repository

```bash
git clone https://github.com/jeppe9821/CurrencyServer.git
```

### Step 2: Insert API Key
Insert your API key from ExchangeRatesAPI (https://exchangeratesapi.io/) into the Dockerfile, replace API_KEY_HERE with your actual API key

```bash
ENV API_KEY=API_KEY_HERE
```

### Step 3: Build Docker Image
Navigate to the root folder in the terminal and run the following command to build the docker image:

```bash
docker build -f "Dockerfile" --force-rm -t currencyserver:dev --target base --build-arg "BUILD_CONFIGURATION=Debug" "."
```

### Step 4: Start Docker Container
In the root folder, in your terminal run the following command to create and run the container:

```bash
docker run -dt -p 80:80 --name CurrencyServer currencyserver:dev
```

Now the container should have a unique port which can be used for API requests

## Usage
```bash
curl --location --request POST 'http://localhost:INSERT_PORT_HERE_FROM_DOCKER/currencydelta' \
--header 'Content-Type: application/json' \
--data-raw '{
    "baseline": "GBP",
    "currencies": ["USD", "SEK"],
    "fromDate": "2021-09-01",
    "toDate": "2023-08-30"
}'
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

