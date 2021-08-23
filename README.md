# Jwt server demo
We want our *Jwt.Blazor* server app get a token from our *Jwt.Server* app & use it to provide auth to its own pages (based on claims) & also to connect to a couple of clients:
- Jwt.Client_SignalR
- Jwt.Client_Api
<br/><br/>

Using a pre fetched token we can successfully connect to bot clients.

At present we cannot figure out how to use the token in the *Jwt.Blazor* apps auth flow, eg using the Authorize atribute.  

