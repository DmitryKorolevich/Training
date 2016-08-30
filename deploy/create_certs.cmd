openssl ecparam -genkey -name secp256r1 -out ca.key
openssl req -x509 -new -SHA256 -nodes -key ca.key -days 3650 -out ca.crt -subj /C=US/ST=Washington/L=Bellingham/O="Vital Choice Wild Seafood & Organics (Vital Choice Seafood, Inc)"/OU="SSL Dept"/CN=root.vitalchoice.com
openssl ecparam -genkey -name secp256r1 -out server.key
openssl req -new -SHA256 -key server.key -nodes -out server.csr -subj /C=US/ST=Washington/L=Bellingham/O="Vital Choice Wild Seafood & Organics (Vital Choice Seafood, Inc)"/OU="SSL Dept"/CN=export.vitalchoice.com
openssl x509 -req -SHA256 -days 3650 -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out server.crt
openssl ecparam -genkey -name secp256r1 -out client.key
openssl req -new -SHA256 -key client.key -nodes -out client.csr -subj /C=US/ST=Washington/L=Bellingham/O="Vital Choice Wild Seafood & Organics (Vital Choice Seafood, Inc)"/OU="SSL Dept"/CN=apps.vitalchoice.com
openssl x509 -req -SHA256 -days 3650 -in client.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out client.crt
openssl pkcs12 -export -out server.pfx -inkey server.key -in server.crt -certfile ca.crt
openssl pkcs12 -export -out client.pfx -inkey client.key -in client.crt -certfile ca.crt