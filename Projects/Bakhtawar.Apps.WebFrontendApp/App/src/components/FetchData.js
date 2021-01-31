import React, { useEffect, useState } from 'react';
import { authorizationManager } from '../services/authorization-manager';
import { prefixHolder } from '../services/prefix-holder';

const ForecastsTable = ({ forecasts }) => {
  return (
    <table className='table table-striped'>
      <thead>
      <tr>
        <th>Date</th>
        <th>Temp. (C)</th>
        <th>Temp. (F)</th>
        <th>Summary</th>
      </tr>
      </thead>
      <tbody>
      {
        forecasts.map((forecast) =>
          <tr key={forecast.date}>
            <td>{forecast.date}</td>
            <td>{forecast.temperatureC}</td>
            <td>{forecast.temperatureF}</td>
            <td>{forecast.summary}</td>
          </tr>
        )
      }
      </tbody>
    </table>
  );
};

export const FetchData = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [forecasts, setForecasts] = useState([]);

  useEffect(() => {
    const populateWeatherData = async () => {
      const token = await authorizationManager.getAccessToken();
      const response = await fetch(`${prefixHolder.prefix}/weather-forecast`, {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
      });
      const forecasts = await response.json();

      setForecasts(forecasts);
      setIsLoading(false);
    };

    populateWeatherData().then();

    return () => {
    };
  }, []);

  return (
    <>
      <h1>Weather forecast</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {
        isLoading
          ? <p><em>Loading...</em></p>
          : <ForecastsTable forecasts={forecasts}/>
      }
    </>
  );
};
