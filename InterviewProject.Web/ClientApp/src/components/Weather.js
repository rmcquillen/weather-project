import React, { Component } from 'react';

export class Weather extends Component {
  static displayName = Weather.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true, location: "" };

    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(forecasts) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
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
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Weather.renderForecastsTable(this.state.forecasts);

    return (
      <div>
        <h1 id="tabelLabel" >5 Day Weather Forecast</h1>
        <form onSubmit={this.handleSubmit}>
          <input
            type="text"
            placeholder="Enter location..."
            onChange={this.handleChange} />
          <input type="submit" value="Go!" />
        </form>
        {contents}
      </div>
    );
  }

  handleChange = (e) => {
    e.preventDefault();
    this.setState({ location: e.target.value });
  };

  handleSubmit(e) {
    e.preventDefault();
    this.populateWeatherData(this.state.location);
  };

  async populateWeatherData(location) {
    if (location === undefined) {
      location = '';
    }

    const response = await fetch('http://localhost:5000/weatherforecast/' + location);

    const data = await response.json();

    this.setState({ forecasts: data.dailyForecasts, loading: false });  }
}
