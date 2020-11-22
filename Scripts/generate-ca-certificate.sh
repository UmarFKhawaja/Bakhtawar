#!/usr/bin/env bash

mkdir -p ~/Keys
pushd ~/Keys

rm huntingdonresearch.srl

openssl genrsa -des3 -out huntingdonresearch.key 4096

openssl req -x509 -new -nodes -key huntingdonresearch.key -subj "/emailAddress=info@huntingdonresearch.com/C=UK/ST=West Midlands/L=Solihull/O=Huntingdon Research/OU=Research and Development/CN=Huntingdon Research CA" -sha256 -days 3653 -out huntingdonresearch.crt

rm -f huntingdonresearch.pem
touch huntingdonresearch.pem
cat huntingdonresearch.crt >> huntingdonresearch.pem
cat huntingdonresearch.key >> huntingdonresearch.pem

rm -f huntingdonresearch.pfx
openssl pkcs12 -export -out huntingdonresearch.pfx -inkey huntingdonresearch.key -in huntingdonresearch.crt

popd
