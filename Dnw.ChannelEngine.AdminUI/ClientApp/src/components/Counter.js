import React, { useState } from 'react';

export const Counter = () => {
    const [ state, setState ] = useState({ currentCount: 0 });
    
    return (
        <div>
            <h1>Counter</h1>
    
            <p>This is a simple example of a React component.</p>
    
            <p aria-live="polite">Current counter: <strong>{state.currentCount}</strong></p>
    
            <button className="btn btn-primary" onClick={() => setState(prev => { return { currentCount: prev.currentCount + 1}; })}>Increment</button>
        </div>
    );
}
