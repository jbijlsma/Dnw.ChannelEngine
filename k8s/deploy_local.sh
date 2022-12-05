#!/bin/bash

# Define variables
RELEASE_NAME=ce

# Create the new cluster with a private container / image registry
echo "Create new KinD cluster"
#. ./create_kind_cluster.sh

# Preload 3rd party images
docker pull mcr.microsoft.com/dotnet/sdk:6.0-alpine
kind load docker-image mcr.microsoft.com/dotnet/sdk:6.0-alpine

docker pull mcr.microsoft.com/dotnet/aspnet:6.0-alpine
kind load docker-image mcr.microsoft.com/dotnet/aspnet:6.0-alpine

docker pull arm64v8/redis:latest
kind load docker-image arm64v8/redis:latest

# Build images, tag them and push them to the local registry
TAG="localhost:5001/$RELEASE_NAME-admin-ui:latest"
echo "TAG=$TAG"
docker build -t $TAG -f ../Dnw.ChannelEngine.AdminUI/Dockerfile ../.
docker push $TAG
kind load docker-image $TAG

TAG="localhost:5001/$RELEASE_NAME-actors-host:latest"
echo "TAG=$TAG"
docker build -t $TAG -f ../Dnw.ChannelEngine.Actors.Host/Dockerfile ../.
docker push $TAG
kind load docker-image $TAG

TAG="localhost:5001/$RELEASE_NAME-merchant-manager:latest"
echo "TAG=$TAG"
docker build -t $TAG -f ../Dnw.ChannelEngine.MerchantManager/Dockerfile ../.
docker push $TAG
kind load docker-image $TAG

# Install app into k8s cluster
helm upgrade "$RELEASE_NAME" ./helm \
  --install \
  --namespace "$RELEASE_NAME" \
  --create-namespace \
  --set PubSubType="$PUB_SUB_TYPE"
  
# Restart the deployments
kubectl rollout restart "deployment/admin-ui" -n "$RELEASE_NAME"
kubectl rollout restart "deployment/actors-host" -n "$RELEASE_NAME"
kubectl rollout restart "deployment/merchant-manager" -n "$RELEASE_NAME"