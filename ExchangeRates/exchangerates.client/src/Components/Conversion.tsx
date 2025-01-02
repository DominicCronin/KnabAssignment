import './Conversion.css'
export interface IConversionProps {
    cryptoCurrencySymbol: string,
    fiatConversions: IFiatConversion[]
}

interface IFiatConversion {
    fiatSymbol: string,
    rate: number
}

function Conversion(props: IConversionProps) {
    const { cryptoCurrencySymbol, fiatConversions } = props
  return (
        <div>              
            <p>The following exchange rates have been retrieved for symbol {cryptoCurrencySymbol} </p>                   
            <table>
                <thead>
                    <tr>
                        <th>Currency</th>
                        <th>Rate</th>
                    </tr>
                </thead>
                <tbody>
                    {fiatConversions.map(conversion =>
                        <tr key={conversion.fiatSymbol}>
                            <td>{conversion.fiatSymbol}</td>
                            <td>{conversion.rate}</td>
                        </tr>
                    )}
                </tbody>
            </table>
         </div>
  );
}

export default Conversion;