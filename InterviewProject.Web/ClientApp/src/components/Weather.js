import React, { Component } from 'react';

export class Weather extends Component {
  static displayName = Weather.name;

  constructor(props) {
    super(props);
    this.state = { summary: "", forecasts: [], loading: true, locationInput: "", forecastLocation: "", invalid: false };

    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(summary, forecasts, location) {
    return (
      <div>
        <h2 id="locationLabel">Your 5 Day Summary for {location}:</h2>
        <h4>{summary}</h4>
        <table className='table table-striped' aria-labelledby="tableLabel">
          <thead>
            <tr>
              <th>Date</th>
              <th>Temp. (C)</th>
              <th>Temp. (F)</th>
              <th>Summary</th>
            </tr>
          </thead>
          <tbody>
            {forecasts.map(forecast =>
              <tr key={forecast.date}>
                <td>{forecast.dateString}</td>
                <td>{forecast.temperature.maximum.value} &deg;C</td>
                <td>{Math.round(32 + (forecast.temperature.maximum.value / 0.5556))} &deg;F</td>
                <td>{forecast.day.iconPhrase}</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Weather.renderForecastsTable(this.state.summary, this.state.forecasts, this.state.forecastLocation);

    return (
      <div>
        <h1 id="tableLabel" >5 Day Weather Forecast</h1>
        <form onSubmit={this.handleSubmit}>
          <input
            type="text"
            className={this.state.invalid ? "border-danger" : ""}
            placeholder="Enter location..."
            required="required"
            onChange={this.handleChange} />
          <input type="submit" value="Go!" />
          {this.state.invalid && <p>No results found for "{this.state.locationInput}"</p>}
        </form>
        <br />
        {contents}
      </div>
    );
  }

  handleChange = (e) => {
    e.preventDefault();
    this.setState({ locationInput: e.target.value });
  };

  handleSubmit(e) {
    e.preventDefault();
    this.populateWeatherData(this.state.locationInput);
  };

  async populateWeatherData(location) {
    if (location === undefined) {
      location = '';
    }

    const response = await fetch('http://localhost:5000/weatherforecast/' + location);

    const data = await response.json();

    if (data.dailyForecasts === null) {
      this.setState({ invalid: true });
    }
    else {
      this.setState({ summary: data.headline.text, forecasts: data.dailyForecasts, loading: false, forecastLocation: data.location, invalid: false });
    }
  }
}
