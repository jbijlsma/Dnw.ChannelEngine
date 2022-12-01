# Introduction

Proof of Concept (PoC) using the DAPR service-invocation, pubsub & actors components to build an application.

In this case the PoC is an application based on a job interview at ChannelEngine. ChannelEngine allows merchants to offer their products on multiple marketplaces (channels) and needs a mechanism to periodically push product updates to all market places.

Actors are a natural way to model long running processes such as pushing product updates. Each merchant channel is implemented as an actor that uses reminders to schedule new product update cycles. In this example no real product updates are performed; the actor simply waits for a random number of seconds. The actor notifies interested parties by using a pubsub channel. In this example the parties listening. In this case both the Dnw.ChannelEngine.AdminUI & Dnw.ChannelEngine.MerchantManager are interested in these updates.    

# Running locally

You need to have a k8s cluster with a local registry at localhost:5001/ before running the deploy script. Then run the script like this:

```shell
cd ./k8s
./deploy_local.sh
```

This will deploy all components to the 'ce' namespace. To view all the running pods use:

```shell
kubctl get po -n ce
```

Then use port-forwarding to access the Dnw.ChannelEngine.AdminUI web application (replace ... with the pod name):

```shell
kubectl port-forward -n ce admin-ui-... 5050:5050
```

Browse to http://localhost:5050 to see the admin web UI. You can open multiple browser tabs / windows.

Click on the "Start Simulation" button to start the simulation. Merchant channels should start appearing almost immediately in all browser tabs / windows. 

To stop the simulation click on "Stop Simulation". This will cause the Dnw.ChannelEngine.MerchantManager to send a message to all merchant actors to stop 

