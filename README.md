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
# IMPLEMENTATION CHOICES
- The Api is supposed to be RESTFul meaning the routes are organized by entities and usually  the enities state may be managed using http verb (POST/PUT/DELETE...). thus the name **REpresentational State Transfer**
- I chose to tweak it a bit. By keeping the organization by entities, but instead of sending the full state when an update is requested (using PUT), I rather send an object representing the update requested (a Command).
- this is inspired by CQRS. which is a way to organize the code by seggregating  commands an queries
- one of the advantages is to (1) clearly identify the usecases and (2) avoid unwanted updates by not exposing the entity state when there is no need to do so.
- also a clear idea of the usecases is quite necessary when applying DDD principles 
- I used MediaTR to accelearate the setup of my commands and queries
- In our code the only entity we have is the DayOffRequest
- the DayOffRequest lifecycle is explicitly implemented inthe **DayOffRequestsService** class and looks like this
  ![state diagram](./day-off-state.png)
- The User should also be an entity but we chose to limit our time spend on the project
 
