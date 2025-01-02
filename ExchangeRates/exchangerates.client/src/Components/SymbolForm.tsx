import React from "react";
import './SymbolForm.css';

interface ISymbolFormProps {
    handleSubmittedSymbol: (symbol: string) => void
}
function SymbolForm(props: ISymbolFormProps) {
    const { handleSubmittedSymbol } = props;

    const inputRef = React.useRef<HTMLInputElement>(null);
    const buttonRef = React.useRef<HTMLButtonElement>(null);
    React.useEffect(() => {
        inputRef.current && inputRef.current.focus();
    }, []);

    const [
        symbol,
        setSymbol,
    ] = React.useState('');
    const isFull = symbol.length > 2;

    return (
        <form className="symbolInput" onSubmit={(event) => {
            event.preventDefault();
            if (isFull) {
                handleSubmittedSymbol(symbol);    
            }            
        }}>
          <input
              ref={inputRef}
              value={symbol}
                onChange={(event) => {
                    setSymbol(event.target.value.toUpperCase().substring(0,3));
              }}
            />
            <button
                ref={buttonRef}
                disabled={!isFull}
            >Convert</button>
      </form>
  );
}


export default SymbolForm;