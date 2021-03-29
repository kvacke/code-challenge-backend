# Doro Backend Challenge: Traffic was a nightmare!

## Introduction

This project was my contribution to Doro's backend challenge.  
It is a traffic update subscription made with ASP.NET core and bootstrapped with 
its Web API template.

## The Challenge

To create a subscription service for SR's traffic API with the following requirements: 
- A client can register as a listener to updates using email or SMS (phone mumber)  
as identifier 
- A client must provide a geolocation for traffic notifications
- A client can update the geolocation for traffic notifications
- A client notification contains at a minimum
  -  Priority
  -  Title
  -  Location
  -  Description
  -  Category
-  A registred client will automatically unregister after 24 hours (to be polite)
-  A registred client can unsubscribe from the service

## My Solution

### Rough system structure

#### Incoming HTTP-requests:  
Requests are validated to keep the format:  

{  

  "identifier" : "some email or phone number string",  
  "longitude" : "XX.XXXXXX",  
  "latitude" : "XX.XXXXXX"  
}

and are handled by an ASP.NET Controller which checks if the identifier is in use,  
and if so is removed from the in-memory DB (since this is a sample project) and  
stored anew after getting a fresh timestamp and an updated SR area location from their /areas endpoint.  
Client can send a DELETE request to remove a subscription entirely.

#### API-fetching:  
A data access object queries the api/....../traffic endpoint and parses the  
response to XML.

#### Business logic:
Notificationservice.cs, the brain of the operation (probably has too many
responsibility).  
1. Every 10 seconds requests a fresh XML of the current traffic messages.  
2. Looks for new messages by comparing against a private cache of already seen  
<messageId, area> pairs. Adds unseen ones and notifies all subscribers in its  
area.
3. Looks for new subscribers by comparing against a private cache of already seen
<subscriber identifier, area> pairs. Adds unseen ones and finds the relevant  
messages in the cache.  
4. Resets both caches every 24 hours at 7 am.


## APIs

### GET http://api.sr.se/api/v2/traffic/messages

Gets all current traffic updates from SR.

### GET http://api.sr.se/api/v2/traffic/areas

Given a longitude and a latitude returns a corresponding geographical area in Sweden.
