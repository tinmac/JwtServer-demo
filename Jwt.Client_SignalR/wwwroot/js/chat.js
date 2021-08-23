"use strict";

//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
this.connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {
        // Wendy Worker Token - 10 years
        accessTokenFactory: () => 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6ImVhMjQ3OGRlLTYyMTctNDdlYy1iNTMwLTNkMDllODFlOWExMiIsIm5hbWUiOiJXZW5keSBTbWl0aCIsIndvcmtlciI6InRydWUiLCJuYmYiOjE2Mjk3MzcxNjgsImV4cCI6MTk0NTI2OTk2OCwiaWF0IjoxNjI5NzM3MTY4fQ.lBoLrG2bu_4hSWw-tE3a1jvrkRoKS35m-9bR_MD0CyA'
    })
    .build();


//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});