import React, { useState } from 'react';

export const Count = () => {
  const [count, setCount] = useState(0);

  const incrementCount = () => {
    setCount((count) => count + 1);
  };

  const decrementCount = () => {
    setCount((count) => count - 1);
  };

  return (
    <>
      <h1>Counter</h1>
      <p>This is a simple example of a React component.</p>
      <p aria-live="polite">Current count: <strong>{count}</strong></p>
      <div className="btn-group">
        <button className="btn btn-primary" onClick={incrementCount}>Increment</button>
        <button className="btn btn-secondary" onClick={decrementCount}>Decrement</button>
      </div>
    </>
  );
};
