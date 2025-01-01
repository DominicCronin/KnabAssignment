import React from "react";

interface ISymbolFormProps {
    handleSubmittedSymbol: (symbol: string) => void
}
function SymbolForm(props: ISymbolFormProps) {
    const { handleSubmittedSymbol } = props;

    const inputRef = React.useRef<HTMLInputElement>(null);
    React.useEffect(() => {
        inputRef.current && inputRef.current.focus();
    }, []);

    const [
        symbol,
        setSymbol,
    ] = React.useState('');

    return (
        <form onSubmit={(event) => {
            event.preventDefault();
            handleSubmittedSymbol(symbol);
        }}>
          <input
              ref={inputRef}
              value={symbol}
              onChange={(event) => {
                  setSymbol(event.target.value);
              }}
          />
          <button>Convert</button>
      </form>
  );
}


export default SymbolForm;