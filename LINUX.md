### Install kubernetes (microk8s)

```bash
sudo snap install microk8s --classic
```

Install required addons:

```bash
microk8s enable cert-manager
microk8s enable dashboard
microk8s enable dns
microk8s enable ha-cluster
microk8s enable helm
microk8s enable helm3
microk8s enable host-access
microk8s enable ingress
microk8s enable metrics-server
```

Add current user to microk8s group:

```bash
sudo usermod -a -G microk8s $USER
sudo chown -R $USER ~/.kube
```

For convinance create alias for kubectl:

```bash
sudo snap alias microk8s.kubectl kubectl
```

Edit iptables:

```bash
sudo nano /etc/iptables/rules.v4
```

Add this 2 lines:

```
-A FORWARD -i cni0 -j ACCEPT
-A FORWARD -o cni0 -j ACCEPT
```

Reload iptables:

```bash
sudo iptables-restore < /etc/iptables/rules.v4
```

### Install Flux (gitops)

```bash
curl -s https://fluxcd.io/install.sh | sudo bash
```

Add github-pat token and user:

```bash
export GITHUB_USER=github-username
export GITHUB_TOKEN=my-token
```

Bootstrap flux:

```bash
flux bootstrap github --owner=$GITHUB_USER --repository=shop-microservices --branch=master --path=./.flux --personal
```

### Install PostgreSQL

Configure apt repository and install PostgreSQL:

```bash
sudo apt install -y postgresql-common
sudo /usr/share/postgresql-common/pgdg/apt.postgresql.org.sh
sudo apt install postgresql-18
```

Configure db connection:

```bash
nano /etc/postgresql/18/main/postgresql.conf
```

Uncomment this line:

```
listen_addresses = '*'
```

Configure allowed IPs:

```bash
nano /etc/postgresql/18/main/pg_hba.conf
```

Add this line to allow microk8s to connect to db (via `10.0.1.1`):

```
host    all             all             10.0.0.0/8              scram-sha-256
```

Configure password for 'postgres' user:

```bash
sudo -u postgres psql

ALTER USER postgres WITH PASSWORD 'postgres';
```

Reload PostgreSQL:

```bash
sudo systemctl restart postgresql
```

### Install RabbitMQ

### Microservices .env variables

Gateway:

```bash
kubectl create secret generic gateway \
  --from-literal=ConnectionStrings__Users='' \
  --from-literal=ConnectionStrings__rabbitmq='' \
  --from-literal=Settings__Auth__Issuer= \
  --from-literal=Settings__Auth__Audience= \
  --from-literal=Settings__Auth__Secret="" \
  --from-literal=Settings__Auth__ExpireMinutes= \
  --namespace shop-prod
```
