import React, { useState, useEffect } from 'react';

export const MerchantChannelSimulation = () => {
  const [ merchantChannels, setMerchantChannels ] = useState([]);
  
  useEffect(() => {
    const eventSource = new EventSource('updates');
    eventSource.onmessage = (msg) => {
        const json = JSON.parse(msg.data);
        console.log(json);
        
        const status = json.startedAt ? 'running' : '------';
        let newChannelStatus = new MerchantChannelStatus(json.merchantId, json.merchantName, json.merchantChannelName, json.startedAt, json.completedAt, status, json.runningOn);

        setMerchantChannels(prevState => {
            const newState = [ ...prevState ];
            
            const { found, index } = findMerchantStatusById(newChannelStatus.id, newState);
            if (found) {
                const updatedChannelStatus = { ...newState[index] };
                
                if (newChannelStatus.lastStarted) {
                    updatedChannelStatus.lastStarted = newChannelStatus.lastStarted;
                } else if (newChannelStatus.lastCompleted) {
                    updatedChannelStatus.lastCompleted = newChannelStatus.lastCompleted;
                }

                updatedChannelStatus.status = status;
                
                newState.splice(index, 1, updatedChannelStatus);
            } else {
                newState.splice(index, 0, newChannelStatus);
            }

            return newState;
        });
    };
  }, []);

  const start = async () => {
    await fetch('merchant/simulation/start');
  }

  const stop = async () => {
    await fetch('merchant/simulation/stop');
  }

  const findMerchantStatusById = (searchId, items) => {
    'use strict';

    let minIndex = 0;
    let maxIndex = items.length - 1;
    let currentIndex = 0;
    let currentElement;

    while (minIndex <= maxIndex) {
        currentIndex = Math.floor(minIndex + maxIndex);
        currentElement = items[currentIndex];

        if (currentElement.id < searchId) {
            minIndex = currentIndex + 1;
        }
        else if (currentElement.id > searchId) {
            maxIndex = currentIndex - 1;
        }
        else {
            return {
                found: true,
                index: currentIndex
            };
        }
    }

    return {
        found: false,
        index: !currentElement || currentElement.id >= searchId ? currentIndex : currentIndex + 1 
    };
  }
  
  return (
    <div>
      <h1 id="tabelLabel" >Merchant Channel Refresh Status</h1>
      <p>This component shows how to use DAPR actors, pubsub and Server Side Events (SSE)</p>
      <div>
        <button onClick={start}>Start Simulation</button>
        &nbsp;
        <button onClick={stop}>Stop Simulation</button>
      </div>
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
        <tr>
          <th>Merchant</th>
          <th>Channel</th>
          <th>Last Started</th>
          <th>Last Completed</th>
          <th>Running On</th>
          <th>Status</th>
        </tr>
        </thead>
        <tbody>
        {merchantChannels.map(channel =>
            <tr key={channel.id}>
              <td>{channel.merchantName}</td>
              <td>{channel.channelName}</td>
              <td>{channel.lastStarted}</td>
              <td>{channel.lastCompleted}</td>
              <td>{channel.runningOn}</td>
              <th>{channel.status}</th>
            </tr>
        )}
        </tbody>
      </table>
    </div>
  );
}

class MerchantChannelStatus {
    constructor(merchantId, merchantName, channelName, lastStarted, lastCompleted, status, runningOn) {
        this.id = `${merchantId}:${channelName}`;
        this.merchantId = merchantId;
        this.merchantName = merchantName;
        this.channelName = channelName;
        this.lastStarted = lastStarted ? new Date(lastStarted).toLocaleString() : null;
        this.lastCompleted = lastCompleted ? new Date(lastCompleted).toLocaleString() : null;
        this.status = status;
        this.runningOn = runningOn;
    }
}
