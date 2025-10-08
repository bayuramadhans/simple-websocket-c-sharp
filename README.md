# WebSocket Chat Application

A real-time chat application built with ASP.NET Core and WebSocket technology. This simple but effective chat application demonstrates the implementation of real-time bidirectional communication between clients through a web browser interface.

## Features

- Real-time messaging using WebSocket protocol
- Clean and responsive user interface
- System notifications for user join/leave events
- Automatic reconnection on disconnection
- Concurrent user support
- Message broadcasting to all connected clients
- Static file serving with clean separation of concerns

## Prerequisites

- .NET 8.0 SDK
- A modern web browser that supports WebSocket

## Dependencies

- Microsoft.AspNetCore.StaticFiles - For serving static files from wwwroot directory

## Getting Started

1. Clone the repository
2. Navigate to the project directory
3. Run the application using:
   ```
   dotnet run
   ```
4. Open your web browser and navigate to `http://localhost:5000` (or the port specified in your environment)

## Technical Details

### Backend
- Built with ASP.NET Core
- Uses `System.Net.WebSockets` for WebSocket implementation
- Manages concurrent connections using `ConcurrentDictionary`
- JSON serialization for message formatting

### Frontend
- Pure HTML, CSS, and JavaScript implementation
- Responsive design
- Real-time updates without page refresh
- Connection status indicator
- Auto-reconnect functionality

## Usage

1. Open the application in your web browser
2. The application will automatically connect to the WebSocket server
3. Type your message in the input field
4. Press Enter or click the Send button to send your message
5. Messages will appear in real-time for all connected users

## Features
- Messages are broadcast to all connected clients
- System notifications for user connection/disconnection
- Visual connection status indicator
- Message history displayed in scrollable container
- Enter key support for sending messages

## Project Structure

- `Program.cs`: Contains the server implementation and WebSocket handling
- `wwwroot/`: Directory for static files
  - `index.html`: The main client-side application with HTML, CSS, and JavaScript
- WebSocket endpoint at `/ws`
- Main page served at root URL `/`

### Directory Structure
```
WebSocketChat/
├── Program.cs           # Backend server implementation
├── WebSocketChat.csproj # Project file with dependencies
├── wwwroot/            # Static files directory
│   └── index.html      # Frontend implementation
└── README.md           # Project documentation
```

## License

This project is open-source and available under the MIT License.