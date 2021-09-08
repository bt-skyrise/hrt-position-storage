# Helsinki Region Transport position storage

A simple .NET console app that subscribes to high-frequency vehicle positioning MQTT broker from Helsinki Region Transport and stores all events in MongoDB.

Live data from HRT is used in [Real-time Map](https://github.com/AsynkronIT/realtimemap), which is a [Proto.Actor](https://proto.actor/) library showcase. [Proto.Actor](https://proto.actor/) is a cross-platform actors solution for .NET and Go.

The purpose of this app is to gather some events in case HRT stops publishing them in the future (so the [Real-time Map](https://github.com/AsynkronIT/realtimemap) can still be run).

Possibly this repo will contain a second app for publishing previously collected events.

More info on data:
* [Helsinki Region Transport - open data](https://www.hsl.fi/en/hsl/open-data)
* [High-frequency positioning from HRT](https://digitransit.fi/en/developers/apis/4-realtime-api/vehicle-positions/)

## Running

Start MongoDB:

```
docker run --name helsinki-mongo -d -p 27017:27017 mongo:4.4.8
```

Run the app:

```
cd HelsinkiRegionTransportPositionStorage.Recorder
dotnet run
```

## HRT data license

© Helsinki Region Transport 2021
Creative Commons BY 4.0 International