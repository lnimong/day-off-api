# How to run
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
1 - run the api from the repository root directory
```
 dotnet run --project .\Tf1DayOff.Api
```

