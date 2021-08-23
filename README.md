# Jwt-demo
We want to get a token from our  web app *Jwt.Server* & use it to connect to a couple of clients:
- Jwt.Client_SignalR
- Jwt.Client_Api
<br/><br/>

Each of the clients will have Jwt Middleware that looks for a **UserType** Claim that we created.
