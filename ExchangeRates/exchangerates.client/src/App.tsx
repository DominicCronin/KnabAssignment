import { useEffect, useState } from 'react';
import './App.css';
import SymbolForm from './Components/SymbolForm';
import Conversion from './Components/Conversion';

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

function App() {
    const [forecasts, setForecasts] = useState<Forecast[]>();
    const [symbol, setSymbol] = useState<string>('');

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
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
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <>
        <div>
            <h1 id="tableLabel">Weather forecast from App.tsx</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
            </div>

            <SymbolForm handleSubmittedSymbol={setSymbol} />
            <Conversion symbol={symbol} />
        </>
    );

    async function handleSubmittedSymbol(symbol: string) {
        const response = await fetch('convert')
    }

    async function populateWeatherData() {
        const response = await fetch('weatherforecast');
        if (response.ok) {
            const data = await response.json();
            setForecasts(data);
        }
    }
}

export default App;