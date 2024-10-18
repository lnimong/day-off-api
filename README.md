# HOW TO RUN
## I - using docker

### prerequisites 
-  [docker](https://docs.docker.com/engine/install/) is installed

### steps
1 - build the image
```
docker build . -t your-image-name 
```

2 - run the container
when running the container we must 
(1) use the -p option to map the conainer exposed port (which is 8080) to the port you want to the api to respond on 
(2) use the -v option to map the container data directory (which is /App/vol) to any directory you want the api data to be stored in
```
docker run -p 8888:8080 -v /your/data/directory:/App/vol tf1img
```
in the example above the app will respond on **http://localhost:8888** and the data will be stored in **/your/data/directory/data.json,**


## II - using dotnet cli

### prerequisites 
-  [dotnet runtime 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is installed

### steps
1 - run tests from the repository root directory
```
 dotnet test
```
2 - run the api from the repository root directory
```
 dotnet run --project .\Tf1DayOff.Api
```
---------------------------------------------
---------------------------------------------
# ENDPOINTS

### POST https://localhost:7114/day-off/new-request

```
curl -X 'POST' \
  'https://localhost:7114/day-off/new-request' \
  -H 'accept: */*' \
  -H 'X-User: the_user' \
  -H 'Content-Type: application/json' \
  -d '{
  "type": "Sick",
  "comment": "string",
  "start": "2024-10-18T12:24:44.788Z",
  "end": "2024-10-18T12:24:44.788Z"
}'
```
creates a new day off request
the user who creates the request is passed as a header (X-User)

#### body
the request details
- type : possible Values are "Sick" or "Vacation"
- comment :  a string that add complementary details to the request
- start : the start date of the request
- end : the end date of the request




### POST https://localhost:7114/day-off/validate-request/56a42cdd-2647-4f8f-b162-74e3a4a25457

```
curl -X 'POST' \
  'https://localhost:7114/day-off/validate-request/56a42cdd-2647-4f8f-b162-74e3a4a25457' \
  -H 'accept: */*' \
  -H 'X-User: the_user' \
  -H 'Content-Type: application/json' \
  -d '{
  "comment": "string",
  "action": "Validate"
}'
```
validate a new day off request
the user who validate the request is passed as a header (X-User)

#### the route param 
it is a GUID which is the id of the request to be updated

#### the body :
the validation details
- comment :  a string that add complementary details to the request validation (or refusal)
- action : possible values are "Validate" or "Reject"




### GET https://localhost:7114/day-off?status=Pending&status=Accepted&type=Sick&type=Vacation
```
curl -X 'GET' \
  'https://localhost:7114/day-off?status=Pending&status=Accepted&type=Sick&type=Vacation' \
  -H 'accept: text/plain' \
  -H 'X-User: the_user'
```

retrieve the list of day off requests for the current user
the current user is passed as a header (X-User)

#### query params
status : the status filter (non mandatory)
- possible Values are  "New", "Pending", "Accepted", "Rejected" (non case sensitive)
type : the type filter (non mandatory)
- possible Values are "Sick" or "Vacation" (non case sensitive)

---------------------------------------------
---------------------------------------------


# IMPLEMENTATION CHOICES
### The Api is supposed to be RESTFul...
- ...meaning the routes are organized by entities and usually  the enities state may be managed using http verb (POST/PUT/DELETE...). thus the name **REpresentational State Transfer**
- I chose to tweak it a bit. By keeping the organization by entities, but instead of sending the full state when an update is requested (using PUT), I rather send an object representing the update requested (a Command).
- this is inspired by **CQRS**. which is a way to organize the code by seggregating  commands an queries
- one of the advantages is to (1) clearly identify the usecases and (2) avoid unwanted updates by not exposing the entity state when there is no need to do so.
- also a clear idea of the usecases is quite necessary when applying **DDD** principles 
- I used MediaTR to accelearate the setup of my commands and queries
- In our code the only entity we have is the DayOffRequest
- the DayOffRequest lifecycle is explicitly implemented inthe **DayOffRequestsService** class and looks like this
  ![state diagram](./day-off-state.png)
- The User should also be an entity but we chose to limit our time spend on the project


### For the testing part...
 - ...to gain time, end ensure the API works (not just some part of it), I chose to have my entire Api as my system under test. Meaning, for each automated test, the api setup code is also triggered.
```
// WebApplicationFactory.cs
    public static HttpClient CreateHttpClientFor<TProgram>(Action<IWebHostBuilder>? registerMock = null)  where TProgram : class
    {
        var factory = new WebApplicationFactory<TProgram>()
            .WithWebHostBuilder(builder =>
            {
                //Environment.SetEnvironmentVariable...

                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    //configurationBuilder.AddJsonFile...
                });


                builder.ConfigureTestServices(services =>
                {
                    //services.AddAuthentication(options => ...);
                });

                registerMock?.Invoke(builder);
            });

        return factory.CreateClient();
    }
```
 - it means that classes like Error management (ExceptionFilters), Middelware are also tested
 - That allows me to actually check the api return codes (400, 404, 500 ...)
 - note that there is no actual http calls and it is stil possible to setup Mocks (usefull to handle Time or Data access)

```
// BaseDayOFfTests.cs
[SetUp]
public void Setup()
{
    FakeClock = Substitute.For<IClock>();
    Repository = new InMemStaticDayOffRequestsRepository();

    Client = HttpClientFactory.CreateHttpClientFor<Tf1DayOFf.Api.Program>(
        registerMock: x =>
        {
            x.MockDependency(FakeClock);
            x.MockDependency<IDayOffRequestsRepository>(Repository);
        });
}
```
