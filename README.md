## Please Note

- The [MetaWeather API](https://www.metaweather.com/api/) no longer seems to exist. To solve this problem I researched the OpenWeather API and the AccuWeather API.
- I selected the AccuWeather API because I found their unique "location keys" to be a more readable and accurate solution than OpenWeather's use of latitude/longitude coordinates.
- The AccuWeather API limits free accounts to 50 API calls per day.
- If this application exceeds those service limits, it will not load new data into the weather forecast table (though it will log an accurate error).
- If for some reason more than 50 calls to AccuWeather's API need to be made, the steps below can be followed to generate a new API key.
- **NOTE:** An existing API key has been stored in this repository for ease of use since it is a coding assessment. Outside of a development environment this should be stored as a proper secret.

## How to Generate a New AccuWeather API Key
1. Register a free [AccuWeather](https://developer.accuweather.com/) account
1. AccuWeather should send an email with a one-time login link to set a password for the new account
1. Navigate to [My Apps](https://developer.accuweather.com/user/me/apps)
1. Click "Add a New App"
1. Give the app a name and select "Core Weather Limited Trial" as the product. Fill out the rest of the required fields as you wish.
1. Click "Create App"
1. Click your new app and copy the key listed under "API Key" and replace the value of "AccuWeatherDevAPIKey" with this key

___

## Assignment

Welcome to a fun interview project.  Please do not take more than a few hours to complete and submit this project.

## Getting Started

1. Clone this repo
1. Install [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)
1. Install [node.js](https://nodejs.org/en/download/)
1. Install [Visual Studio](https://visualstudio.microsoft.com/vs/community/) or [Visual Studio Code](https://code.visualstudio.com/download)
1. Run (npm takes a few minutes on first launch, so be patient)

## Your Task

This project in its current state is a slightly modified version of the output from `dotnet new react` using ASP.NET Core 3.0.  Right now, static weather data is displayed in a table.

- Modify the project to allow the user to search for a location.
- Display a five day forecast for that location.  
- Use the [Metaweather API](https://www.metaweather.com/api/) which is free and well documented.  

## What we're looking for

- Working features
- Unit tests
- Clean, readable code
- Good design principles
- Commit history that is easy to follow

## Bonus points

- React function components and hooks
- Input validation
- Integration tests
- Resilient API calls
- Security
- Great user experience

When you are done, push your code to a public git repo, either on github or elsewhere, and send it our way.  Good luck!
