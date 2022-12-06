import React, { useState, useEffect } from 'react';

import { toast } from 'react-toastify';

export const MerchantChannelSimulation = () => {
  const toastId = React.useRef(null);
  const collator = new Intl.Collator(undefined, {numeric: true, sensitivity: 'base'});
  
  const [ merchantChannels, setMerchantChannels ] = useState([]);
  const [ startBtnDisabled, setStartBtnDisabled ] = useState(false);
  const [ stopBtnDisabled, setStopBtnDisabled ] = useState(false);
  
  useEffect(() => {
    const eventSource = new EventSource('updates');
    eventSource.onmessage = handleMessage;
  }, []);
  
  const handleMessage = (msg) => {
    const data = JSON.parse(msg.data);
    console.log(data);

    if (data.messageType === 'ChannelProductRefreshStopped') {
      handleChannelProductRefreshStoppedMessage(data);
    } else {
      handleOtherMessage(data);  
    } 
  }
  
  const handleChannelProductRefreshStoppedMessage = (msg) => {
    setMerchantChannels(prevState => {
      const { found, index } = findMerchantStatusById(`${msg.merchantId}:${msg.merchantChannelName}`, prevState);
      if (found) {
        const newState = [...prevState];
        const updatedChannelStatus = { ...newState[index], status: 'Stopped' };
        newState.splice(index, 1, updatedChannelStatus);

        if (newState.every(m => m.status === 'Stopped')) {
          showSuccessToast('Simulation stopped successfully!');
        }
        
        return newState;
      }
      
      return prevState;
    });
  }
  
  const handleOtherMessage = (msg) => {
    const status = msg.startedAt ? 'running' : '------';
    let newChannelStatus = new MerchantChannelStatus(msg.merchantId, msg.merchantName, msg.merchantChannelName, msg.startedAt, msg.completedAt, status, msg.runningOn);
    
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

  const start = async () => {
    try {
      setStartBtnDisabled(true);
      setMerchantChannels([]);
      await fetch('merchant/simulation/start');
      showSuccessToast("Simulation started!");
    }
    catch (err) {
      showErrorToast(err);    
    } finally {
      setStartBtnDisabled(false);
    }
  }

  const stop = async () => {
    try {
      setStopBtnDisabled(true);
      await fetch('merchant/simulation/stop');
      showInfoToast("Simulation stopping. Wait until all merchant channels have the 'Stopped' status  ...");
    }
    catch (err) {
      showErrorToast(err);
    } finally {
      setStopBtnDisabled(false);
    }
  }

  const findMerchantStatusById = (searchId, items) => {
    'use strict';
    
    let minIndex = 0;
    let maxIndex = items.length - 1;
    let currentIndex = 0;
    let currentElement;
    let compare = 0;

    while (minIndex <= maxIndex) {
        currentIndex = Math.floor((minIndex + maxIndex) / 2);
        currentElement = items[currentIndex];

        compare = collator.compare(searchId, currentElement.id);
        if (compare < 0) {
            maxIndex = currentIndex - 1;
        }
        else if (compare > 0) {
            minIndex = currentIndex + 1;
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
        index: !currentElement || compare < 0 ? currentIndex : currentIndex + 1 
    };
  }

  const showSuccessToast = (msg) => {
    showToast(toast.TYPE.SUCCESS, msg);
  }

  const showInfoToast = (msg) => {
    showToast(toast.TYPE.INFO, msg);
  }

  const showErrorToast = (msg) => {
    showToast(toast.TYPE.ERROR, msg);
  }

  const showToast = (type, msg) => {
    if (!toast.isActive(toastId.current)) {
      toastId.current = toast(msg, { toastId: 'merchantSimulationToast', type: type });  
    }
    else {
      toast.update(toastId.current, {
        render: msg,
        type: type
      });
    }
  }
  
  return (
    <div>
      <h1 id="tabelLabel" >Merchant Channel Refresh Status</h1>
      <p>This component shows how to use DAPR actors, pubsub and Server Side Events (SSE)</p>
      <div>
        <button disabled={startBtnDisabled} onClick={start}>Start Simulation</button>
        &nbsp;
        <button disabled={stopBtnDisabled} onClick={stop}>Stop Simulation</button>
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
