import react, { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';

function App() {
  const [message, setMessage] = useState("");
  const [chat,setChat] = useState([]);
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7193/chatHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();
    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start().then(res => console.log("Connected!")).catch(e => console.log("there is an error", e))
      connection.on("ReceiveMessage", message => {
        setChat([...chat, ...message]);
      })
    }
  }, [connection])

  const handleMessageChange = (e) => {
    setMessage(e.target.value);
  };

  const handleSend = (e) => {
    e.preventDefault();
    fetch('https://localhost:7193/api/message', {
      method: "POST",
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        message
      })
    });
    setMessage("");
  }

  return (
    <div>
      <h1>Welcome to Chat application</h1>
      <div>
        {chat.map((t, index) => <p key={index}>{t}</p>)}
      </div>
      <form>
        <label>Enter message</label>
        <input name="message" value={message} onChange={handleMessageChange} />
        <button onClick={handleSend}>Send</button>
      </form>
    </div>
  );
}

export default App;
