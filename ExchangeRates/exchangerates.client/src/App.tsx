import { useState } from 'react';
import './App.css';
import SymbolForm from './Components/SymbolForm';
import Conversion, { IConversionProps } from './Components/Conversion';
import NotFound from './Components/NotFound';

function App() {

    const [symbol, setSymbol] = useState<string>('');
    const emptyConversion: IConversionProps = { cryptoCurrencySymbol: '', fiatConversions: [] }
    const [conversions, setConversions] = useState<IConversionProps>(emptyConversion)
    const [hasDoneConversion, setHasDoneConversion] = useState(false);
    
    return (
        <>
        <div>
            <h1 >Currency conversion</h1>
                <SymbolForm handleSubmittedSymbol={handleSubmittedSymbol} />
                {conversions.cryptoCurrencySymbol && < Conversion {...conversions} />}                
                {hasDoneConversion && !conversions.cryptoCurrencySymbol && <NotFound symbol={symbol} />}                
            </div>
        </>
    );

    async function handleSubmittedSymbol(symbol: string) {
        setHasDoneConversion(true);
        setSymbol(symbol)
        const response = await fetch(`convert?symbol=${symbol}`)
        if (response.ok) {
            const data = await response.json();
            setConversions(data)
        }
        else {
            setConversions(emptyConversion)
        }
    }
}

export default App;