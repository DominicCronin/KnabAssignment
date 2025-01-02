
function NotFound(props: {symbol: string}) {
    const { symbol } = props
    return (
      <>
        <p>Didn't find any data for symbol {symbol}</p>
        <p>Please have another go!</p>
      </>
  );
}

export default NotFound;