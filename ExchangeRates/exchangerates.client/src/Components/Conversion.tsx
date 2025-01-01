
interface IConversionProps {
    symbol: string
}
function Conversion(props: IConversionProps) {
    const { symbol } = props
  return (
      <>
          <div>
              <h1 >Currency conversion</h1>
              <p>The following exchange rates have been retrieved for symbol {symbol} </p>                   
          </div>          
      </>
  );
}

export default Conversion;